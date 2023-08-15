using System;
using System.Collections.Generic;
using UnityEngine;
using unityutilities;
using unityutilities.OVR;

public class OVRHandLocomotion : MonoBehaviour
{
	public Rig rig;

	// left and right hands
	public TrackedHand[] hands;
	private Side _grabbingSide = Side.None;
	private Side _teleportingSide = Side.None;
	private bool _wasKinematic;
	public float teleportHeadHeightOffset = -.1f;
	public float grabMoveAbove = 0;
	public float snapTurnAngle = 45;
	public float maxReleaseSpeed = 10;

	/// <summary>
	/// Difference between raw arm length value and where the teleporter is actually placed
	/// </summary>
	public float teleportBeyondReachArmOffset = .1f;

	public float TeleportBeyondReach => GameManager.instance.player.ArmLength - teleportBeyondReachArmOffset;
	public Movement m;

	[Range(0, 16)] public float cdRatioGrabbing = 1;

	private Vector3 originalPivot;
	private Vector3 lastPivot;
	private Vector3 lastPivotTarget;
	private Side pivotSide = Side.None;

	/// <summary>
	/// Called when the teleporter is activated.
	/// </summary>
	public Action<Side> TeleportStart;

	/// <summary>
	/// Called when the teleport happens.
	/// Contains the translation offset vector. 
	/// Side: which hand, float: time the teleporter was held, Vector3: displacement vector of the teleport  
	/// </summary>
	public Action<Side, float, Vector3> TeleportEnd;

	/// <summary>
	/// Contains the direction of the snap turn.
	/// </summary>
	public Action<Side, string> SnapTurn;

	private void Start()
	{
		_wasKinematic = rig.rb.isKinematic;
	}


	private void Update()
	{
		if (GameManager.instance.player.trackedHandsVisible)
		{
			for (int i = 0; i < 2; i++)
			{
				Side side = (Side)i;
				TrackedHand hand = hands[i];

				// // if holding with this hand, but initiating with the other
				// if (!InputManOVRHands.GetHand(side).IsSystemGestureInProgress && InputManOVRHands.Pinch(side) && InputManOVRHands.PinchDown(side.OtherSide()))
				// {
				// 	lastPivot = InputManOVRHands.GetPinchPos(side);
				// 	lastPivotTarget = InputManOVRHands.GetPinchPos(side.OtherSide());
				// 	pivotSide = side;
				// 	originalPivot = lastPivot;
				// }

				#region Grab Move

				bool grabDown = InputManOVRHands.PinchDown(side) && !InputManOVRHands.GetHand(side).IsSystemGestureInProgress;
				bool grab = InputManOVRHands.Pinch(side) && !InputManOVRHands.GetHand(side).IsSystemGestureInProgress;

				// starting a new grab
				if (grabDown)
				{
					// if we are allowed to grab move based on height and stuff
					if (hands[(int)side].CurrentPinchMode
					    is TrackedHand.PinchMode.GrabMove
					    // or TrackedHand.PinchMode.Teleport
					   )
					{
						// if we are already grabbing with a different hand
						if (_grabbingSide != Side.None)
						{
							ReleaseGrab(_grabbingSide);
						}

						StartGrabbing(side, null);
					}
				}

				// been grabbing
				else if (side == _grabbingSide)
				{
					// still grabbing
					if (grab)
					{
						hands[(int)side].grabPos.transform
							.Translate(hands[(int)side].deltaPos * Mathf.Clamp(cdRatioGrabbing - 1, 0, 100));
						GameManager.instance.player.movement.cpt.positionOffset = hands[(int)side].avgOffset;
						// GameManager.instance.player.movement.cpt.enabled = true;
					}
					// stopped grabbing
					else
					{
						ReleaseGrab(side);
					}
				}

				#endregion

				#region Snap Turn

				// if released a pinch that was short and not a drag
				if (InputManOVRHands.PinchUp(side) && hand.pinchHoldDuration is > .01f and < .5f &&
				    Vector3.Distance(hand.initialPinchPos, hand.avgPos) < .1f)
				{
					if (hand.CurrentPinchMode == TrackedHand.PinchMode.SnapTurnLeft)
					{
						rig.transform.RotateAround(rig.head.position, rig.transform.up, snapTurnAngle);
						SnapTurn?.Invoke(side, "left");
					}
					else if (hand.CurrentPinchMode == TrackedHand.PinchMode.SnapTurnRight)
					{
						rig.transform.RotateAround(rig.head.position, rig.transform.up, -snapTurnAngle);
						SnapTurn?.Invoke(side, "right");
					}
				}

				#endregion

				#region Teleporting

				// if not already teleporting
				if (!hand.currentlyTeleporting)
				{
					// if we are in a mode that allows teleporting to start
					if (hand.CurrentPinchMode == TrackedHand.PinchMode.Teleport && InputManOVRHands.PinchDown(side))
					{
						// start teleporting
						_teleportingSide = side;
						hand.currentlyTeleporting = true;
						hand.teleporter.smoothTeleportTime = 0;

						// stop grab moving. This would cause flinging into space otherwise
						if (_grabbingSide != Side.None)
						{
							ReleaseGrab(_grabbingSide);
						}

						TeleportStart?.Invoke(side);
					}
					else
					{
						hand.teleporter.Active = false;
					}
				}
				// if teleporting with this hand
				else if (true || hand.CurrentPinchMode == TrackedHand.PinchMode.Teleport)
				{
					Vector3 lrOffset = rig.head.right * (((int)side * 2 - 1) * -.1f);
					Vector3 upDownOffset = rig.transform.up * teleportHeadHeightOffset;
					RenderTeleporterLine(hand.teleporter, hand.avgPos, (hand.avgPos - rig.head.position + lrOffset + upDownOffset).normalized * hand.teleporter.teleportArcInitialVel);

					hand.teleporter.smoothTeleportTime += Time.deltaTime;

					// if we tapped pinch with this hand
					if (InputManOVRHands.PinchUp(side)
					    // && hand.pinchHoldDuration is > .01f and < .5f
					    // && Vector3.Distance(hand.initialPinchPos, hand.avgPos) < .1f
					   )
					{
						m.TeleportTo(hand.teleporter);
						hand.currentlyTeleporting = false;
						TeleportEnd?.Invoke(side, hand.teleporter.smoothTeleportTime, hand.teleporter.Pos + rig.head.transform.position - transform.position);
					}
				}
				// if stopped teleporting (moved out of teleport zone)
				else
				{
					CancelTeleport(hand);
					hand.currentlyTeleporting = false;
				}

				#endregion
			}
		}
		else
		{
			for (int i = 0; i < 2; i++)
			{
				Side side = (Side)i;
				TrackedHand hand = hands[i];

				hand.teleporter.Active = false;
			}
		}
	}

	private void CancelTeleport(TrackedHand hand)
	{
		_teleportingSide = Side.None;
		hand.currentlyTeleporting = false;
		hand.teleporter.Active = false;
	}

	/// <summary>
	/// Simulates a teleporter line
	/// </summary>
	/// <param name="initialPos">The initial position</param>
	/// <param name="initialVel">The initial trajectory velocity</param>
	/// <returns>The hit position if it exists, null otherwise</returns>
	private Vector3? RenderTeleporterLine(Movement.Teleporter teleporter, Vector3 initialPos, Vector3 initialVel)
	{
		teleporter.Active = true;

		// simulate the curved ray
		Vector3 xVelocity = Vector3.ProjectOnPlane(initialVel, Vector3.up);
		float yVelocity = Vector3.Dot(initialVel, Vector3.up);

		const int maxSegments = 200;

		List<Vector3> points = new List<Vector3>();

		// add the point to the line renderer
		points.Add(initialPos);

		Vector3 lastVel = initialVel;
		Vector3 lastPos = initialPos;

		// the teleport line will stop at a max distance
		for (int i = 0; i < maxSegments; i++)
		{
			// if we hit a wall
			if (Physics.Raycast(lastPos, lastVel.normalized, out RaycastHit teleportHit, lastVel.magnitude,
				    teleporter.validLayers))
			{
				points.Add(teleportHit.point);


				// if the hit point is valid
				if (Vector3.Angle(teleportHit.normal, Vector3.up) < teleporter.maxTeleportableSlope)
				{
					// define the point as a good teleportable point
					teleporter.Pos = teleportHit.point;
					Vector3 dir = rig.head.forward;
					teleporter.Dir = Vector3.ProjectOnPlane(dir, Vector3.up);
					teleporter.Valid = true;
				}
				else
				{
					// if the hit point is close enough to the last valid point
					teleporter.Valid = !(Vector3.Distance(teleporter.Pos, teleportHit.point) > .1f);
				}

				break;
			}
			else
			{
				// calculate the next ray
				lastPos += lastVel;
				Vector3 newPos = lastPos + xVelocity + Vector3.up * yVelocity;
				lastVel = newPos - lastPos;
				yVelocity -= .01f;

				// add the endpoint to the line renderer
				points.Add(lastPos);
			}

			// if we reached the end of the arc without hitting something
			if (i + 1 == maxSegments)
			{
				teleporter.Valid = false;
			}
		}

		if (teleporter.Active)
		{
			teleporter.lineRenderer.positionCount = points.Count;
			teleporter.lineRenderer.SetPositions(points.ToArray());
		}

		return teleporter.Valid ? teleporter.Pos : null;
	}

	private void StartGrabbing(Side side, Transform parent)
	{
		_grabbingSide = side;
		hands[(int)side].currentlyGrabMoving = true;

		if (hands[(int)side].grabPos == null)
		{
			hands[(int)side].grabPos = new GameObject(side + " OVR Hand Grab Pos");
			hands[(int)side].grabPos.transform.SetParent(parent);
		}

		hands[(int)side].grabPos.transform.position = hands[(int)side].avgPos;
		GameManager.instance.player.movement.cpt.SetTarget(hands[(int)side].grabPos.transform, false);
		GameManager.instance.player.movement.cpt.positionOffset = hands[(int)side].avgOffset;
		GameManager.instance.player.movement.cpt.snapIfDistanceGreaterThan = 1f;
		rig.rb.isKinematic = false;
	}

	private void ReleaseGrab(Side side)
	{
		_grabbingSide = Side.None;
		GameManager.instance.player.movement.cpt.SetTarget(null);
		// rig.rb.velocity = MedianAvg(lastVels);
		// rig.rb.velocity = -rig.transform.TransformVector(InputMan.ControllerVelocity(side));
		rig.rb.velocity = Vector3.ClampMagnitude(rig.rb.velocity, maxReleaseSpeed);
		RoundVelToZero();
		rig.rb.isKinematic = _wasKinematic;

		hands[(int)side].currentlyGrabMoving = false;
	}


	private void RoundVelToZero()
	{
		if (rig.rb.velocity.magnitude < .1f)
		{
			rig.rb.velocity = Vector3.zero;
		}
	}
}