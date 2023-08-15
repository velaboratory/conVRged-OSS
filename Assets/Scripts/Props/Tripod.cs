using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using unityutilities.VRInteraction;


namespace ENGREDUVR
{
	public class Tripod : MonoBehaviour
	{
		public TripodLeg legA;
		public TripodLeg legB;
		public TripodLeg legC;
		private VRMoveable myMovable;

		public AudioSource releaseSound;
		public AudioSource grabSound;
		public SnapPoint snapPoint;
		public bool snapLegsOnStart;

		[Header("Show Total Station Height")] public bool showTSHeightObj = true;
		public Transform TSHeightObj;
		public Text tsHeightText;
		private Transform tsHeightObjAimTarget;

		private Vector3 lastPos;

		private bool foldedUp;

		private bool FoldedUp
		{
			get => foldedUp;
			set
			{
				if (value != foldedUp)
				{
				}
			}
		}


		// Use this for initialization
		private IEnumerator Start()
		{
			myMovable = GetComponent<VRMoveable>();
			myMovable.Moved += TripodMoved;
			myMovable.Grabbed += TripodGrabbed;
			myMovable.Released += TripodReleased;
			legA.tripod = this;
			legB.tripod = this;
			legC.tripod = this;

			lastPos = transform.position;

			TSHeightObj.gameObject.SetActive(snapPoint.snapped && showTSHeightObj);

			yield return null;

			if (snapLegsOnStart)
			{
				TripodMoved(true);
			}
		}

		// Update is called once per frame
		private void Update()
		{
			// only if position is dirty
			if (lastPos != transform.position)
			{
				TripodMoved(false);
				lastPos = transform.position;
			}


			if (tsHeightObjAimTarget != null)
			{
				Vector3 aimTargetPos = tsHeightObjAimTarget.position;
				aimTargetPos.y = TSHeightObj.position.y;
				TSHeightObj.LookAt(aimTargetPos, transform.up);
			}
			else
			{
				tsHeightObjAimTarget = GameManager.instance?.player.rig.head;
			}
		}

		private void TripodMoved(bool localInput)
		{
			legA.ResetRot();
			legB.ResetRot();
			legC.ResetRot();

			legA.Adjust();
			legB.Adjust();
			legC.Adjust();

			// Show total station height
			if (snapPoint.snapped && showTSHeightObj)
			{
				if (Physics.Raycast(TSHeightObj.position, -TSHeightObj.up, out RaycastHit hit, 10f,
					    LayerMask.GetMask("Ground")))
				{
					tsHeightText.text = "Total Station Height: " + (hit.distance + .753f).ToString("#.###") + " m";
				}

				TSHeightObj.gameObject.SetActive(true);
			}
			else
			{
				TSHeightObj.gameObject.SetActive(false);
			}
		}

		private void TripodGrabbed()
		{
			grabSound.Play();
		}

		private void TripodReleased()
		{
			releaseSound.Play();
		}

		// Call before adjusting the height
		public void AdjustLegAngleForNewLength(TripodLeg leg, float oldLength, float newLength)
		{
			//when the height is adjusted, the whole tripod rotates to compensate, counter rotating on the leg axis to compensate
			Vector3 bOther1;
			Vector3 bOther2;

			if (leg == legA)
			{
				bOther1 = legC.bottomPosition;
				bOther2 = legB.bottomPosition;
			}
			else if (leg == legB)
			{
				bOther1 = legA.bottomPosition;
				bOther2 = legC.bottomPosition;
			}
			else if (leg == legC)
			{
				bOther1 = legB.bottomPosition;
				bOther2 = legA.bottomPosition;
			}
			else
			{
				return;
			}

			float delta = newLength - oldLength;

			//legs are a, b, c, looking down from the top in counterclockwise order

			var axis = (bOther1 - bOther2).normalized;
			var between = (bOther1 + bOther2) / 2.0f;


			transform.RotateAround(between, axis, (newLength - oldLength) * -20f);
		}

		// 📷
		private float ComputeDeltaAngle(float da, float a, float b, float c)
		{
			float A = Mathf.Acos((a * a - b * b - c * c) / (-2 * b * c));
			float dA = (Mathf.Acos((a * a + 2 * da + da * da - b * b - c * c) / (-2 * b * c)) - A) * Mathf.Rad2Deg;
			return dA;
		}
	}
}