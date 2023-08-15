using UnityEngine;

public class WaterArrowsBox : MonoBehaviour
{
	private static readonly int waterTop = Shader.PropertyToID("_WaterTop");
	public float bouyancyMultiplier = 1;

	private void OnTriggerEnter(Collider other)
	{
		PressurePoints waterArrows = other.attachedRigidbody.GetComponent<PressurePoints>();
		if (waterArrows != null)
		{
			waterArrows.enabled = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		PressurePoints waterArrows = other.attachedRigidbody.GetComponent<PressurePoints>();
		if (waterArrows != null)
		{
			waterArrows.enabled = false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		PressurePoints waterArrows = other.attachedRigidbody.GetComponent<PressurePoints>();
		if (waterArrows != null)
		{
			float surfacePos = transform.position.y + transform.lossyScale.y * .5f;
			waterArrows.waterHeight = surfacePos;

			// makes things float by treating everything as a scaled unit sphere for volume
			// volume of spherical cap https://en.wikipedia.org/wiki/Spherical_cap
			// float r = (other.bounds.extents.x + other.bounds.extents.y + other.bounds.extents.z) / 3f;
			float r = waterArrows.radius;
			float h = Mathf.Clamp(surfacePos - other.attachedRigidbody.transform.position.y + r, 0, r * 2);
			float volume = Mathf.Pow(h, 2) * Mathf.PI * (3 * r - h) / 3;
			other.attachedRigidbody.AddForce(Vector3.up * volume * bouyancyMultiplier);
			other.attachedRigidbody.velocity *= .95f; // add a bunch of drag under the water
		}
	}
}