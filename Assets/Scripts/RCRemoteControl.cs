using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;
using VelNet;

public class RCRemoteControl : MonoBehaviour
{
	public RCControllable target;
	public VRGrabbable grabbable;
	public bool desktopInput;
	private Side lastGrabbedBySide = Side.None;

	private void Start()
	{
		grabbable.Grabbed += DisableTeleporting;
		grabbable.Released += EnableTeleporting;
	}

	// Update is called once per frame
	private void Update()
	{
		if (grabbable && grabbable.GrabbedBy != null)
		{
			Side side = grabbable.GrabbedBy.side;
			target.horizontalInput = NullZones(InputMan.ThumbstickX(side));
			target.verticalInput = -NullZones(InputMan.ThumbstickY(side));
			target.yawInput = InputMan.ThumbstickX(side.OtherSide());
			target.altitudeInput = InputMan.ThumbstickY(side.OtherSide());
			target.activelyControlled = true;
		}
		else if (desktopInput)
		{
			target.horizontalInput = Input.GetAxis("Horizontal");
			target.verticalInput = Input.GetAxis("Vertical");
			//target.yawInput = InputMan.ThumbstickX(Side.Right);
			//target.altitudeInput = InputMan.ThumbstickY(Side.Right);
			target.activelyControlled = true;
		}
		else
		{
			target.activelyControlled = false;
		}
	}

	private static float NullZones(float input)
	{
		input *= 1.5f;
		input = Mathf.Clamp(input, -1, 1);
		return input;
	}

	private void DisableTeleporting()
	{
		// take ownership of the car as well
		target.GetComponent<NetworkObject>().TakeOwnership();

		lastGrabbedBySide = grabbable.GrabbedBy.side;
		Side currentlyEnabledSide = GameManager.instance.player.movement.teleporter.inputSide;

		GameManager.instance.player.movement.teleporter.inputSide = currentlyEnabledSide.SubtractOption(lastGrabbedBySide);
		GameManager.instance.player.movement.turnInput = currentlyEnabledSide.SubtractOption(lastGrabbedBySide);
		GameManager.instance.player.movement.scootBackMovementController = currentlyEnabledSide.SubtractOption(lastGrabbedBySide);
	}

	private void EnableTeleporting()
	{
		Side currentlyEnabledSide = GameManager.instance.player.movement.teleporter.inputSide;

		GameManager.instance.player.movement.teleporter.inputSide = currentlyEnabledSide.AddOption(lastGrabbedBySide);
		GameManager.instance.player.movement.turnInput = currentlyEnabledSide.AddOption(lastGrabbedBySide);
		GameManager.instance.player.movement.scootBackMovementController = currentlyEnabledSide.SubtractOption(lastGrabbedBySide);

		lastGrabbedBySide = Side.None;
	}
}