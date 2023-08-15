using System.Collections.Generic;
using UnityEngine;
using unityutilities.Interaction.WorldMouse;

namespace unityutilities
{
	public class FingerTouchHandRPMe : MonoBehaviour
	{
		public GameObject fingerInteractorPrefab;

		public string[] leftHandFingers =
		{
			"Armature/Hips/Spine/LeftHand/LeftHandIndex1/LeftHandIndex2/LeftHandIndex3",
		};

		public string[] rightHandFingers =
		{
			"Armature/Hips/Spine/RightHand/RightHandIndex1/RightHandIndex2/RightHandIndex3",
		};

		private readonly List<GameObject> spawnedPrefabs = new List<GameObject>();

		public Vector3 direction = Vector3.up;
		public Vector3 offset = Vector3.up * .01f;

		// TODO right now finger tips are created then destroyed for every remote avatar
		// this could be improved
		private void OnEnable()
		{
			FindFingerTips();
		}

		private void OnDisable()
		{
			DestroyFingerTips();
			WorldMouseInputModule.FindCanvases();
		}

		public void FindFingerTips()
		{
			if (fingerInteractorPrefab == null) return;
			
			DestroyFingerTips();

			AddPrefab(leftHandFingers, Side.Left);
			AddPrefab(rightHandFingers, Side.Right);

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

		private void AddPrefab(string[] paths, Side side)
		{
			foreach (string f in paths)
			{
				Transform tip = transform.Find(f);
				GameObject obj = Instantiate(fingerInteractorPrefab, Vector3.zero, Quaternion.identity, tip);
				obj.transform.localPosition = offset;
				obj.transform.forward = tip.TransformDirection(direction);
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