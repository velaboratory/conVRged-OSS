using System.Collections.Generic;
using System.IO;
using System.Linq;
using MenuTablet;
using UnityEngine;
using UnityEngine.UI;
using unityutilities;
using Random = System.Random;

namespace conVRged
{
	// this namespace is defined in an inner scope to avoid conflicts with binary reader overloads
	using VelNet;

	public class VelNetWolf3DAvatarController : SyncState
	{
		public Wolf3DAvatar avatar;
		[ReadOnly] public Rig rig;
		private HandBase leftHand;
		private HandBase rightHand;
		public RemoteHand leftHandRemote;
		public RemoteHand rightHandRemote;

		public VelNetSyncHand leftHandTracked;
		public VelNetSyncHand rightHandTracked;
		public Renderer leftLocalHand;
		public Renderer rightLocalHand;
		
		public GameObject remoteMenuTabletStatic;
		private bool init;

		private float playerScale = 1;
		private Quaternion networkLeftHandRot;
		private Quaternion networkRightHandRot;

		public bool spectatorMode;

		public bool laserPointerVisible;
		public bool videoPanelVisible;
		private bool videoPanelWasVisible;
		private GameObject videoPanelObject;
		public GameObject videoPanelPrefab;
		public GameObject videoPanelParent;

		private readonly float[] audioBuffer = new float[1024];
		public Slider audioLevelSlider;

		/// <summary>
		/// Increments every time the avatar needs to update, even if the url is the same
		/// </summary>
		public int avatarUrlIndex;

		public string avatarUrl;

		public string nickName = "";


		public static string HeadsetShortCode
		{
			get
			{
				Hash128 hash = new Hash128();
				hash.Append(SystemInfo.deviceUniqueIdentifier);
				const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
				Random random = new Random(hash.GetHashCode());
				return new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
			}
		}

		private void OnEnable()
		{
			VelNetMan.AvatarURLChanged += OnAvatarURLChanged;
		}

		private void OnDisable()
		{
			VelNetMan.AvatarURLChanged -= OnAvatarURLChanged;
		}

		public void Start()
		{
			avatar.Local = IsMine;
			VelNetMan.playerPrefabs[Owner.userid] = this;


			// if this is a local avatar
			if (IsMine)
			{
				leftHand = GameManager.instance.player.leftHand;
				rightHand = GameManager.instance.player.rightHand;

				rig = GameManager.instance.player.rig;

				AddToolsToFingersWolf3D addToolsToFingers = GetComponent<AddToolsToFingersWolf3D>();
				if (addToolsToFingers)
				{
					addToolsToFingers.rig = rig;
				}

				leftHandTracked.skeleton = GameManager.instance.player.leftOVRSkeleton;
				rightHandTracked.skeleton = GameManager.instance.player.rightOVRSkeleton;

				OnAvatarURLChanged(VelNetMan.GetAvatarURL());
			}
			// if this is a remote avatar
			else
			{
				leftHand = leftHandRemote;
				rightHand = rightHandRemote;
			}
			
			avatar.AvatarFinishedLoading += () =>
			{
				Material mat = avatar.HandsMesh.Renderer.sharedMaterial;
				leftHandTracked.rend.material = mat;
				rightHandTracked.rend.material = mat;
			};

			init = true;
		}


		private void Update()
		{
			if (!init) return;

			if (avatar == null) return;

			switch (IsMine)
			{
				// if this is a local avatar
				case true:
				{
					break;
				}
				// if this is a remote avatar
				case false:


					// Hand objs
					leftHand.transform.localPosition = avatar.state.leftHandPos;
					leftHand.transform.localRotation = networkLeftHandRot;
					rightHand.transform.localPosition = avatar.state.rightHandPos;
					rightHand.transform.localRotation = networkRightHandRot;

					transform.localScale = Vector3.one * playerScale;
					break;
			}
		}

		private void OnDestroy()
		{
			VelNetMan.playerPrefabs.Remove(Owner.userid);
		}

		private void OnAvatarURLChanged(string url)
		{
			if (!IsMine) return;
			avatarUrlIndex++;
			avatarUrl = url;
			avatar.SetAvatarURL(url);
		}

		protected override void ReceiveState(BinaryReader reader)
		{
			Wolf3DAvatar.AvatarState newState = new Wolf3DAvatar.AvatarState()
			{
				headPos = reader.ReadVector3(),
				headRot = reader.ReadQuaternion(),
				leftHandPos = reader.ReadVector3(),
				leftHandRot = reader.ReadQuaternion(),
				rightHandPos = reader.ReadVector3(),
				rightHandRot = reader.ReadQuaternion(),

				// read finger positions
				triggerLeft = reader.ReadSingle(),
				triggerRight = reader.ReadSingle(),
				gripLeft = reader.ReadSingle(),
				gripRight = reader.ReadSingle(),
				thumbLeft = reader.ReadSingle(),
				thumbRight = reader.ReadSingle(),
				
				leftHandVisible = reader.ReadBoolean(),
				rightHandVisible = reader.ReadBoolean(),
			};
			if (avatar != null) avatar.SetState(newState, serializationRateHz);

			// player scale
			//playerScale = reader.ReadSingle();

			networkLeftHandRot = reader.ReadQuaternion();
			networkRightHandRot = reader.ReadQuaternion();

			HandBase.Tool leftHandTool = (HandBase.Tool)reader.ReadByte();
			HandBase.Tool rightHandTool = (HandBase.Tool)reader.ReadByte();
			// leftHand.SwitchTool(leftHandTool);
			// rightHand.SwitchTool(rightHandTool);

			Vector3 menuTabletPosition = reader.ReadVector3();
			Quaternion menuTabletRotation = reader.ReadQuaternion();

			byte bitmaskBytes = reader.ReadByte();
			List<bool> bools = bitmaskBytes.GetBitmaskValues();


			string remoteHeadsetShortCode = reader.ReadString();
			spectatorMode = reader.ReadBoolean();

			if (leftHand != null)
			{
				leftHand.ShowLASER(bools[0]);
				rightHand.ShowLASER(bools[1]);

				leftHand.ShowRemoteTeleporter(bools[2]);
				rightHand.ShowRemoteTeleporter(bools[3]);

				remoteMenuTabletStatic.SetActive(bools[4]);
				remoteMenuTabletStatic.transform.position = menuTabletPosition;
				remoteMenuTabletStatic.transform.rotation = menuTabletRotation;

				// bool videoPanelVisibleNew = bools[5];
				// // panel switched on
				// if (videoPanelVisibleNew && videoPanelObject == null)
				// {
				// 	videoPanelObject = Instantiate(videoPanelPrefab, videoPanelParent.transform);
				// 	string newURL = $"https://vdo.ninja/?view={remoteHeadsetShortCode}&cleanish&noaudio&hidemenu";
				// 	CanvasWebViewPrefab webView = videoPanelObject.GetComponentInChildren<CanvasWebViewPrefab>();
				// 	webView.InitialUrl = newURL;
				// 	// webView..LoadUrl(newURL);
				// }
				// // panel switched off
				// else if (!videoPanelVisibleNew && videoPanelObject != null)
				// {
				// 	Destroy(videoPanelObject);
				// 	Resources.UnloadUnusedAssets();
				// }
			}

			int newAvatarUrlIndex = reader.ReadInt32();
			string newAvatarUrl = reader.ReadString();
			nickName = reader.ReadString();

			if (newAvatarUrlIndex != avatarUrlIndex)
			{
				avatar.SetAvatarURL(newAvatarUrl);

				avatarUrl = newAvatarUrl;
				avatarUrlIndex = newAvatarUrlIndex;
			}

			audioLevelSlider.value = reader.ReadSingle();
			
			string voskText = reader.ReadString();
		}

		protected override void SendState(BinaryWriter writer)
		{
			writer.Write(rig.head.localPosition);
			writer.Write(rig.head.localRotation);

			// local hand positions with offsets
			writer.Write(rig.transform.InverseTransformPoint(GameManager.instance.player.leftHandAvatarAnchor.position));
			Quaternion rigRotation = rig.transform.rotation;
			writer.Write(Quaternion.Inverse(rigRotation) * GameManager.instance.player.leftHandAvatarAnchor.rotation);
			writer.Write(rig.transform.InverseTransformPoint(GameManager.instance.player.rightHandAvatarAnchor.position));
			writer.Write(Quaternion.Inverse(rigRotation) * GameManager.instance.player.rightHandAvatarAnchor.rotation);

			// send finger positions
			writer.Write(InputMan.TriggerValue(Side.Left));
			writer.Write(InputMan.TriggerValue(Side.Right));
			writer.Write(InputMan.GripValue(Side.Left));
			writer.Write(InputMan.GripValue(Side.Right));
			writer.Write(InputMan.ThumbstickPress(Side.Left) || InputMan.Button1(Side.Left) || InputMan.Button2(Side.Left) ? 1f : 0f);
			writer.Write(InputMan.ThumbstickPress(Side.Right) || InputMan.Button1(Side.Right) || InputMan.Button2(Side.Right) ? 1f : 0f);
			
			writer.Write(!GameManager.instance.player.leftTrackedHandVisibleLocal);
			writer.Write(!GameManager.instance.player.rightTrackedHandVisibleLocal);

			// player scale
			//writer.Write(rig.transform.localScale.x);

			writer.Write(Quaternion.Inverse(rigRotation) * rig.leftHand.rotation);
			writer.Write(Quaternion.Inverse(rigRotation) * rig.rightHand.rotation);

			writer.Write((byte)leftHand.currentTool);
			writer.Write((byte)rightHand.currentTool);

			writer.Write(MenuTabletMover.instance.transform.position);
			writer.Write(MenuTabletMover.instance.transform.rotation);

			// create a bitmask from some bools
			List<bool> bools = new List<bool>
			{
				leftHand.LASERShowing,
				rightHand.LASERShowing,
				leftHand.TeleporterShowing,
				rightHand.TeleporterShowing,
				MenuTabletMover.isVisible,
				videoPanelVisible,
			};
			writer.Write(bools.GetBitmasks());

			// stuff that was CustomProperties before
			writer.Write(HeadsetShortCode);
			writer.Write(spectatorMode);
			writer.Write(avatarUrlIndex);
			writer.Write(avatarUrl ?? "");
			writer.Write(nickName ?? "");
			AudioListener.GetOutputData(audioBuffer, 0);
#if UNITY_ANDROID && !UNITY_EDITOR
			writer.Write(Mathf.Clamp01(audioBuffer.Average() * 20));
			// writer.Write(Mathf.Clamp01(audioBuffer.Average() * 20 * OVRPlugin.systemVolume));
#else
			writer.Write(Mathf.Clamp01(audioBuffer.Average() * 20));
#endif
			writer.Write(VoskController.instance.currentText);
		}
	}
}
