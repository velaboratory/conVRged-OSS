using System;
using System.Collections;
using UnityEngine;

namespace unityutilities
{
	public class AddToolsToFingersWolf3D : MonoBehaviour
	{
		[Serializable]
		public class FingerTool
		{

			public FingerTool()
			{
				finger = Wolf3DFinger.Index;
				leftHand = true;
				rightHand = true;
				avatarHands = true;
				trackedHands = true;
			}

			public Transform toolPrefab;

			public Wolf3DFinger finger;
			[Space]
			public bool leftHand;
			public bool rightHand;
			[Space]
			public bool avatarHands;
			public bool trackedHands;
		}

		public enum Wolf3DFinger
		{
			Index = 0,
			Middle = 1,
			Ring = 2,
			Pinky = 3,
			Thumb = 4,
		}
		public Rig rig;

		[ReadOnly, SerializeField]
		private Wolf3DAvatar avatar;

		public FingerTool[] tools;

		private void Start()
		{
			if (tools != null && tools.Length > 0)
			{
				StartCoroutine(AttachToolsToHands(tools, true));
				StartCoroutine(AttachToolsToHands(tools, false));
			}
		}


		private IEnumerator AttachToolsToHands(FingerTool[] toolObjects, bool tracked)
		{
			yield return null;

			foreach (FingerTool tool in toolObjects)
			{
				// skip this tool if it doesn't have a prefab
				if (tool.toolPrefab == null) continue;

				// twice for left and right hands
				for (int i = 0; i < 2; i++)
				{
					bool isRight = (i == 0);
					if ((isRight && tool.rightHand) || (!isRight && tool.leftHand))
					{
						if (!tracked && tool.avatarHands)
						{
							if (isRight)
							{
								var obj = Instantiate(tool.toolPrefab, transform.Find("RightHandIndex3_end"));
								if (obj.GetComponent<TouchMenuFingerColliderWolf3D>())
								{
									obj.GetComponent<TouchMenuFingerColliderWolf3D>().isLeft = !isRight;
								}
							}
							else
							{
								var obj = Instantiate(tool.toolPrefab, transform.Find("LeftHandIndex3_end"));
								if (obj.GetComponent<TouchMenuFingerColliderWolf3D>())
								{
									obj.GetComponent<TouchMenuFingerColliderWolf3D>().isLeft = !isRight;
								}
							}
						}
					}
				}
			}
		}
	}
}
