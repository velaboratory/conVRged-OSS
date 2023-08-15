using UnityEngine;
using UnityEngine.UI;

public class TicTacToeToggle : MonoBehaviour {
	public Rigidbody spinnyPart;
	public float forceMultiplier = 10f;
	public bool wasOn;
	private int frameIndex = 0;
	public int updateInterval = 10;
	

	public AnimationCurve forceCurve = new AnimationCurve(new Keyframe(-1,1), new Keyframe(1,-1));

	public Toggle.ToggleEvent onValueChanged;

	// Start is called before the first frame update
	private void Start() {
		wasOn = isOn;
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		// only do every n frames for performance
		if (frameIndex++ % updateInterval != 0) return;
		
		if (Vector3.Angle(transform.forward, spinnyPart.transform.forward) < 90f)
		{
			float angle = Vector3.SignedAngle(transform.forward, spinnyPart.transform.forward, transform.up);
			float force = forceCurve.Evaluate(angle / 90f);

			spinnyPart.AddTorque(0, force * forceMultiplier * Time.fixedDeltaTime, 0);
		}

		if (Vector3.Angle(transform.forward, -spinnyPart.transform.forward) < 90f)
		{
			float angle = Vector3.SignedAngle(transform.forward, -spinnyPart.transform.forward, transform.up);
			float force = forceCurve.Evaluate(angle / 90f);

			spinnyPart.AddTorque(0, force * forceMultiplier * Time.fixedDeltaTime, 0);
		}

		if (wasOn != isOn)
		{
			onValueChanged?.Invoke(isOn);
		}

		wasOn = isOn;
	}

	public bool isOn {
		get => Vector3.Angle(spinnyPart.transform.forward, transform.forward) > 90;
		set => spinnyPart.transform.localEulerAngles = value ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
	}
}
