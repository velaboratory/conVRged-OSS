using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using unityutilities;
using Logger = unityutilities.Logger;
using Random = UnityEngine.Random;

namespace conVRged
{
	public class VELPlayer : MonoBehaviour
	{
		public GameManager.PlayerPrefabType prefabType;

		public Rig rig => movement.rig;
		public Movement movement;
		public OVRHandLocomotion handLoco;
		public Hand leftHand;
		public Hand rightHand;
		public TrackedHand leftTrackedHand;
		public TrackedHand rightTrackedHand;

		public OVRSkeleton leftOVRSkeleton;
		public OVRSkeleton rightOVRSkeleton;

		public OVRHand leftOVRHand;
		public OVRHand rightOVRHand;

		public GameObject controllerModelLeft;
		public GameObject controllerModelRight;

		/// <summary>
		/// Used as a switch between tracked hands and controller
		/// </summary>
		public bool trackedHandsVisible;

		public bool leftTrackedHandVisibleLocal = true;
		public bool rightTrackedHandVisibleLocal = true;

		[Header("Wolf3D Hand Anchors")] public Transform leftHandAvatarAnchor;
		public Transform rightHandAvatarAnchor;
		public Transform armLengthObj;

		public float ArmLength
		{
			get => armLengthObj.localPosition.z;
			set
			{
				Vector3 pos = armLengthObj.localPosition;
				pos.z = value;
				armLengthObj.localPosition = pos;
			}
		}

		public Vector3 leftHandPosition;
		public Vector3 rightHandPosition;

		private void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneManagerOnSceneLoaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneManagerOnSceneLoaded;
		}

		private void OnSceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Additive) return;
			TeleportToStartPositions();
			StartCoroutine(SetTooltipsOnSceneJoin(scene.name));
		}


		// Update is called once per frame
		private void Update()
		{
			if (prefabType == GameManager.PlayerPrefabType.Oculus)
			{
				trackedHandsVisible = leftOVRHand.IsTracked || rightOVRHand.IsTracked;
				leftTrackedHandVisibleLocal = leftOVRHand.IsTracked;
				rightTrackedHandVisibleLocal = rightOVRHand.IsTracked;

				leftHand.gameObject.SetActive(!leftTrackedHandVisibleLocal);
				rightHand.gameObject.SetActive(!rightTrackedHandVisibleLocal);


				leftHandPosition = trackedHandsVisible ? leftTrackedHand.avgPos : leftHand.transform.position;
				rightHandPosition = trackedHandsVisible ? rightTrackedHand.avgPos : rightHand.transform.position;

				// TODO add this back for non-autohand
				// not working atm
				// bool holdingLeft = leftHand.hand.grabbedVRGrabbable != null;
				// movement.grabAirLeft = !holdingLeft;
				// controllerModelLeft.SetActive(!holdingLeft);
				//
				// bool holdingRight = rightHand.hand.grabbedVRGrabbable != null;
				// movement.grabAirRight = !holdingRight;
				// controllerModelRight.SetActive(!holdingRight);
			}
			else
			{
				leftHandPosition = leftHand.transform.position;
				rightHandPosition = rightHand.transform.position;
			}

			// reset the player position if the player goes off into space
			if (transform.position.sqrMagnitude > 100000)
			{
				Logger.LogRow("events", "reset-position", "too far away");
				TeleportToStartPositions();
			}
		}

		/// <summary>
		/// Teleports to a random object with the spawn_location tag
		/// </summary>
		private void TeleportToStartPositions()
		{
			Vector3 beforePos = transform.position;
			
			// find spawn locations
			GameObject[] playerStartPositions = GameObject.FindGameObjectsWithTag("spawn_location");
			// if the list is initialized
			if (playerStartPositions != null && playerStartPositions.Length > 0)
			{
				// find a random point in the array
				int startPos = Random.Range(0, playerStartPositions.Length);

				// teleport to it
				GameManager.instance.player.movement.TeleportTo(playerStartPositions[startPos].transform.position, playerStartPositions[startPos].transform.rotation, true);
			}
			// else go to the default pos
			else
			{
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
			
			Vector3 afterPos = transform.position;
			Logger.LogRow("events", "teleport-to-spawn",
				beforePos.x.ToString(CultureInfo.InvariantCulture),
				beforePos.y.ToString(CultureInfo.InvariantCulture),
				beforePos.z.ToString(CultureInfo.InvariantCulture),
				afterPos.x.ToString(CultureInfo.InvariantCulture),
				afterPos.y.ToString(CultureInfo.InvariantCulture),
				afterPos.z.ToString(CultureInfo.InvariantCulture)
			);
		}

		private IEnumerator SetTooltipsOnSceneJoin(string sceneName)
		{
			if (sceneName != "TutorialWorld" &&
			    sceneName != "TutorialRoom")
			{
				if (GameManager.instance.lastLaunchedVersion != Application.version)
				{
					yield return new WaitForSeconds(2);
					ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Button2, "Press B/Y to open menu");
					yield return new WaitForSeconds(7);
					ControllerHelp.HideAllHints();
				}
			}
			else
			{
				ControllerHelp.HideAllHints();
			}
		}
	}
}