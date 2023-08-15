#define OCULUS_INTEGRATION

using System;
using System.Collections;
using MenuTablet;
using UnityEngine;
using UnityEngine.UI;
using unityutilities;

public enum AvatarType
{
	OVRAvatar,
	Wolf3DAvatar,
	OVRHands,
	None
}

public class AddWatchesToAvatar : MonoBehaviour
{
	public AvatarType avatarType;
	public GameObject watchPrefab;
#if OCULUS_INTEGRATION
	public OVRHand ovrHandLeft;
	public OVRHand ovrHandRight;
	public OVRSkeleton ovrSkeletonLeft;
	public OVRSkeleton ovrSkeletonRight;
#endif
	private Wolf3DAvatar wolfAvatar;

	private Vector3 posOffsetRift = new Vector3(0.0403f, -.0254f, -.1057f);
	private readonly Vector3 rotOffsetRightRift = new Vector3(77f, 110f, 35f);
	private readonly Vector3 rotOffsetLeftRift = new Vector3(72f, -104f, -28f);

	private Vector3 posOffsetRiftSQuest = new Vector3(0.0334f, -.0341f, -.1085f);
	private readonly Vector3 rotOffsetRightRiftSQuest = new Vector3(94.8f, 6.2f, -76.8f);
	private readonly Vector3 rotOffsetLeftRiftSQuest = new Vector3(83f, -148f, -66f);

	private Vector3 posOffsetWolf3D = new Vector3(-0.000162f, -0.000425f, -6.7e-05f);
	private readonly Vector3 rotOffsetRightWolf3D = new Vector3(180f, 90f, -90f);
	private readonly Vector3 rotOffsetLeftWolf3D = new Vector3(180f, 90f, -90f);

	private Vector3 posOffsetOVRHands = new Vector3(0, 0,0);
	private readonly Vector3 rotOffsetRightOVRHands = new Vector3(180f, 0, 0);
	private readonly Vector3 rotOffsetLeftOVRHands = new Vector3(0, 0, 0);

	private GameObject leftWatch;
	private GameObject rightWatch;

	private IEnumerator Start()
	{
		float scale = 1;

		Vector3 posOffset = Vector3.zero;
		Vector3 rotOffsetLeft = Vector3.zero;
		Vector3 rotOffsetRight = Vector3.zero;

		Transform leftParent = null;
		Transform rightParent = null;

		switch (avatarType)
		{
			case AvatarType.OVRHands:
			{
#if OCULUS_INTEGRATION
				posOffset = posOffsetOVRHands;
				rotOffsetLeft = rotOffsetRightOVRHands;
				rotOffsetRight = rotOffsetLeftOVRHands;

				leftParent = ovrSkeletonLeft.transform;
				rightParent = ovrSkeletonRight.transform;
#endif
				break;
			}
			case AvatarType.Wolf3DAvatar:
				wolfAvatar = GetComponent<Wolf3DAvatar>();
				scale = .01f;

				// using Wolf3D avatar:
				posOffset = posOffsetWolf3D;
				rotOffsetLeft = rotOffsetRightWolf3D;
				rotOffsetRight = rotOffsetLeftWolf3D;

				leftParent = wolfAvatar.HandLeftPart.Transform;
				rightParent = wolfAvatar.HandRightPart.Transform;
				break;
			case AvatarType.None:
				switch (InputMan.controllerStyle)
				{
					case HeadsetControllerStyle.RiftSQuest:
					case HeadsetControllerStyle.Quest2:
						posOffset = posOffsetRiftSQuest;
						rotOffsetLeft = rotOffsetLeftRiftSQuest;
						rotOffsetRight = rotOffsetRightRiftSQuest;
						break;
					case HeadsetControllerStyle.Rift:
						posOffset = posOffsetRift;
						rotOffsetLeft = rotOffsetLeftRift;
						rotOffsetRight = rotOffsetRightRift;
						break;
				}

				leftParent = GameManager.instance.player.rig.leftHand;
				rightParent = GameManager.instance.player.rig.rightHand;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		leftWatch = Instantiate(watchPrefab, leftParent);
		posOffset.x *= -1;
		leftWatch.transform.localPosition = posOffset;
		leftWatch.transform.localEulerAngles = rotOffsetLeft;
		leftWatch.transform.localScale *= scale;
		leftWatch.GetComponent<WatchController>().side = Side.Left;

		rightWatch = Instantiate(watchPrefab, rightParent);
		rightWatch.transform.localPosition = posOffset;
		rightWatch.transform.localEulerAngles = rotOffsetRight;
		rightWatch.transform.localScale *= scale;
		rightWatch.GetComponent<WatchController>().side = Side.Right;

		yield return null;
	}

	private void Update()
	{
		#if OCULUS_INTEGRATION
		if (avatarType == AvatarType.OVRHands)
		{
			leftWatch.SetActive(GameManager.instance.player.leftTrackedHandVisibleLocal);
			rightWatch.SetActive(GameManager.instance.player.rightTrackedHandVisibleLocal);
		}
		#endif
	}
}