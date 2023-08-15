using UnityEngine;

public class RotateWheel : MonoBehaviour
{
	public Transform wheelMesh;
	public WheelCollider wheelCollider;

	private void Update()

	{
		if (wheelMesh != null && wheelCollider != null)
		{
			wheelMesh.RotateAround(wheelCollider.transform.position, -wheelCollider.transform.right, wheelCollider.rpm / 60 * 360 * Time.deltaTime);
		}
	}
}