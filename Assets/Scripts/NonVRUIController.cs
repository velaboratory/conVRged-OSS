using System;
using MindMap;
using UnityEngine;
using unityutilities;

public class NonVRUIController : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			ExportAllMindMaps();
		}
	}

	public void ExportAllMindMaps()
	{
		MindMapController[] controllers = FindObjectsOfType<MindMapController>();
		foreach (MindMapController mindMapController in controllers)
		{
			mindMapController.Log();
		}
	}
}