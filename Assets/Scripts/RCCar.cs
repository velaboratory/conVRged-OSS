using System.Collections.Generic;
using UnityEngine;

public class RCCar : RCControllable
{
	public List<WheelCollider> frontWheels;
	public List<WheelCollider> backWheels;
	public float maxMotorTorque; // maximum torque the motor can apply to wheel
	public float maxSteeringAngle; // maximum steer angle the wheel can have

	// Update is called once per frame
	private void FixedUpdate()
	{
		float motor = maxMotorTorque * verticalInput;
		float steering = maxSteeringAngle * horizontalInput;

		foreach (WheelCollider w in frontWheels)
		{
			w.steerAngle = steering;
			w.motorTorque = motor;
		}

		foreach (WheelCollider w in backWheels)
		{
			w.motorTorque = motor;
		}
	}
}
