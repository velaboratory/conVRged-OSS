using System;
using UnityEngine;
using unityutilities;

public class Drone : RCControllable
{
	private Rigidbody rb;
	public float throttleMultiplier = 1;
	public float torqueMultiplier = 1;
	public float yawTorqueMultiplier = 1;
	public float yawSpeed = 10f;
	public float maxTilt;
	public bool enableYawTorque;
	[Range(-1, 1)] public float roll;
	[Range(-1, 1)] public float pitch;
	[Range(-1, 1)] public float yaw;
	[Range(0, 1)] public float throttle;

	public AudioSource audioSource;

	public PID heightPID;
	public PID rollPID;
	public PID pitchPID;
	public PID xPID;
	public PID yPID;
	public PID yawPID;

	[Serializable]
	public class PID
	{
		public float p;
		public float i;
		public float d;
		[ReadOnly] public float integral;
		[ReadOnly] public float previousError;
		public float goal;
		[ReadOnly] public float output;
		[ReadOnly] public float timeStep;

		public float UpdatePID(float current)
		{
			// Proportional value
			float error = goal - current;

			// Integral value
			integral += error * timeStep;

			// Derivative value
			float derivative = (error - previousError) / timeStep;

			previousError = error;

			output = error * p + integral * i + derivative * d;
			return output;
		}
	}

	public enum DroneMode
	{
		Raw,
		PID,
		HeightPIDOnly,
		AngleModeYaw,
	}

	public DroneMode droneMode;

	// Start is called before the first frame update
	private void Start()
	{
		if (!rb)
		{
			rb = GetComponent<Rigidbody>();
		}

		heightPID.timeStep = Time.fixedDeltaTime;
		heightPID.goal = rb.transform.position.y;

		rollPID.timeStep = Time.fixedDeltaTime;
		rollPID.goal = 0;

		pitchPID.timeStep = Time.fixedDeltaTime;
		pitchPID.goal = 0;

		xPID.timeStep = Time.fixedDeltaTime;
		xPID.goal = rb.transform.position.x;

		yPID.timeStep = Time.fixedDeltaTime;
		yPID.goal = rb.transform.position.z;

		yawPID.timeStep = Time.fixedDeltaTime;
		yawPID.goal = Vector3.SignedAngle(rb.transform.forward, Vector3.forward, Vector3.up);
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		audioSource.enabled = activelyControlled;
		if (!activelyControlled)
		{
			return;
		}

		if (Input.GetKey(KeyCode.LeftBracket))
		{
			yawInput = -1;
		}
		else if (Input.GetKey(KeyCode.RightBracket))
		{
			yawInput = 1;
		}


		switch (droneMode)
		{
			case DroneMode.Raw:
			case DroneMode.HeightPIDOnly:
				roll = horizontalInput;
				pitch = verticalInput;
				yaw = yawInput;
				break;
			case DroneMode.PID:
				xPID.goal += horizontalInput * Time.fixedDeltaTime;
				yPID.goal += verticalInput * Time.fixedDeltaTime;
				yawPID.goal += yawInput * Time.fixedDeltaTime * yawSpeed;
				break;
			case DroneMode.AngleModeYaw:
				rollPID.goal += 0;
				pitchPID.goal = verticalInput * Time.fixedDeltaTime;
				yawPID.goal += horizontalInput * Time.fixedDeltaTime * yawSpeed;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}


		if (Input.GetKey(KeyCode.PageUp))
		{
			switch (droneMode)
			{
				case DroneMode.Raw:
					throttle += Time.fixedDeltaTime;
					throttle = Mathf.Clamp01(throttle);
					break;
				case DroneMode.PID:
				case DroneMode.HeightPIDOnly:
					heightPID.goal += Time.fixedDeltaTime;
					break;
				case DroneMode.AngleModeYaw:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		else if (Input.GetKey(KeyCode.PageDown))
		{
			switch (droneMode)
			{
				case DroneMode.Raw:
					throttle -= Time.fixedDeltaTime;
					throttle = Mathf.Clamp01(throttle);
					break;
				case DroneMode.PID:
				case DroneMode.HeightPIDOnly:
					heightPID.goal -= Time.fixedDeltaTime;
					break;
				case DroneMode.AngleModeYaw:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		// update PIDs
		switch (droneMode)
		{
			case DroneMode.Raw:
				break;
			case DroneMode.HeightPIDOnly:
				throttle = heightPID.UpdatePID(rb.transform.position.y);
				throttle = Mathf.Clamp01(throttle);
				break;
			case DroneMode.PID:
				throttle = heightPID.UpdatePID(rb.transform.position.y);
				throttle = Mathf.Clamp01(throttle);

				roll = xPID.UpdatePID(rb.transform.position.x);
				roll = Mathf.Clamp(roll, -1, 1);

				pitch = yPID.UpdatePID(rb.transform.position.z);
				pitch = Mathf.Clamp(pitch, -1, 1);

				yaw = yawPID.UpdatePID(Vector3.SignedAngle(rb.transform.forward, Vector3.forward, Vector3.up));
				yaw = Mathf.Clamp(yaw, -1, 1);
				break;
			case DroneMode.AngleModeYaw:
				throttle = heightPID.UpdatePID(rb.transform.position.y);
				throttle = Mathf.Clamp01(throttle);

				roll = rollPID.UpdatePID(Vector3.Dot(rb.velocity, rb.transform.right));
				roll = Mathf.Clamp(roll, -1, 1);

				pitch = pitchPID.UpdatePID(Vector3.Dot(rb.velocity, rb.transform.forward));
				pitch = Mathf.Clamp(pitch, -1, 1);

				yaw = yawPID.UpdatePID(Vector3.SignedAngle(rb.transform.forward, Vector3.forward, Vector3.up));
				yaw = Mathf.Clamp(yaw, -1, 1);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		rb.AddForce(rb.transform.up * (throttle * throttleMultiplier * 1000f * Time.fixedDeltaTime));

		Vector3 torq =
			roll * -rb.transform.forward +
			pitch * rb.transform.right;

		Vector3 yawTorq = -rb.transform.up * (yaw * yawTorqueMultiplier);

		// if we are outside of the max allowed tilt
		if (Vector3.Angle(rb.transform.up, Vector3.up) > maxTilt)
		{
			// if this velocity would bring us back to safety
			if (Vector3.Angle(Quaternion.Euler(rb.transform.eulerAngles + torq * Time.fixedDeltaTime) * Vector3.up, Vector3.up) <
			    Vector3.Angle(rb.transform.up, Vector3.up))
			{
				rb.angularVelocity = torq * torqueMultiplier;
				//rb.AddTorque(-(torq - yawTorq) * torqueMultiplier * Time.fixedDeltaTime);
			}
		}
		else
		{
			rb.angularVelocity = torq * torqueMultiplier;
		}

		if (enableYawTorque)
		{
			rb.AddTorque(-(torq - yawTorq) * (torqueMultiplier * Time.fixedDeltaTime));
		}
	}
}