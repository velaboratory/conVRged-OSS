using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unityutilities.Interaction.WorldMouse;

namespace unityutilities.OVR
{
	public class FingerTouchHandOculusHandTracking : MonoBehaviour
	{
		public GameObject fingerInteractorPrefab;


		private readonly List<GameObject> spawnedPrefabs = new List<GameObject>();

		public Vector3 direction = Vector3.up;
		public Vector3 offset = Vector3.up * .01f;

		private void OnEnable()
		{
			StopAllCoroutines();
			StartCoroutine(FindFingerTips());
		}

		private void OnDisable()
		{
			DestroyFingerTips();
			WorldMouseInputModule.FindCanvases();
		}

		public IEnumerator FindFingerTips()
		{
			if (fingerInteractorPrefab == null) yield break;

			DestroyFingerTips();

			for (int i = 0; i < 2; i++)
			{
				Side side = (Side)i;
				OVRSkeleton skele = InputManOVRHands.GetSkeleton(side);
				while (skele.Bones == null || skele.Bones.Count == 0)
				{
					yield return null;
				}

				Transform bone = skele.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform;
				AddPrefab(side, bone);
			}

			WorldMouseInputModule.FindCanvases();
		}

		private void DestroyFingerTips()
		{
			foreach (GameObject obj in spawnedPrefabs)
			{
				Destroy(obj);
			}

			spawnedPrefabs.Clear();
		}

		private void AddPrefab(Side side, params Transform[] bones)
		{
			foreach (Transform t in bones)
			{
				GameObject obj = Instantiate(fingerInteractorPrefab, Vector3.zero, Quaternion.identity, t);
				obj.transform.localPosition = side == Side.Left ? offset : -offset;
				obj.transform.forward = t.TransformDirection(side == Side.Left ? direction : -direction);
				FingerWorldMouse wm = obj.GetComponent<FingerWorldMouse>();
				if (wm != null)
				{
					wm.side = side;
				}

				spawnedPrefabs.Add(obj);
			}
		}
	}
}