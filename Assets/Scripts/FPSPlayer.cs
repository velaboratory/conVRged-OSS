using UnityEngine;
using unityutilities;

/// <summary>
/// Must be placed on the top-level player object that is moved
/// </summary>
public class FPSPlayer : MonoBehaviour
{
	public Transform fpsCamera;
	public Rig rig;
	[Range(0, 2f)] public float mouseSpeed = .5f;
	private float pitch;
	public float jumpForce = 1000;
	public float slidingSpeed = 3;
	public float shiftSpeedMultiplier = 1.5f;


	// Update is called once per frame
	private void Update()
	{
		fpsCamera.position = rig.head.position;
		Vector3 pos = fpsCamera.localPosition;
		pos.y = 1.5f;
		fpsCamera.localPosition = pos;

		// Hide and lock cursor when right mouse button pressed
		if (Input.GetMouseButtonDown(1))
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		// Unlock and show cursor when right mouse button released
		if (Input.GetMouseButtonUp(1))
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetMouseButton(1))
		{
			Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			rig.rb.transform.RotateAround(rig.head.position, rig.rb.transform.up, mouseMovement.x * mouseSpeed);
			pitch += -mouseMovement.y * mouseSpeed;
			pitch = Mathf.Clamp(pitch, -90, 90);
			fpsCamera.localRotation = Quaternion.identity;
			fpsCamera.Rotate(Vector3.right, pitch, Space.Self);
		}

		// auto-jump across terrain height changes
		if (Physics.Raycast(fpsCamera.position, -Vector3.up, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
		{
			float diff = hit.distance - fpsCamera.localPosition.y;
			transform.position = Vector3.Lerp(transform.position, transform.position - diff * Vector3.up, .2f);
		}


		SlidingMovement();
	}

	private void SlidingMovement()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = -Input.GetAxis("Vertical");

		// JUMP 🦘
		if (Input.GetKeyDown(KeyCode.Space))
		{
			rig.rb.AddForce(Vector3.up * jumpForce);
		}

		Vector3 forward = -rig.head.forward;
		forward.y = 0;
		forward.Normalize();

		Vector3 right = new Vector3(-forward.z, 0, forward.x);


		Vector3 currentSpeed = rig.rb.velocity;
		Vector3 forwardSpeed = vertical * forward;
		Vector3 rightSpeed = horizontal * right;
		Vector3 speed = forwardSpeed + rightSpeed;
		if (Input.GetKey(KeyCode.LeftShift)) speed *= shiftSpeedMultiplier;
		rig.rb.velocity = slidingSpeed * speed + (currentSpeed.y * rig.rb.transform.up);
	}
}