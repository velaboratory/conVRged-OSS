using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VelNet;

namespace conVRged
{
	public class RemoteNametagVelNet : MonoBehaviour
	{
		public Transform nameTagObject;
		public Text nameTagText;
		public VelNetWolf3DAvatarController remoteAvatarPrefab;
		public Wolf3DAvatar remoteAvatar;
		[Range(0, 2)] public float heightAboveHead = .9f;

		[FormerlySerializedAs("photonSpeakerAmplitude")]
		public Wolf3DSpeakerAmplitude speakerAmplitude;

		public Transform speakerObj;
		public Slider speakerVolumeSlider;
		public NetworkObject playerNetworkObject;

		private float nextUpdateTime = 0;

		private void Start()
		{
			nameTagObject.gameObject.SetActive(!playerNetworkObject.IsMine);
		}

		// Update is called once per frame
		private void Update()
		{
			nameTagText.text = remoteAvatarPrefab.nickName;

			if (remoteAvatar != null && remoteAvatar.BodyPart != null && remoteAvatar.avatar != null)
			{
				nameTagObject.position = remoteAvatar.BodyPart.Transform.position + Vector3.up * heightAboveHead;
				nameTagObject.LookAt(GameManager.instance.player.rig.head);
				nameTagObject.Rotate(0, 180, 0, Space.Self);

				if (nextUpdateTime < Time.time)
				{
					if (speakerObj != null)
					{
						speakerObj.gameObject.SetActive(speakerAmplitude.GetAmplitude() > .5f);
						speakerVolumeSlider.value = speakerAmplitude.GetAmplitude();
					}

					// only update every .1 seconds
					nextUpdateTime = Time.time + .1f;
				}
			}
		}
	}
}