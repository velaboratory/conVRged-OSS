using System;
using UnityEngine;
using unityutilities.VRInteraction;
using VelNet;

namespace ENGREDUVR
{
	public class Prism : MonoBehaviour
	{
		[Header("Parts")] public VRMoveableButOnceItsStuckInTheGroundItsADial moveableObj;
		public VRDial panDial;
		public VRDial pitchDial;

		[Header("Height Indicator")] public bool showHeightObj = true;
		public Transform heightObj;
		private Transform heightObjAimTarget;

		private void Start()
		{
			heightObj.gameObject.SetActive(showHeightObj);
		}

		private void Update()
		{
			if (heightObjAimTarget != null)
			{
				Vector3 aimTargetPos = heightObjAimTarget.position;
				aimTargetPos.y = heightObj.position.y;
				heightObj.LookAt(aimTargetPos, transform.up);
			}
			else
			{
				heightObjAimTarget = GameManager.instance.player.rig.head;
			}
		}
	}
}