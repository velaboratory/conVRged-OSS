using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VelNet;
#if VUPLEX
using Vuplex.WebView;
#endif

// this task takes in a prefab and number to place & find.  The task is over when all objects have been found.
// for now, monitoring the task is the responsibility of the player
namespace LocomotionStudy
{
	public class LocomotionStudyManager : SyncState
	{
		public Action<BoxObject> OnObjectGrabbed;
		public Action<BoxObject> OnObjectFound;
		public int numObjects;
		public int numBoxes;
		public List<Box> boxes = new List<Box>();
		public List<BoxObject> objectsToFind = new List<BoxObject>();
		public Action<float> OnTaskCompleted;
		public float timeTaken;
		public float minHeight = 1f;
		public float maxHeight = 2f;

		public GameObject spawnSurface;
		public float minSeparation = 1f;
		public Box locomotionStudyBoxPrefab;
		public BoxObject locomotionStudyBoxObjectPrefab;
#if VUPLEX
		public TVScreen tv;
		public CanvasWebViewPrefab surveyWebView;
#endif
		public Slider completionSlider;
		public TMP_Text completionText;
		public AudioSource boxTaskSuccessSound;

		private IEnumerator Start()
		{
			while (true)
			{
#if VUPLEX
				if (surveyWebView.WebView != null)
				{
					surveyWebView.WebView.ExecuteJavaScript($"document.getElementById('QR~QID2').value = {VelNetMan.NickName};");
					surveyWebView.WebView.ExecuteJavaScript($"document.getElementById('QR~QID26').value = {VelNetManager.LocalPlayer?.userid};");
				}
#endif

				yield return new WaitForSeconds(.5f);
			}
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				networkObject.TakeOwnership();
				StartTask(1);
			}

			if (Input.GetKeyDown(KeyCode.F2))
			{
				networkObject.TakeOwnership();
				StartTask(2);
				// EndTask();
			}

#if VUPLEX
			if (Input.GetKeyDown(KeyCode.F3))
			{
				tv.SetUrl("https://docs.google.com/presentation/d/1c6xeOyKObBYLsacoiPXgYmSARGAD_gId4wldLcocZjs/present#slide=id.p");
			}

			if (Input.GetKeyDown(KeyCode.F4))
			{
				tv.SetUrl("https://docs.google.com/presentation/d/1c6xeOyKObBYLsacoiPXgYmSARGAD_gId4wldLcocZjs/present#slide=id.g1fa3065e52a_0_164");
			}
#endif

			if (networkObject.IsMine)
			{
				timeTaken += Time.deltaTime;
			}

			completionSlider.maxValue = boxes.Count;

			if (completionSlider.value < completionSlider.maxValue && objectsToFind.Count == 0)
			{
				boxTaskSuccessSound.Play();
			}

			completionSlider.value = boxes.Count - objectsToFind.Count;
			completionText.text = $"{boxes.Count - objectsToFind.Count}/{boxes.Count}";
		}

		public void StartTask(int randomSeed = -1)
		{
			EndTask();

			boxes = new List<Box>();
			objectsToFind = new List<BoxObject>();

			timeTaken = 0f;
			gameObject.SetActive(true);

			if (randomSeed > 0)
			{
				UnityEngine.Random.InitState(randomSeed);
			}

			Vector3 localScale = spawnSurface.transform.localScale;
			int numItemsX = (int)(localScale.x / minSeparation);
			int numItemsZ = (int)(localScale.y / minSeparation);
			List<int> spots = Enumerable.Range(0, numItemsX * numItemsZ).ToList();

			for (int i = 0; i < numBoxes; i++)
			{
				// choose one of the available spots
				int spotIndex = UnityEngine.Random.Range(0, spots.Count);
				// one of 8 directions in 45 degree increments
				int rot = UnityEngine.Random.Range(0, 8) * 45;

				int spot = spots[spotIndex];
				spots.RemoveAt(spotIndex);
				Vector3 spawnSurfacePosition = spawnSurface.transform.position;
				float gridX = (spot / numItemsX) * minSeparation - localScale.x / 2f + spawnSurfacePosition.x;
				float gridZ = (spot % numItemsZ) * minSeparation - localScale.y / 2f + spawnSurfacePosition.z;
				Vector3 pos = new Vector3(gridX, UnityEngine.Random.Range(minHeight, maxHeight), gridZ);
				Box b = VelNetManager.NetworkInstantiate(locomotionStudyBoxPrefab.name, pos, Quaternion.Euler(0, rot, 0)).GetComponent<Box>();

				// disabling and re-enabling the object is necessary for the hinge joint to work for some reason
				// b.gameObject.SetActive(false);
				// b.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, rot, 0));
				// b.gameObject.SetActive(true);
				boxes.Add(b);
			}


			List<Box> unusedBoxes = new List<Box>(boxes);
			int totalUsed = 0;
			while (totalUsed < numObjects)
			{
				int box = UnityEngine.Random.Range(0, unusedBoxes.Count);
				BoxObject bo = VelNetManager.NetworkInstantiate(locomotionStudyBoxObjectPrefab.name, unusedBoxes[box].transform.position, Quaternion.identity).GetComponent<BoxObject>();
				bo.id = totalUsed;
				bo.grabbable.Grabbed += () => { OnObjectGrabbed?.Invoke(bo); };

				bo.OnFound += () =>
				{
					if (objectsToFind.Contains(bo))
					{
						OnObjectFound?.Invoke(bo);
						objectsToFind.Remove(bo);
						if (objectsToFind.Count == 0)
						{
							OnTaskCompleted?.Invoke(timeTaken);
						}
					}
					else
					{
						Debug.LogError("Box Object found but not in the list anymore", bo);
					}
				};
				objectsToFind.Add(bo);
				unusedBoxes.Remove(unusedBoxes[box]);
				totalUsed++;
			}
		}


		public void EndTask()
		{
			// destroy the boxes
			foreach (Box b in boxes.ToArray())
			{
				if (b != null)
				{
					b.GetComponent<NetworkObject>().TakeOwnership();
					VelNetManager.NetworkDestroy(b.GetComponent<NetworkObject>());
				}
			}

			foreach (BoxObject go in objectsToFind.ToArray())
			{
				if (go != null)
				{
					go.networkObject.TakeOwnership();
					VelNetManager.NetworkDestroy(go.networkObject);
				}
			}
		}

		protected override void SendState(BinaryWriter binaryWriter)
		{
		}

		protected override void ReceiveState(BinaryReader binaryReader)
		{
		}
	}
}