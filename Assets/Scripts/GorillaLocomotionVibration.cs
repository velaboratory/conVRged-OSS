using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unityutilities;

public class GorillaLocomotionVibration : MonoBehaviour
{
	public GorillaLocomotion.Player player;

	// Start is called before the first frame update
	private void Start()
	{
		player.HandTouchEnter += (isRight, pos, vel) =>
		{
			if (vel.magnitude < .001f) return;

			InputMan.Vibrate(isRight ? Side.Right : Side.Left, .5f);
		};
	}
}