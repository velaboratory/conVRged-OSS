using UnityEngine;
using VelNet;

public class EnableObjectsIfMasterClient : MonoBehaviour
{
	public GameObject[] objects;

	public bool enableIfMaster = true;

	private void Update()
	{
		if (objects == null) return;
		bool active = VelNetManager.LocalPlayer?.IsMaster == enableIfMaster;

		foreach (GameObject o in objects)
		{
			o.SetActive(active);
		}
		
	}
}