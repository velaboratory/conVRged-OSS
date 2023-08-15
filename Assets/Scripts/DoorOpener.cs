using System.Collections;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
	private Collider col;
	private Material mat;
	public float openingTime = 1;

	public bool open = false;

	// Start is called before the first frame update
	private void Start()
	{
		col = GetComponent<Collider>();
		mat = GetComponent<Renderer>().material;
	}

	/// <summary>
	/// Opens door
	/// </summary>
	/// <returns>Previous state of the door</returns>
	public bool OpenDoor()
	{
		bool prevValue = open;
		StartCoroutine(Fade());
		return prevValue;
	}

	private IEnumerator Fade()
	{
		if (open) yield break;

		open = true;

		float startFade = mat.color.a;

		col.enabled = false;

		for (float x = startFade; x >= 0; x -= Time.deltaTime / openingTime)
		{
			mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, x);
			yield return null;
		}

		GetComponent<MeshRenderer>().enabled = false;
	}
}
