using TMPro;
using UnityEngine;


namespace ENGREDUVR
{
	public class MeasuringToolController : MonoBehaviour
	{
		/// <summary>
		/// For billboarding text 
		/// </summary>
		private Transform head;

		public Transform vertex;
		public Transform end0;
		public Transform end1;
		public Transform line0;
		public Transform line1;

		public TMP_Text length0Text;
		public TMP_Text length1Text;
		public TMP_Text angleText;

		private void Start()
		{
			if (Camera.main != null) head = Camera.main.transform;
		}

		// Update is called once per frame
		private void Update()
		{
			UpdateToolTransform();
			MoveText();
			UpdateText();
		}

		private void UpdateToolTransform()
		{
			//position
			Vector3 v = vertex.position;
			Vector3 e0 = end0.position;
			Vector3 e1 = end1.position;

			line0.position = v + (e0 - v) / 2;
			line1.position = v + (e1 - v) / 2;

			//rotation
			line0.up = (e0 - v).normalized;
			line1.up = (e1 - v).normalized;

			//size
			line0.localScale = new Vector3(.01f, Vector3.Distance(e0, v) / 2, .01f);
			line1.localScale = new Vector3(.01f, Vector3.Distance(e1, v) / 2, .01f);
		}

		private void MoveText()
		{
			length0Text.transform.position = line0.position;
			length1Text.transform.position = line1.position;
			angleText.transform.position = vertex.position;

			Vector3 forward = head.forward;
			length0Text.transform.forward = forward;
			length1Text.transform.forward = forward;
			angleText.transform.forward = forward;
		}

		private void UpdateText()
		{
			length0Text.text = Mathf.Round(line0.localScale.y * 2 * 100) / 100f + " m";
			length1Text.text = Mathf.Round(line1.localScale.y * 2 * 100) / 100f + " m";
			Vector3 v = vertex.position;
			angleText.text = Mathf.Round(Vector3.Angle(end0.position - v, end1.position - v)) + " °";
		}
	}
}