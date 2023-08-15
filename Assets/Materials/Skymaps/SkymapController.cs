using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkymapController : MonoBehaviour
{
	public Transform timeOfDaySkyboxes;
	public GameObject constellationsObject;
	public Renderer constellationsRenderer;
	public Renderer starsRenderer;

	// Start is called before the first frame update
	private void Start()
	{
		VELConnect.VELConnectManager.AddRoomDataListener($"planetariumTime", this, s =>
		{
			Vector3 rot = timeOfDaySkyboxes.localEulerAngles;
			rot.x = float.Parse(s) / 24 * 360;
			timeOfDaySkyboxes.localEulerAngles = rot;
		});
		VELConnect.VELConnectManager.AddRoomDataListener($"constellationsVisible", this, s =>
		{
			if (bool.TryParse(s, out bool visible))
			{
				constellationsObject.SetActive(visible);
			}
		});
		VELConnect.VELConnectManager.AddRoomDataListener($"planetariumConstellationExposure", this, s => { constellationsRenderer.material.SetFloat("_Exposure", float.Parse(s)); });
		VELConnect.VELConnectManager.AddRoomDataListener($"planetariumExposure", this, s => { starsRenderer.material.SetFloat("_Exposure", float.Parse(s)); });
	}

	// Update is called once per frame
	void Update()
	{
	}
}