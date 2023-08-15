using System.Collections.Generic;
using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;
using Logger = unityutilities.Logger;

public class ConvrgedPlayerLogger : MonoBehaviour
{
	public Rig rig;
	public Movement m;
	public OVRHandLocomotion handLoco;
	[Space] private const string positionsFileName = "positions";
	private const string inputsFileName = "inputs";
	private const string movementFileName = "movement";
	[Space] public float updateRateHz = 4;

	private float nextUpdateTime;

	public bool logInputs;

	// Start is called before the first frame update
	private void Start()
	{
		m.TeleportStart += (side) => { TeleportStart(side, "controller"); };
		m.TeleportEnd += (side, time, vector3) => { TeleportEnd(side, "controller", time, vector3); };
		m.SnapTurn += (side, direction) => { SnapTurn(side, "controller", direction); };

		handLoco.TeleportStart += side => { TeleportStart(side, "hand"); };
		handLoco.TeleportEnd += (side, time, vector3) => { TeleportEnd(side, "hand", time, vector3); };
		handLoco.SnapTurn += (side, direction) => { SnapTurn(side, "hand", direction); };

		m.OnGrab += (go, side) =>
		{
			List<string> l = new StringList(new List<dynamic>
			{
				"grab-move",
				side
			}).List;

			Logger.LogRow(movementFileName, l);
		};
		m.OnGrabCancel += (go, side, time) =>
		{
			List<string> l = new StringList(new List<dynamic>
			{
				"grab-cancel",
				side,
				time
			}).List;

			Logger.LogRow(movementFileName, l);
		};
		m.OnRelease += (go, side, localOffset, globalOffset, velocity, time) =>
		{
			List<string> l = new StringList(new List<dynamic>
			{
				"grab-release",
				side,
				localOffset,
				globalOffset,
				velocity,
				time
			}).List;

			Logger.LogRow(movementFileName, l);
		};
	}

	private void Update()
	{
		// if we are due for an update
		if (Time.time < nextUpdateTime) return;

		// set the next update time
		nextUpdateTime += 1f / updateRateHz;

		// if we are still behind, we missed an update - just reset
		if (Time.time > nextUpdateTime)
		{
			Debug.Log("Missed a log cycle", this);
			nextUpdateTime = Time.time + 1f / updateRateHz;
		}


		StringList positions = new StringList();

		// tracking space pos
		Vector3 spacePos = rig.transform.position;
		Quaternion spaceRot = rig.transform.rotation;
		positions.Add(spacePos);
		positions.Add(spaceRot);
		positions.Add(rig.transform.eulerAngles.y);

		// local space of head and hands
		Vector3 headPos = rig.head.position;
		Quaternion headRot = rig.head.rotation;
		positions.Add(headPos);
		positions.Add(headRot);

		Vector3 leftHandPos = rig.leftHand.position;
		Quaternion leftHandRot = rig.leftHand.rotation;
		positions.Add(leftHandPos);
		positions.Add(leftHandRot);

		Vector3 rightHandPos = rig.rightHand.position;
		Quaternion rightHandRot = rig.rightHand.rotation;
		positions.Add(rightHandPos);
		positions.Add(rightHandRot);


		positions.Add(GameManager.instance.player.leftOVRHand.IsTracked);
		positions.Add(GameManager.instance.player.rightOVRHand.IsTracked);

		Vector3 leftTrackedHandPos = GameManager.instance.player.leftTrackedHand.avgPos;
		positions.Add(leftTrackedHandPos);
		Vector3 rightTrackedHandPos = GameManager.instance.player.rightTrackedHand.avgPos;
		positions.Add(rightTrackedHandPos);

		Logger.LogRow(positionsFileName, positions.List);

		if (logInputs)
		{
			List<string> l = new StringList(new List<dynamic>
			{
				// trigger
				InputMan.Trigger(Side.Left),
				InputMan.Trigger(Side.Right),
				InputMan.TriggerValue(Side.Left),
				InputMan.TriggerValue(Side.Right),
				// grip
				InputMan.Grip(Side.Left),
				InputMan.Grip(Side.Right),
				InputMan.GripValue(Side.Left),
				InputMan.GripValue(Side.Right),
				// buttons
				InputMan.Button1(Side.Left),
				InputMan.Button1(Side.Right),
				InputMan.Button2(Side.Left),
				InputMan.Button2(Side.Right),
				// thumbstick
				InputMan.Up(Side.Left),
				InputMan.Up(Side.Right),
				InputMan.Down(Side.Left),
				InputMan.Down(Side.Right),
				InputMan.Left(Side.Left),
				InputMan.Left(Side.Right),
				InputMan.Right(Side.Left),
				InputMan.Right(Side.Right),
				InputMan.ThumbstickX(Side.Left),
				InputMan.ThumbstickX(Side.Right),
				InputMan.ThumbstickY(Side.Left),
				InputMan.ThumbstickY(Side.Right),
				InputMan.ThumbstickPress(Side.Left),
				InputMan.ThumbstickPress(Side.Right),

				GameManager.instance.player.leftOVRHand.IsTracked,
				GameManager.instance.player.rightOVRHand.IsTracked,
				GameManager.instance.player.leftOVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Index),
				GameManager.instance.player.rightOVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Index),
			}).List;

			Logger.LogRow(inputsFileName, l);
		}
	}


	private void TeleportStart(Side side, string loco)
	{
		StringList l = new StringList(new List<dynamic>()
		{
			"teleport-start",
			side,
			loco,	
		});

		Logger.LogRow(movementFileName, l.List);
	}

	private void TeleportEnd(Side side, string loco, float time, Vector3 translation)
	{
		StringList l = new StringList(new List<dynamic>()
		{
			"teleport-end",
			side,
			loco,
			time,
			translation,
		});


		Logger.LogRow(movementFileName, l.List);
	}

	private void SnapTurn(Side side, string loco, string direction)
	{
		List<string> l = new StringList(new List<dynamic>
		{
			"snap-turn",
			side,
			loco,
			direction,
		}).List;

		Logger.LogRow(movementFileName, l);
	}
}

public static class PlayerLoggerExtensionMethods
{
	public static Vector3 ToMomentVector(this Quaternion value)
	{
		value.ToAngleAxis(out float angle, out Vector3 axis);
		return axis * angle;
	}
}