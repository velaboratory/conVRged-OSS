using System;
using System.Collections;
using System.Collections.Generic;
using conVRged;
using MenuTablet;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using unityutilities;
using Wolf3D.ReadyPlayerMe.AvatarSDK;

[Serializable]
public class Wolf3DBodyPart
{
	private Transform transform;

	public Transform Transform
	{
		get => transform;
		set
		{
			if (transform != value)
			{
				color = null;
				renderer = null;
				skinnedMeshRenderer = null;
			}

			transform = value;
		}
	}

	private Renderer renderer;

	public Renderer Renderer
	{
		get
		{
			if (renderer == null && transform != null)
			{
				renderer = transform.GetComponent<Renderer>();
			}

			return renderer;
		}
	}

	private SkinnedMeshRenderer skinnedMeshRenderer;

	public SkinnedMeshRenderer SkinnedMeshRenderer
	{
		get
		{
			if (skinnedMeshRenderer == null)
			{
				skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
			}

			if (skinnedMeshRenderer == null)
			{
				skinnedMeshRenderer = transform.GetComponentInParent<SkinnedMeshRenderer>();
			}

			return skinnedMeshRenderer;
		}
	}

	private Color? color;

	public Color? Color
	{
		get => color;
		set
		{
			if (color != value && value != null && Renderer != null && Renderer.material != null)
			{
				Renderer.material.SetColor("_Color", (Color)value);
			}

			color = value;
		}
	}
}

[Serializable]
public class Wolf3DHand : Wolf3DBodyPart
{
	public Side side;
	public List<List<Transform>> fingers;

	private float trigger;

	public float Trigger
	{
		get => trigger;
		set
		{
			trigger = value;
			if (fingers == null) FindFingers();
			fingers[0][0].localRotation = Quaternion.Slerp(Quaternion.Euler(5, 0, 0), Quaternion.Euler(69, -21, -22), value);
			fingers[0][1].localRotation = Quaternion.Slerp(Quaternion.Euler(5, 0, 0), Quaternion.Euler(70, 0, 0), value);
			fingers[0][2].localRotation = Quaternion.Slerp(Quaternion.Euler(5, 0, 0), Quaternion.Euler(70, 0, 0), value);
		}
	}

	private float grip;

	public float Grip
	{
		get => grip;
		set
		{
			grip = value;
			if (fingers == null) FindFingers();
			for (int i = 1; i < 4; i++)
			{
				fingers[i][0].localRotation = Quaternion.Slerp(Quaternion.Euler(5, 0, 0), Quaternion.Euler(70, 0, 0), value);
				fingers[i][1].localRotation = Quaternion.Slerp(Quaternion.Euler(5, 0, 0), Quaternion.Euler(70, 0, 0), value);
				fingers[i][2].localRotation = Quaternion.Slerp(Quaternion.Euler(5, 0, 0), Quaternion.Euler(70, 0, 0), value);
			}
		}
	}

	private float thumb;

	public float Thumb
	{
		get => thumb;
		set
		{
			thumb = value;
			if (fingers == null) FindFingers();
			Quaternion rot1 = Quaternion.Slerp(Quaternion.Euler(-22, 28, 34).MirrorX(), Quaternion.Euler(-9, -46, -22), value);
			Quaternion rot2 = Quaternion.Slerp(Quaternion.Euler(28, 3, 0).MirrorX(), Quaternion.Euler(28, -3, 0), value);
			Quaternion rot3 = Quaternion.Slerp(Quaternion.Euler(14, -3, 0).MirrorX(), Quaternion.Euler(57, 0, 7), value);

			fingers[4][0].localRotation = side == Side.Left ? rot1 : rot1.MirrorX();
			fingers[4][1].localRotation = side == Side.Left ? rot2 : rot2.MirrorX();
			fingers[4][2].localRotation = side == Side.Left ? rot3 : rot3.MirrorX();
		}
	}

	private void FindFingers()
	{
		fingers = new List<List<Transform>>();
		for (int f = 0; f < Transform.childCount; f++)
		{
			fingers.Add(new List<Transform>());
			Transform knuckle = Transform.GetChild(f);
			fingers[f].Add(knuckle);
			while (knuckle.childCount > 0)
			{
				knuckle = knuckle.GetChild(0);
				fingers[f].Add(knuckle);
			}
		}
	}
}

public abstract class Wolf3DSpeakerAmplitude : MonoBehaviour
{
	public abstract float GetAmplitude();
}

public class Wolf3DAvatar : MonoBehaviour, AvatarBase
{
	[ReadOnly] public Rig rig;
	[HideInInspector] public VELPlayer player;
	public CopyTransform copyTransform;
	public Transform avatar;
	public GameObject defaultAvatar;
	public SkinnedMeshRenderer handsOnlyMesh;
	public Transform emptyBone;
	public bool showLocalBody;
	public bool showLocalHands;
	public bool showLocalHead;
	private bool local = true;
	public GameObject fingerInteractorPrefab;
	public GameObject fingerColliderFollowerPrefab;
	public GameObject watchPrefab;
	public Vector3[] watchPosOffsets;
	public Vector3[] watchRotOffsets;
	public Transform Head => HeadPart?.Transform;
	public Transform LeftHand => HandLeftPart?.Transform;
	public Transform RightHand => HandRightPart?.Transform;


	public bool Local
	{
		get => local;
		set
		{
			local = value;
			if (local)
			{
				rig = GameManager.instance.player.rig;
				player = GameManager.instance.player;
				if (copyTransform != null) copyTransform.SetTarget(rig.transform, false);
			}
			else
			{
				rig = null;
				player = null;
				if (copyTransform != null) copyTransform.SetTarget(null, false);
			}
			// disable a lot of parts on local avatar

			if (avatar == null) return;

			if (local && !showLocalBody) Shirt.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead) EyeLeft.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead) EyeRight.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead && FacialHair.Transform != null) FacialHair.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead && Hair.Transform != null) Hair.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			handsOnlyMesh.enabled = false;
			if (local && !showLocalHands)
			{
				HandsMesh.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
				handsOnlyMesh.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			}
			else if (local)
			{
				HandsMesh.Transform.gameObject.layer = LayerMask.NameToLayer("Hands");
				handsOnlyMesh.gameObject.layer = LayerMask.NameToLayer("Hands");
				handsOnlyMesh.rootBone = avatar.Find("Armature/Hips");
				Transform[] newBones = HandsMesh.SkinnedMeshRenderer.bones;
				newBones[2] = emptyBone;
				newBones[3] = emptyBone;
				newBones[4] = emptyBone;
				newBones[5] = emptyBone;
				handsOnlyMesh.bones = newBones;
				handsOnlyMesh.sharedMaterial = HandsMesh.SkinnedMeshRenderer.sharedMaterial;
				handsOnlyMesh.enabled = true;
			}

			if (local && !showLocalHead) HeadMesh.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead) Teeth.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead && Glasses.Transform != null) Glasses.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead && Facewear.Transform != null) Facewear.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
			if (local && !showLocalHead && Headwear.Transform != null) Headwear.Transform.gameObject.layer = LayerMask.NameToLayer("MirrorOnly");
		}
	}

	public float bodyToEyeDistance = 0.56f;

	public float headToEyeDistance;

	public Wolf3DBodyPart BodyPart;
	public Wolf3DBodyPart HeadPart;
	public Wolf3DBodyPart HandsMesh;
	public Wolf3DBodyPart HeadMesh;
	public Wolf3DHand HandLeftPart;
	public Wolf3DHand HandRightPart;
	public Wolf3DBodyPart Shirt;
	public Wolf3DBodyPart EyeLeft;
	public Wolf3DBodyPart EyeRight;
	public Wolf3DBodyPart FacialHair;
	public Wolf3DBodyPart Glasses;
	public Wolf3DBodyPart Facewear;
	public Wolf3DBodyPart Headwear;
	public Wolf3DBodyPart Hair;
	public Wolf3DBodyPart Teeth;
	public Wolf3DHand ControllerLeft;
	public Wolf3DHand ControllerRight;

	public Transform speakerIndicator;

	public Wolf3DSpeakerAmplitude speakerAmplitude;

	public struct AvatarState
	{
		/// <summary>
		/// Local position in rig space
		/// </summary>
		public Vector3 headPos;

		/// <summary>
		/// Local rotation in rig space
		/// </summary>
		public Quaternion headRot;

		/// <summary>
		/// Local position in rig space
		/// </summary>
		public Vector3 leftHandPos;

		/// <summary>
		/// Local rotation in rig space
		/// </summary>
		public Quaternion leftHandRot;

		/// <summary>
		/// Local position in rig space
		/// </summary>
		public Vector3 rightHandPos;

		/// <summary>
		/// Local rotation in rig space
		/// </summary>
		public Quaternion rightHandRot;

		public float triggerLeft;
		public float triggerRight;
		public float gripLeft;
		public float gripRight;
		public float thumbLeft;
		public float thumbRight;

		/// <summary>
		/// Used to completely hide hands for grabbing or hand tracking 
		/// </summary>
		public bool leftHandVisible;

		public bool rightHandVisible;
	}

	public Action AvatarFinishedLoading;

	/// <summary>
	/// The current actual state displayed on the avatar
	/// </summary>
	public AvatarState state;

	/// <summary>
	/// The most recent state received from the network
	/// </summary>
	private AvatarState networkState;

	/// <summary>
	/// The second most recent state received from the network
	/// </summary>
	private AvatarState lastNetworkState;

	public float serializationRate = 1;

	// Start is called before the first frame update
	private void Awake()
	{
		if (avatar != null)
		{
			FindBodyParts();
			Local = Local;

			// set the mesh bounds to be large so that frustrum culling doesn't hide stuff all the time
			Mesh mesh = HandsMesh.Transform.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			mesh.bounds = new Bounds(mesh.bounds.center, mesh.bounds.extents * 10);
		}
	}

	private void Update()
	{
		if (avatar == null || !avatar.gameObject.activeInHierarchy) return;

		if (local)
		{
			state = new AvatarState()
			{
				headPos = rig.head.localPosition,
				headRot = rig.head.localRotation,
				leftHandPos = rig.transform.InverseTransformPoint(player.leftHandAvatarAnchor.position),
				leftHandRot = Quaternion.Inverse(rig.transform.rotation) * player.leftHandAvatarAnchor.rotation,
				rightHandPos = rig.transform.InverseTransformPoint(player.rightHandAvatarAnchor.position),
				rightHandRot = Quaternion.Inverse(rig.transform.rotation) * player.rightHandAvatarAnchor.rotation,

				triggerLeft = InputMan.TriggerValue(Side.Left),
				triggerRight = InputMan.TriggerValue(Side.Right),
				gripLeft = InputMan.GripValue(Side.Left),
				gripRight = InputMan.GripValue(Side.Right),
				thumbLeft = InputMan.ThumbstickPress(Side.Left) || InputMan.Button1(Side.Left) || InputMan.Button2(Side.Left) ? 1f : 0f,
				thumbRight = InputMan.ThumbstickPress(Side.Right) || InputMan.Button1(Side.Right) || InputMan.Button2(Side.Right) ? 1f : 0f,

				leftHandVisible = !player.leftTrackedHandVisibleLocal,
				rightHandVisible = !player.rightTrackedHandVisibleLocal,
			};
		}
		else
		{
			// state is set from photonavatarview using SetState()
			// do interpolation here
			state.headPos = Interpolate(lastNetworkState.headPos, state.headPos, networkState.headPos, serializationRate);
			state.headRot = Interpolate(lastNetworkState.headRot, state.headRot, networkState.headRot, serializationRate);
			state.leftHandPos = Interpolate(lastNetworkState.leftHandPos, state.leftHandPos, networkState.leftHandPos, serializationRate);
			state.leftHandRot = Interpolate(lastNetworkState.leftHandRot, state.leftHandRot, networkState.leftHandRot, serializationRate);
			state.rightHandPos = Interpolate(lastNetworkState.rightHandPos, state.rightHandPos, networkState.rightHandPos, serializationRate);
			state.rightHandRot = Interpolate(lastNetworkState.rightHandRot, state.rightHandRot, networkState.rightHandRot, serializationRate);

			state.triggerLeft = Mathf.Lerp(state.triggerLeft, networkState.triggerLeft, .2f);
			state.triggerRight = Mathf.Lerp(state.triggerRight, networkState.triggerRight, .2f);
			state.gripLeft = Mathf.Lerp(state.gripLeft, networkState.gripLeft, .2f);
			state.gripRight = Mathf.Lerp(state.gripRight, networkState.gripRight, .2f);
			state.thumbLeft = Mathf.Lerp(state.thumbLeft, networkState.thumbLeft, .2f);
			state.thumbRight = Mathf.Lerp(state.thumbRight, networkState.thumbRight, .2f);

			state.leftHandVisible = networkState.leftHandVisible;
			state.rightHandVisible = networkState.rightHandVisible;
		}

		Vector3 lookDir = state.headRot * Vector3.forward;
		lookDir.y = 0;
		if (lookDir != Vector3.zero)
		{
			BodyPart.Transform.rotation = transform.rotation * Quaternion.LookRotation(lookDir, Vector3.up);
		}

		BodyPart.Transform.position = transform.TransformPoint(state.headPos - Vector3.up * bodyToEyeDistance);
		HeadPart.Transform.position = transform.TransformPoint(state.headPos - Vector3.up * headToEyeDistance);
		HeadPart.Transform.rotation = transform.rotation * state.headRot;

		HandLeftPart.Transform.position = transform.TransformPoint(state.leftHandPos);
		HandLeftPart.Transform.rotation = transform.rotation * state.leftHandRot;

		HandRightPart.Transform.position = transform.TransformPoint(state.rightHandPos);
		HandRightPart.Transform.rotation = transform.rotation * state.rightHandRot;

		// finger positions
		HandLeftPart.Trigger = state.triggerLeft;
		HandRightPart.Trigger = state.triggerRight;
		HandLeftPart.Grip = state.gripLeft;
		HandRightPart.Grip = state.gripRight;
		HandLeftPart.Thumb = state.thumbLeft;
		HandRightPart.Thumb = state.thumbRight;

		HandLeftPart.Transform.localScale = state.leftHandVisible ? Vector3.one : Vector3.zero;
		HandRightPart.Transform.localScale = state.rightHandVisible ? Vector3.one : Vector3.zero;

		if (speakerIndicator != null) speakerIndicator.transform.position = HeadPart.Transform.position;
		if (speakerAmplitude != null)
		{
			HeadMesh.SkinnedMeshRenderer.SetBlendShapeWeight(0, speakerAmplitude.GetAmplitude());
		}
	}

	private void FindBodyParts()
	{
		BodyPart.Transform = avatar.Find("Armature");
		HeadPart.Transform = avatar.Find("Armature/Hips/Spine/Neck/Head");
		HandLeftPart.Transform = avatar.Find("Armature/Hips/Spine/LeftHand");
		HandLeftPart.fingers = null;
		HandRightPart.Transform = avatar.Find("Armature/Hips/Spine/RightHand");
		HandRightPart.fingers = null;

		// if split avatars
		if (avatar.Find("Wolf3D.Avatar_Renderer_EyeLeft") != null)
		{
			EyeLeft.Transform = avatar.Find("Wolf3D.Avatar_Renderer_EyeLeft");
			EyeRight.Transform = avatar.Find("Wolf3D.Avatar_Renderer_EyeRight");
			FacialHair.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Beard");
			Hair.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Hair");
			HandsMesh.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Hands");
			if (HandsMesh.Transform != null) HandsMesh.Transform.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = false;
			HeadMesh.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Head");
			Facewear.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Facewear");
			Headwear.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Headwear");
			Glasses.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Glasses");
			Shirt.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Shirt");
			Teeth.Transform = avatar.Find("Wolf3D.Avatar_Renderer_Teeth");
		}
		else
		{
			Transform rend = avatar.Find("Wolf3D.Avatar_Renderer_Avatar");
			Transform rendTransparent = avatar.Find("Wolf3D.Avatar_Renderer_Avatar_Transparent");
			if (!rend)
			{
				rend = avatar.Find("Wolf3D_Avatar");
			}

			EyeLeft.Transform = rend;
			EyeRight.Transform = rend;
			FacialHair.Transform = rend;
			Hair.Transform = rend;
			HandsMesh.Transform = rend;
			if (HandsMesh.Transform != null) HandsMesh.Transform.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = false;
			HeadMesh.Transform = rend;
			Facewear.Transform = rendTransparent;
			Headwear.Transform = rend;
			Glasses.Transform = rend;
			Shirt.Transform = rend;
			Teeth.Transform = rend;
		}
	}

	public void SetAvatarURL(string avatarUrl)
	{
		// selected default avatars
		// male albino
		// https://d1a370nemizbjq.cloudfront.net/109539a0-a690-49d7-b2e8-e3b30b8335dc.glb
		// female albino
		// https://d1a370nemizbjq.cloudfront.net/88309f4c-d792-4e39-851f-6fa7e468d2f4.glb
		// male normal
		// https://d1a370nemizbjq.cloudfront.net/109539a0-a690-49d7-b2e8-e3b30b8335dc.glb
		// female normal
		// https://d1a370nemizbjq.cloudfront.net/a59f6904-bbf8-415f-bc12-0c57bb6afab2.glb
		// blue face
		// https://d1a370nemizbjq.cloudfront.net/a52178c5-4590-403a-9ed6-e879ef1478d3.glb
		// pink hair
		// https://d1a370nemizbjq.cloudfront.net/c77a2b3b-c352-4a7e-b447-8e676a030f74.glb

		if (string.IsNullOrEmpty(avatarUrl))
		{
			GameObject a = Instantiate(defaultAvatar);

			StartCoroutine(DelayInvoke(.1f, () => { AvatarLoadedCallback(a, null); }));
		}
		else
		{
			AvatarLoader avatarLoader = new AvatarLoader();
			avatarLoader.LoadAvatar(avatarUrl, AvatarImportedCallback, AvatarLoadedCallback);
		}
	}

	private IEnumerator DelayInvoke(float seconds, Action callback)
	{
		yield return new WaitForSeconds(seconds);
		callback();
	}


	// private void ManualAvatarLoading(GameObject newAvatar)
	// {
	// 	// newAvatar.transform.SetParent(transform);
	// 	// newAvatar.transform.localPosition = Vector3.zero;
	// 	// newAvatar.transform.localRotation = Quaternion.identity;
	// 	// newAvatar.transform.localScale = Vector3.one;
	// 	avatar.gameObject.SetActive(false);
	// 	newAvatar.gameObject.SetActive(true);
	// 	avatar = newAvatar.transform;
	// 	FindBodyParts();
	// 	FingerTouchHandRPMe fingerTipCreator = newAvatar.GetComponent<FingerTouchHandRPMe>();
	// 	if (!local) fingerTipCreator.enabled = false;
	// 	Local = local;
	// }

	private void AvatarImportedCallback(GameObject newAvatar)
	{
		// Debug.Log("Avatar Imported");
	}

	private void AvatarLoadedCallback(GameObject newAvatar, AvatarMetaData metaData)
	{
		Debug.Log("Avatar Loaded");

		if (this == null) return;

		CopyTransform cp = newAvatar.AddComponent<CopyTransform>();
		cp.SetTarget(transform, false);
		cp.followPosition = true;
		cp.followRotation = true;
		cp.positionFollowType = CopyTransform.FollowType.Copy;
		cp.rotationFollowType = CopyTransform.FollowType.Copy;
		newAvatar.transform.localScale = Vector3.one * transform.lossyScale.x;

		if (avatar != null) Destroy(avatar.gameObject);
		avatar = newAvatar.transform;
		FindBodyParts();

		FingerTouchHandRPMe fingerTipCreator = newAvatar.AddComponent<FingerTouchHandRPMe>();
		fingerTipCreator.fingerInteractorPrefab = fingerInteractorPrefab;
		fingerTipCreator.FindFingerTips();


		if (local)
		{
			// don't spawn watches anymore. This is just for the controller version

			// Transform leftTip = avatar.Find("Armature/Hips/Spine/LeftHand");
			// GameObject leftWatch = Instantiate(watchPrefab, Vector3.zero, Quaternion.identity, leftTip);
			// leftWatch.transform.localPosition = watchPosOffsets[0];
			// leftWatch.transform.localEulerAngles = watchRotOffsets[0];
			// leftWatch.GetComponent<WatchController>().side = Side.Left;
			// Transform rightTip = avatar.Find("Armature/Hips/Spine/RightHand");
			// GameObject rightWatch = Instantiate(watchPrefab, Vector3.zero, Quaternion.identity, rightTip);
			// rightWatch.transform.localPosition = watchPosOffsets[1];
			// rightWatch.transform.localEulerAngles = watchRotOffsets[1];
			// rightWatch.GetComponent<WatchController>().side = Side.Right;

			Transform leftTip = avatar.Find("Armature/Hips/Spine/LeftHand/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3");
			if (leftTip != null)
			{
				GameObject obj = Instantiate(fingerColliderFollowerPrefab, Vector3.zero, Quaternion.identity, leftTip);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
			}

			Transform rightTip = avatar.Find("Armature/Hips/Spine/RightHand/RightHandIndex1/RightHandIndex2/RightHandIndex3");
			if (rightTip != null)
			{
				GameObject obj = Instantiate(fingerColliderFollowerPrefab, Vector3.zero, Quaternion.identity, rightTip);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
			}
		}


		EyeAnimationHandler eyeMovement = newAvatar.GetComponent<EyeAnimationHandler>();
		if (eyeMovement == null)
		{
			eyeMovement = newAvatar.AddComponent<EyeAnimationHandler>();
		}

		if (!local) fingerTipCreator.enabled = false;
		Local = local;

		try
		{
			AvatarFinishedLoading?.Invoke();
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}

	private void OnDestroy()
	{
		if (avatar != null) Destroy(avatar.gameObject);
	}

	private IEnumerator InvokeWithDelay(Action action, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		action();
	}

	public void SetState(AvatarState newState, float serializationRateHz)
	{
		networkState = newState;
		// deep copy
		lastNetworkState = new AvatarState
		{
			headPos = state.headPos,
			headRot = state.headRot,
			leftHandPos = state.leftHandPos,
			leftHandRot = state.leftHandRot,
			rightHandPos = state.rightHandPos,
			rightHandRot = state.rightHandRot,

			// send finger positions
			triggerLeft = state.triggerLeft,
			triggerRight = state.triggerRight,
			gripLeft = state.gripLeft,
			gripRight = state.gripRight,
			thumbLeft = state.thumbLeft,
			thumbRight = state.thumbRight,

			leftHandVisible = state.leftHandVisible,
			rightHandVisible = state.rightHandVisible,
		};
		serializationRate = serializationRateHz;
	}

	private static Vector3 Interpolate(Vector3 initial, Vector3 current, Vector3 target, float serializationRate)
	{
		float distance = Vector3.Distance(initial, target);
		return Vector3.MoveTowards(current, target, Time.deltaTime * distance * serializationRate);
	}

	private static Quaternion Interpolate(Quaternion initial, Quaternion current, Quaternion target, float serializationRate)
	{
		float angle = Quaternion.Angle(initial, target);
		return Quaternion.RotateTowards(current, target, Time.deltaTime * angle * serializationRate);
	}
}