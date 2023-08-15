using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LinearSpring : MonoBehaviour
{
	// please lock rotation yourself
	private Rigidbody rb;

	// direction of press is always on the local negative y
	public float distance = .1f;
	public float forceMultiplier = 10f;
	private Vector3 initialPosition;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		initialPosition = transform.localPosition;
	}

	private void FixedUpdate()
	{
		Vector3 pos = transform.localPosition;
		if (pos.y < 0)
		{
			rb.AddForce(transform.up * forceMultiplier);
		}

		if (pos.y < -distance)
		{
			rb.AddForce(transform.up * (forceMultiplier * 10f));
		}

		pos.x = 0;
		if (pos.y > 0)
		{
			rb.velocity = Vector3.zero;
		}

		transform.localRotation = Quaternion.identity;

		pos.y = Mathf.Clamp(pos.y, -distance, 0);
		pos.z = 0;
		transform.localPosition = pos;
	}
}