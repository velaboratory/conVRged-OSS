using System;
using System.Collections.Generic;
using MenuTablet;
using unityutilities;
using UnityEngine;
using Logger = unityutilities.Logger;
using UnityEngine.SceneManagement;
using unityutilities.OVR;
using unityutilities.VRInteraction;
using VelNet;

namespace conVRged
{
	public class Hand : HandBase
	{
		private Movement m;

		private Collider SnapUICollider;
		[ReadOnly] public SnapPoint hoveredSnapPoint;

		private bool grabbingObj;

		private float timeHeld;

		private ControllerHelp.ControllerLabel[] triggerHintLabels;

		private void Start()
		{
			m = GameManager.instance.player.movement;

			m.TeleportStart += delegate(Side side)
			{
				if (side == hand.side)
				{
					TeleporterShowing = true;
				}
			};
			m.TeleportEnd += delegate { TeleporterShowing = false; };

			hand.OnGrab += GrabEvent;
			hand.OnRelease += ReleaseEvent;

			m.OnGrab += GrabMoveEvent;
			m.OnRelease += ReleaseMoveEvent;

			MenuTabletMover.OnHide += (tablet) =>
			{
				// remove the collider reference (as if we just left the collider)
				if (SnapUICollider != null)
				{
					LeaveSnapUI(SnapUICollider);
				}
			};


			SwitchTool(Tool.AvatarHand);
		}

		// Update is called once per frame
		private void Update()
		{
			#region Total Station Cursor

			//uicursor update
			if (SnapUICollider)
			{
				//update pos
				UpdateUICursor(SnapUICollider);

				//send click
				if (SnapUICollider)
				{
					RaycastHit hit;

					if (Physics.Raycast(uiCursorTool.transform.position, -uiCursorTool.transform.forward, out hit,
						    0.02f))
					{
						MeshButton b = hit.collider.GetComponent<MeshButton>();
						if (b != null)
						{
							// hovered over button
							b.Select();

							if (InputMan.TriggerDown(hand.side) || Input.GetMouseButtonDown(0))
							{
								// clicked button
								b.onClick.Invoke();
								InputMan.Vibrate(hand.side, .5f);
							}
						}
					}
				}
			}

			#endregion

			#region WhiteboardMarker

			// //sound and rumble
			// if (markerTool != null)
			// {
			// 	//get vars
			// 	VRPen.MarkerInput mi = markerTool.transform.parent.GetComponent<VRPen.MarkerInput>();
			// 	VRPen.RemoteMarker rm = markerTool.transform.parent.GetComponent<VRPen.RemoteMarker>();
			// 	VRPen.InputDevice deviceData = null;
			//
			// 	//get devicedata
			// 	if (mi != null)
			// 	{
			// 		deviceData = mi.deviceData;
			// 	}
			// 	else if (rm != null)
			// 	{
			// 		deviceData = rm.deviceData;
			// 	}
			// 	else
			// 	{
			// 		Debug.LogError("No state for marker");
			// 	}
			//
			// 	if (deviceData != null)
			// 	{
			// 		//vars
			// 		Vector3 velocity = deviceData.velocity;
			// 		bool clickDown = false;
			// 		if (mi != null)
			// 		{
			// 			clickDown = !markerSnapped && mi.snappedTo != null;
			// 			markerSnapped = mi.snappedTo != null;
			// 		}
			// 		else if (rm != null)
			// 		{
			// 			clickDown = !markerSnapped && rm.snappedTo != null;
			// 			markerSnapped = rm.snappedTo != null;
			// 		}
			//
			// 		if (clickDown)
			// 		{
			// 			//set vars
			// 			float volume = Mathf.Clamp(InputMan.ControllerVelocity(hand.side).magnitude * 0.8f, 0, 1);
			// 			markerHitSound.volume = volume;
			//
			// 			//rumble
			// 			float rumble = Mathf.Clamp(volume * 1.5f, 0, 1);
			// 			InputMan.Vibrate(hand.side, rumble, 0.1f);
			//
			// 			//start sound clip
			// 			markerHitSound.Play();
			// 		}
			// 		else if (markerSnapped)
			// 		{
			// 			//set vars
			// 			float volume = Mathf.Clamp(velocity.magnitude * 15, 0, 1);
			// 			float pitch = Mathf.Clamp(velocity.x * 10 + 1.5f, 1, 2);
			// 			markerDrawSound.volume = volume;
			// 			markerDrawSound.pitch = pitch;
			//
			// 			//rumble
			// 			float rumble = Mathf.Clamp(volume - 0.02f, 0, 1);
			// 			InputMan.Vibrate(hand.side, rumble, Time.deltaTime);
			//
			// 			//start soundclip
			// 			if (!markerDrawSound.isPlaying) markerDrawSound.Play();
			// 		}
			// 		else if (markerDrawSound.isPlaying)
			// 		{
			// 			//end sound clip
			// 			markerDrawSound.Stop();
			// 		}
			// 	}
			// }

			#endregion

			#region Grab

			// grab things
			if (!SnapUICollider)
			{
				// Snap total station to tripod
				if (hand.grabbedVRGrabbable)
				{
					//if (hand.grabbedVRGrabbable.GetComponent<TotalStation>())
					//{
					//	CheckSnap(SnapPoint.Type.TotalStation);
					//}
					try
					{
						if (hand.grabbedVRGrabbable.CompareTag("train_piece"))
						{
							CheckSnap(SnapPoint.Type.TrainPiece);
						}
					}
					catch (Exception)
					{
						Debug.LogError("Tag train_piece doesn't exist. I suggest you add it or remove this code");
					}
				}

				// TODO log grab events
			}

			switch (hand.side)
			{
				case Side.Left:
					if (hand.touchedObjs.Count == 0 && hand.grabbedVRGrabbable == null)
					{
						GameManager.instance.player.movement.grabAirLeftLocks.Remove("hovering-object");
					}
					else
					{
						GameManager.instance.player.movement.grabAirLeftLocks.Add("hovering-object");
					}
					break;
				case Side.Right:
					if (hand.touchedObjs.Count == 0 && hand.grabbedVRGrabbable == null)
					{
						GameManager.instance.player.movement.grabAirRightLocks.Remove("hovering-object");
					}
					else
					{
						GameManager.instance.player.movement.grabAirRightLocks.Add("hovering-object");
					}
					break;
			}

			#endregion

			#region Laser

			// only do laser things if not about to grab something instead

			// if we are hovering over a snap collider or a local object
			if (SnapUICollider || hand.touchedObjs.Count > 0)
			{
				ShowLASER(false);
				if (triggerHintLabels != null)
				{
					ControllerHelp.HideHint(triggerHintLabels);
					triggerHintLabels = null;
				}
			}
			// if we are pointing at a distance grabbable object
			else if (hand.remoteTouchedObjs.Count > 0 && hand.selectedVRGrabbable != null)
			{
				ShowLASER(true);
				ResizeLaser(hand.selectedVRGrabbable.transform.position, 1);
				triggerHintLabels ??= ControllerHelp.ShowHint(hand.side, ControllerHelp.ButtonHintType.Trigger, "");
			}
			// if we are not hovering over anything
			else
			{
				ShowLASER(worldMouseWithLaser.currentRayLength < Mathf.Infinity &&
				          worldMouseWithLaser.currentRayLength != 0);
				ResizeLaser(worldMouseWithLaser.currentRayLength, InputMan.TriggerValue(hand.side));
				if (triggerHintLabels != null)
				{
					ControllerHelp.HideHint(triggerHintLabels);
					triggerHintLabels = null;
				}
			}

			#endregion

			#region MenuTablet Button Activation

			// oculus hand tracking system gesture does not work
			if (InputMan.MenuButtonDown(hand.side))
			{
				MenuTabletMover.ToggleTablet(transform, hand.side);
				InputMan.Vibrate(hand.side, .5f);
			}

			if (InputMan.MenuButtonUp(hand.side))
			{
				MenuTabletMover.DetachTablet();
				InputMan.Vibrate(hand.side, .2f);
			}

			#endregion

			timeHeld += Time.deltaTime;
		}

		private void CheckSnap(SnapPoint.Type type)
		{
			Collider[] colliders = Physics.OverlapSphere(hand.grabbedVRGrabbable.transform.position, .1f);
			bool found = false;
			foreach (Collider c in colliders)
			{
				SnapPoint s = c.GetComponent<SnapPoint>();
				if (s && s.pointType == type)
				{
					found = true;
					hoveredSnapPoint = s;
					hoveredSnapPoint.ghost?.SetActive(true);
				}
			}

			if (!found)
			{
				hoveredSnapPoint = null;
				if (hoveredSnapPoint) hoveredSnapPoint.ghost?.SetActive(false);
			}
		}

		void OnSceneChange(Scene oldScene, Scene newScene)
		{
			SwitchTool(Tool.AvatarHand);
		}

		private void GrabMoveEvent(Transform parent, Side side)
		{
			if (side == hand.side)
			{
				hand.grabLocks.Add("grabmove");
			}
		}

		private void ReleaseMoveEvent(Transform heldObject, Side side, Vector3 localDisplacement,
			Vector3 worldDisplacement, Vector3 releaseVel, float duration)
		{
			if (side == hand.side)
			{
				hand.grabLocks.Remove("grabmove");
			}
		}

		private void GrabEvent(VRGrabbable grabbable)
		{
			// velnet 🌐
			NetworkComponent component = grabbable.GetComponent<NetworkComponent>();
			if (component == null)
			{
				component = grabbable.GetComponentInParent<NetworkComponent>();
			}
			if (component != null)
			{
				component.networkObject.TakeOwnership();
			}

			LogGrabEvent(grabbable.transform);

			// release grab move with the same side hand
			m.Release(hand.side);

			if (grabAudio != null) grabAudio.Play();
		}

		private void ReleaseEvent(VRGrabbable grabbable)
		{
			if (!grabbable) return;

			bool snap = false;

			try
			{
				if (grabbable.CompareTag("train_piece"))
				{
					snap = SnapTo(grabbable.transform, hoveredSnapPoint);

					// TODO Network the snap, like above
				}
			}
			catch (Exception)
			{
				Debug.LogError("Tag train_piece doesn't exist. I suggest you add it or remove this code");
			}


			//// give the object back to the scene
			//PhotonView netObj = grabbable.GetComponent<PhotonView>();
			//if (netObj && netObj.IsMine)
			//{
			//	netObj.TransferOwnership(0);
			//}

			GameManager.instance.sceneSaver.Save(true);

			LogReleaseEvent(grabbable.transform, snap);

			if (releaseAudio != null) releaseAudio.Play();
		}

		public static bool SnapTo(Transform snapObj, SnapPoint snapParent)
		{
			if (snapParent != null)
			{
				snapObj.transform.SetParent(snapParent.transform);
				snapParent.snapped = true;
				snapParent.currentObject = snapObj;
				snapParent.ghost.SetActive(false);
				snapObj.transform.localPosition = Vector3.zero;
				snapObj.transform.localRotation = Quaternion.identity;
				//snapObj.GetComponent<Rigidbody>().isKinematic = true;
				return true;
			}

			snapObj.transform.SetParent(null);
			//snapObj.GetComponent<Rigidbody>().isKinematic = false;
			return false;
		}

		public void LogGrabEvent(Transform grabbedObj)
		{
			timeHeld = 0;

			Logger.LogRow("grab_events", new StringList(new List<dynamic>()
			{
				grabbedObj.name,
				grabbedObj.GetComponent<NetworkObject>() != null
					? grabbedObj.GetComponent<NetworkObject>().networkId
					: "-1",
				hand.side,
				"grab",
				grabbedObj.transform.position
			}).List);
		}

		public void LogReleaseEvent(Transform grabbedObj, bool snap)
		{
			Logger.LogRow("grab_events", new StringList(new List<dynamic>()
			{
				grabbedObj.name,
				grabbedObj.GetComponent<NetworkObject>() != null
					? grabbedObj.GetComponent<NetworkObject>().networkId
					: "-1",
				hand.side,
				"release",
				transform.position,
				timeHeld,
				snap ? "snap" : "",
			}).List);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("SnapUI") && !grabbingObj)
			{
				hand.grabLocks.Add("snapui");

				SnapUICollider = other;
				NetworkObject netObj = other.GetComponentInParent<NetworkObject>();
				Logger.LogRow("grab_events", new List<string>
				{
					"snap_ui",
					netObj != null ? netObj.networkId : "-1",
					hand.side.ToString(),
					"enter",
					"0",
					"0",
					"0"
				});

				InputMan.Vibrate(hand.side, .2f);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			// VRPen.Tag tag;
			// if ((tag = other.gameObject.GetComponent<VRPen.Tag>()) != null)
			// {
			// 	if (tag.tag.Equals("marker-visible")) SwitchTool(Tool.AvatarHand);
			// }

			if (other.gameObject.CompareTag("SnapUI"))
			{
				LeaveSnapUI(other);
			}
		}

		private void LeaveSnapUI(Collider other)
		{
			hand.canGrab = true;
			hand.grabLocks.Remove("snapui");

			SnapUICollider = null;
			SwitchTool(Tool.AvatarHand);
			NetworkObject netObj = other.GetComponentInParent<NetworkObject>();
			string uniqueName;
			if (netObj != null)
			{
				uniqueName = netObj.networkId;
			}
			else
			{
				uniqueName = other.transform.root.name;
			}

			Logger.LogRow("grab_events", new List<string>
			{
				"snap_ui",
				uniqueName,
				hand.side.ToString(),
				"exit",
				"0",
				"0",
				"0"
			});
		}

		/// <summary>
		/// Updates the UICursor (used on total station buttons)
		/// </summary>
		/// <param name="col">The collider that caused this to be enabled. Used for orientation</param>
		/// <param name="visible">Whether to enable or disable</param>
		private void UpdateUICursor(Collider col)
		{
			SwitchTool(Tool.UICursor);

			//position setting
			Vector3 colliderPos = transform.TransformPoint(GetComponent<SphereCollider>().center);
			float distance = Vector3.Dot(colliderPos - col.transform.position, col.transform.up);
			uiCursorTool.transform.position = colliderPos - col.transform.up * distance;
			uiCursorTool.transform.forward = col.transform.up;
			uiCursorTool.transform.Rotate(0, 0, 45);
		}

		public void SetupCopyTransform(Transform target)
		{
			CopyTransform ct;
			if (target.GetComponent<CopyTransform>())
			{
				ct = target.GetComponent<CopyTransform>();
			}
			else
			{
				ct = target.gameObject.AddComponent<CopyTransform>();
				ct.positionFollowType = CopyTransform.FollowType.Copy;
				ct.rotationFollowType = CopyTransform.FollowType.Copy;
				ct.snapIfAngleGreaterThan = 90;
				ct.snapIfDistanceGreaterThan = 1f;
				ct.followPosition = true;
				ct.followRotation = true;
				ct.positionOffsetCoordinateSystem = Space.Self;
				ct.rotationOffsetCoordinateSystem = Space.Self;
			}

			ct.SetTarget(transform);
		}

		public void SetupCopyTransform(Transform target, Vector3 posOffsetLocal, Quaternion rotOffsetLocal)
		{
			SetupCopyTransform(target);
			CopyTransform ct;
			if (gameObject.GetComponent<CopyTransform>())
			{
				ct = gameObject.GetComponent<CopyTransform>();
			}
			else
			{
				ct = gameObject.AddComponent<CopyTransform>();
				ct.positionOffset = posOffsetLocal;
				ct.rotationOffset = rotOffsetLocal;
			}
		}
	}
}