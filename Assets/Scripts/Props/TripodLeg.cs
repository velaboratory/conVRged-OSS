using UnityEngine;
using unityutilities.VRInteraction;

namespace ENGREDUVR
{
	public class TripodLeg : MonoBehaviour
	{
		public Tripod tripod;
		public Transform adjustableLeg;
		public Transform adjustableLegFullyRetracted;
		public Transform adjustableLegFullyExtended;
		public Transform adjustableLegTip;
		public bool valid = true;
		public VRSlider slider;
		public Vector3 topPosition;
		public Vector3 bottomPosition;
		public float defaultLegAngle = 15f;
		private Quaternion origRot;
		private float stuckDistance = .08f;


		public float Rotation {
			set {
				transform.localRotation = origRot;
				transform.Rotate(-value, 0, 0, Space.Self);
			}
		}


		// Use this for initialization
		private void Start()
		{
			origRot = transform.localRotation;
			slider.OnSlide += LengthAdjusted;
			Adjust();
		}

		public void Adjust()
		{
			adjustableLeg.transform.localPosition = adjustableLegFullyRetracted.localPosition;
			Vector3 minPoint = adjustableLegTip.position;
			adjustableLeg.transform.localPosition = adjustableLegFullyExtended.localPosition;
			Vector3 maxPoint = adjustableLegTip.position;
			adjustableLeg.transform.localPosition = adjustableLegFullyRetracted.localPosition;

			//shoot a ray down the fully retracted position
			RaycastHit hit;
			float maxDist = (maxPoint - minPoint).magnitude;
			if (Physics.Raycast(new Ray(minPoint, -adjustableLegFullyRetracted.forward), out hit, maxDist, LayerMask.GetMask("Ground")))
			{
				float r = hit.distance / maxDist;

				adjustableLeg.localPosition = Vector3.Lerp(adjustableLegFullyRetracted.localPosition,
					adjustableLegFullyExtended.localPosition, r);
				slider.currentSlidePosition = r * maxDist;
				slider.slideMax = maxDist;
				slider.slideMin = 0;
				topPosition = TopPosition;
				bottomPosition = BottomPosition;
				valid = false;
			}
			else
			{
				valid = false;
			}
		}

		public void LengthAdjusted(float currentSlideAmount, float deltaLength, bool localInput)
		{
			float oldLength = Length;
			float newLength = oldLength + deltaLength;
			//if (deltaLength != 0) TODO ADD BACK
			{
				if (tripod)
					tripod.AdjustLegAngleForNewLength(this, oldLength, newLength);

				Adjust();
			}

		}

		public Vector3 TopPosition => transform.position;
		public Vector3 BottomPosition => adjustableLegTip.position;
		public float Length => (BottomPosition - TopPosition).magnitude;

		public void ResetRot()
		{
			Rotation = defaultLegAngle;
		}

		public void LookAtPoint(Vector3 point)
		{
			transform.LookAt(point, Vector3.up);
			transform.Rotate(-90, 0, 0, Space.Self);
		}
	}
}