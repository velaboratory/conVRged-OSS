using System;
using System.Collections.Generic;
using System.Linq;
using conVRged;
using MenuTablet;
using UnityEngine;
using UnityEngine.Serialization;
using unityutilities;
using unityutilities.OVR;
using unityutilities.VRInteraction;
using VelNet;
using Logger = unityutilities.Logger;
using unityutilities.Interaction.WorldMouse;

public class TrackedHand : MonoBehaviour
{
	public Rig rig;
	public OVRHandLocomotion Locomotion;

	public VRGrabbableHand hand;

	private float timeHeld;

	public GameObject grabPos;
	public Vector3 teleportSlingshotOrigin;

	public float pinchHoldDuration;


	// tuple is (Rig pos, Hand pos)
	private Queue<(Vector3, Vector3)> _lastHandPositions = new Queue<(Vector3, Vector3)>();
	public float smoothingTime = .5f;

	private Vector3 rigPosSmoothed;
	private Vector3 handPosSmoothed;

	public enum PinchMode
	{
		None,
		GrabbingObject,
		GrabMove,
		SnapTurnLeft,
		SnapTurnRight,
		Teleport
	}

	private PinchMode _currentPinchMode;

	public PinchMode CurrentPinchMode
	{
		get => _currentPinchMode;
		set
		{
			if (value != _currentPinchMode)
			{
				visualizationSphere.GetComponent<Renderer>().material = value switch
				{
					PinchMode.GrabMove => grabMoveColor,
					PinchMode.SnapTurnLeft => snapTurnColor,
					PinchMode.SnapTurnRight => snapTurnColor,
					PinchMode.Teleport => teleportColor,
					PinchMode.None => grabMoveColor,
					PinchMode.GrabbingObject => grabMoveColor,
					_ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
				};

				switch (value)
				{
					case PinchMode.GrabMove:
					case PinchMode.SnapTurnLeft:
					case PinchMode.SnapTurnRight:
					case PinchMode.Teleport:
						visualizationSphere.GetComponent<Renderer>().enabled = true;
						visualizationSphere.transform.localScale = Vector3.one * .05f;
						break;
					case PinchMode.None:
					case PinchMode.GrabbingObject:
						visualizationSphere.GetComponent<Renderer>().enabled = false;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(value), value, null);
				}
			}

			_currentPinchMode = value;
		}
	}

	public GameObject visualizationSphere;
	public Material grabMoveColor;
	public Material snapTurnColor;
	public Material teleportColor;

	public bool currentlyTeleporting;
	public bool currentlyGrabMoving;

	public Vector3 avgPos = Vector3.zero;
	public Vector3 avgOffset = Vector3.zero;
	public Vector3 deltaPos = Vector3.zero;
	public Vector3 initialPinchPos = Vector3.zero;
	public float teleportDragDistance = .5f;
	public Movement.Teleporter teleporter;
	public GameObject teleporterRegion;
	public GameObject snapTurnLeftRegion;
	public GameObject snapTurnRightRegion;

	private void Start()
	{
		hand.OnGrab += GrabEvent;
		hand.OnRelease += ReleaseEvent;

		CurrentPinchMode = PinchMode.GrabbingObject;
		CurrentPinchMode = PinchMode.None;
		teleporter = new Movement.Teleporter(Locomotion.m.teleporter);
	}

	private void Update()
	{
		if (!InputManOVRHands.GetHand(hand.side).IsDataHighConfidence ||
		    InputManOVRHands.GetHand(hand.side).IsSystemGestureInProgress)
		{
			CurrentPinchMode = PinchMode.None;
			return;
		}

		Vector3 pinchPos = InputManOVRHands.GetPinchPos(hand.side);

		hand.transform.position = pinchPos;

		if (InputManOVRHands.PinchDown(hand.side))
		{
			initialPinchPos = pinchPos;
			pinchHoldDuration = 0;
		}

		#region MenuTablet Button Activation

		// oculus hand tracking system gesture does not work
		if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LHand)
		    // || InputManOVRHands.MiddleFingerPinchDown(hand.side)
		   )
		{
			// MenuTabletMover.ToggleTablet(transform, hand.side);
			MenuTabletMover.ToggleTablet();
			WorldMouseInputModule.FindCanvases();
		}

		if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.LHand)
		    // || InputManOVRHands.MiddleFingerPinchUp(hand.side)
		   )
		{
			MenuTabletMover.DetachTablet();
		}

		#endregion

		#region Grabbing Objects

		// switch (hand.side)
		// {
		// 	case Side.Left:
		// 		if (hand.touchedObjs.Count == 0 && hand.grabbedVRGrabbable == null)
		// 		{
		// 			GameManager.instance.player.movement.grabAirLeftLocks.Remove("hovering-object");
		// 		}
		// 		else
		// 		{
		// 			GameManager.instance.player.movement.grabAirLeftLocks.Add("hovering-object");
		// 		}
		//
		// 		break;
		// 	case Side.Right:
		// 		if (hand.touchedObjs.Count == 0 && hand.grabbedVRGrabbable == null)
		// 		{
		// 			GameManager.instance.player.movement.grabAirRightLocks.Remove("hovering-object");
		// 		}
		// 		else
		// 		{
		// 			GameManager.instance.player.movement.grabAirRightLocks.Add("hovering-object");
		// 		}
		//
		// 		break;
		// }

		if (InputManOVRHands.PinchDown(hand.side))
		{
			hand.Grab();
		}

		if (InputManOVRHands.PinchUp(hand.side))
		{
			hand.Release();
		}

		#endregion


		#region Locomotion

		Vector3 newPos = InputManOVRHands.GetPinchPos(hand.side);

		// Need to investigate using this method instead of window
		// float smoothing = .1f;
		// handPosSmoothed = handPosSmoothed * (1 - smoothing) + newPos * smoothing;
		// rigPosSmoothed = rigPosSmoothed * (1 - smoothing) + rig.rb.position * smoothing;


		float handDistImmediate = Vector3.Distance(rig.head.position, newPos);
		smoothingTime = Mathf.Lerp(1f, .1f, handDistImmediate / .3f);

		_lastHandPositions.Enqueue((rig.rb.position, newPos));
		avgPos = _lastHandPositions.Aggregate(Vector3.zero, (s, v) => s + v.Item2) / _lastHandPositions.Count;
		avgOffset = _lastHandPositions.Aggregate(Vector3.zero, (s, v) => s + (v.Item1 - v.Item2)) /
		            _lastHandPositions.Count;

		while (_lastHandPositions.Count > Math.Clamp((int)(smoothingTime / Time.smoothDeltaTime), 2, 100))
		{
			_lastHandPositions.Dequeue();
		}


		List<(Vector3, Vector3)> list = _lastHandPositions.ToList();
		if (list.Count > 1)
		{
			deltaPos = (list[^1].Item1 - list[^1].Item2) - (list[^2].Item1 - list[^2].Item2);
		}


		float yHeight = (Vector3.Project(avgPos, rig.transform.up).y -
		                 Vector3.Project(rig.head.position, rig.transform.up).y);
		float distanceFromHead = Vector3.Distance(avgPos, rig.head.position);
		// the angle of the hand away from the forward direction
		float angle = Vector3.SignedAngle(rig.head.forward, avgPos - rig.head.position, rig.head.up);

		if (currentlyGrabMoving || currentlyTeleporting)
		{
			// if (Vector3.Distance(initialPinchPos, avgPos) > teleportDragDistance)
			// {
			// 	currentlyGrabMoving = false;
			// 	CurrentPinchMode = PinchMode.Teleport;
			// 	currentlyTeleporting = true;
			// }
		}
		else if (hand.selectedVRGrabbable != null || hand.grabbedVRGrabbable != null)
		{
			CurrentPinchMode = PinchMode.GrabbingObject;
		}
		// else if (distanceFromHead > Locomotion.TeleportBeyondReach && yHeight > Locomotion.teleportBeyondReachHeight)
		else if (Vector3.Distance(pinchPos, teleporterRegion.transform.position) < teleporterRegion.transform.localScale.x / 2f)
		{
			CurrentPinchMode = PinchMode.Teleport;
		}
		// else if (yHeight > Locomotion.snapTurnAbove && Mathf.Abs(angle) > Locomotion.snapTurnAngleAwayFromForward)
		// {
		// 	CurrentPinchMode = angle > 0 ? PinchMode.SnapTurnLeft : PinchMode.SnapTurnRight;
		// }
		else if (Vector3.Distance(pinchPos, snapTurnLeftRegion.transform.position) < snapTurnLeftRegion.transform.localScale.x / 2f)
		{
			CurrentPinchMode = PinchMode.SnapTurnLeft;
		}
		else if (Vector3.Distance(pinchPos, snapTurnRightRegion.transform.position) < snapTurnRightRegion.transform.localScale.x / 2f)
		{
			CurrentPinchMode = PinchMode.SnapTurnRight;
		}
		// else if (yHeight > Locomotion.grabMoveAbove)
		else
		{
			CurrentPinchMode = PinchMode.GrabMove;
		}
		// else
		// {
		// 	CurrentPinchMode = PinchMode.None;
		// }

		if (CurrentPinchMode == PinchMode.GrabMove)
		{
			visualizationSphere.transform.localScale = Vector3.one * ((Mathf.Clamp01(InputManOVRHands.PinchValue(hand.side) + .2f)) / 20);
		}

		visualizationSphere.transform.position = avgPos;

		#endregion

		pinchHoldDuration += Time.deltaTime;
		timeHeld += Time.deltaTime;
	}

	/// <summary>
	/// TODO use the version in Hand.cs
	/// </summary>
	private void GrabEvent(VRGrabbable grabbable)
	{
		// velnet 🌐
		NetworkObject velNetObject = grabbable.GetComponent<NetworkObject>();
		if (velNetObject != null)
		{
			velNetObject.TakeOwnership();
		}
		else
		{
			// or if the actual grabbed obj is just a component of a larger network object,
			// take ownership of the whole object
			NetworkComponent component = grabbable.GetComponent<NetworkComponent>();
			component.networkObject.TakeOwnership();
		}

		LogGrabEvent(grabbable.transform);
	}

	/// <summary>
	/// TODO use the version in Hand.cs
	/// </summary>
	private void ReleaseEvent(VRGrabbable grabbable)
	{
		if (!grabbable) return;

		bool snap = false;

		LogReleaseEvent(grabbable.transform, snap);
	}

	/// <summary>
	/// TODO use the version in Hand.cs
	/// </summary>
	public void LogGrabEvent(Transform grabbedObj)
	{
		timeHeld = 0;

		List<string> data = new List<string>
		{
			grabbedObj.name,
			grabbedObj.GetComponent<NetworkObject>() != null
				? grabbedObj.GetComponent<NetworkObject>().networkId
				: "-1",
			hand.side.ToString(),
			"grab",
			grabbedObj.transform.position.x.ToString(),
			grabbedObj.transform.position.y.ToString(),
			grabbedObj.transform.position.z.ToString()
		};
		Logger.LogRow("grab_events", data);
	}

	/// <summary>
	/// TODO use the version in Hand.cs
	/// </summary>
	public void LogReleaseEvent(Transform grabbedObj, bool snap)
	{
		List<string> data = new List<string>
		{
			grabbedObj.name,
			grabbedObj.GetComponent<NetworkObject>() != null
				? grabbedObj.GetComponent<NetworkObject>().networkId
				: "-1",
			hand.side.ToString(),
			"release",
			transform.position.x.ToString(),
			transform.position.y.ToString(),
			transform.position.z.ToString(),
			timeHeld.ToString()
		};
		if (snap)
		{
			data.Add("snap");
		}

		Logger.LogRow("grab_events", data);
	}
}