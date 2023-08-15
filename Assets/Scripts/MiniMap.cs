using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VelNet;

public class MiniMap : MonoBehaviour
{
	public GameObject markerPrefab;
	public Transform scene_0_0;
	public Transform scene_10_10;
	private Vector3 scaleFactor;
	private List<Transform> markers = new List<Transform>();

	// Update is called once per frame
	private void Update()
	{
		scaleFactor = (scene_10_10.localPosition - scene_0_0.localPosition) / 10f;

		// remove extra markers
		while (markers.Count > VelNetManager.Players.Count)
		{
			Destroy(markers.Last().gameObject);
			markers.RemoveAt(markers.Count - 1);
		}

		// add missing markers
		while (markers.Count < VelNetManager.Players.Count)
		{
			Transform obj = Instantiate(markerPrefab, transform).transform;
			markers.Add(obj);
		}

		for (int i = 0; i < VelNetManager.Players.Count; i++)
		{
			VelNetPlayer velNetPlayer = VelNetManager.Players[i];
			Vector3 pos = VelNetMan.GetPlayerPrefab(velNetPlayer).transform.position;
			markers[i].localPosition = new Vector3(pos.x * scaleFactor.x, pos.z * scaleFactor.y, 0) + scene_0_0.localPosition;
		}
	}
}