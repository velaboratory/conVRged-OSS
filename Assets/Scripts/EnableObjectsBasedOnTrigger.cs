using System;
using UnityEngine;

public class EnableObjectsBasedOnTrigger : MonoBehaviour
{
	public bool enableOnStart;
	public bool disableOnStart;
	public GameObject[] objects;

	private void Start()
	{
		if (enableOnStart)
		{
			foreach (GameObject o in objects)
			{
				o.SetActive(true);
			}
		}

		if (disableOnStart)
		{
			foreach (GameObject o in objects)
			{
				o.SetActive(false);
				
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("MainCamera")) return;
		foreach (GameObject o in objects)
		{
			o.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("MainCamera")) return;
		foreach (GameObject o in objects)
		{
			o.SetActive(false);
			// if (o.GetComponent<CanvasWebViewPrefab>())
			// {
			// 	o.GetComponent<CanvasWebViewPrefab>().WebView.Dispose();
			// }
		}
	}
}