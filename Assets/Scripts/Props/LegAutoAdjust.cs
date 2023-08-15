using UnityEngine;
using unityutilities.VRInteraction;

namespace ENGREDUVR
{
	public class LegAutoAdjust : MonoBehaviour
	{
		public bool localChanges;

		public Transform adjustableLeg;
		public Transform adjustableLegFullyRetracted;
		public Transform adjustableLegFullyExtended;
		public Transform adjustableLegTip;
		public bool valid = true;
		public VRSlider slider;
		public Vector3 topPosition;
		public Vector3 bottomPosition;
		private Quaternion origRot;


		public float Rotation
		{
			set
			{
				transform.localRotation = origRot;
				transform.Rotate(-value, 0, 0, Space.Self);
			}
		}


		// Use this for initialization
		void Start()
		{
			origRot = transform.localRotation;
			slider.OnSlide += LengthAdjusted;
			Adjust();
		}

		private void Update()
		{
			Adjust();
		}

		public void Adjust()
		{
			adjustableLeg.transform.localPosition = adjustableLegFullyRetracted.localPosition;
			Vector3 minPoint = adjustableLegTip.position;
			adjustableLeg.transform.localPosition = adjustableLegFullyExtended.localPosition;
			Vector3 maxPoint = adjustableLegTip.position;
			adjustableLeg.transform.localPosition = adjustableLegFullyRetracted.localPosition;

			//shoot a ray down the fully retracted positionq
			RaycastHit hit;
			float maxDist = (maxPoint - minPoint).magnitude;
			Debug.DrawRay(minPoint, -adjustableLegFullyRetracted.forward);
			if (Physics.Raycast(new Ray(minPoint, -adjustableLegFullyRetracted.forward), out hit, maxDist))
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
			if (localInput) localChanges = true;

			float oldLength = Length;
			float newLength = oldLength + deltaLength;
			if (deltaLength != 0)
			{
				transform.Translate(-Vector3.up * deltaLength, Space.Self);
				Adjust();
			}
		}

		public Vector3 TopPosition => transform.position;
		public Vector3 BottomPosition => adjustableLegTip.position;
		public float Length => (BottomPosition - TopPosition).magnitude;
	}
}