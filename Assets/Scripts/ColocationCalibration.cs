using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unityutilities;

public class ColocationCalibration : MonoBehaviour {
	public Transform leftHandSpot;
	public Transform rightHandSpot;
	Rig rig;

	void Start() {
		rig = GameManager.instance.player.rig;
	}

	// Update is called once per frame
	void Update() {
		if (InputMan.MenuButtonDown(Side.Either)) {
			rig.transform.Translate((leftHandSpot.position - rig.leftHand.position), Space.World);

			// the direction from the controllers
			Vector3 rightDir = rig.leftHand.position - rig.rightHand.position;
			rightDir.y = 0;
			rightDir.Normalize();

			// the direction from the virtual calibration points
			Vector3 rightDirSpots = leftHandSpot.position - rightHandSpot.position;
			rightDirSpots.y = 0;
			rightDirSpots.Normalize();

			float angle = Vector3.SignedAngle(rightDir, rightDirSpots, Vector3.up);

			rig.transform.RotateAround(rig.leftHand.position, Vector3.up, angle);
		}
	}
}
