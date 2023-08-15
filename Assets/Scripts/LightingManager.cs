using UnityEngine;
using VELConnect;

public class LightingManager : MonoBehaviour
{
	public GameObject[] dayOnlyObjects;
	public GameObject[] nightOnlyObjects;

	public LevelLightmapData levelLightmapData;

	private void Start()
	{
		levelLightmapData.LoadLightingScenario(0);
		VELConnectManager.AddRoomDataListener("night", this, value =>
		{
			bool isNight = value == "true";
			levelLightmapData.LoadLightingScenario(isNight ? 1 : 0);
			foreach (GameObject g in dayOnlyObjects)
			{
				g.SetActive(!isNight);
			}

			foreach (GameObject g in nightOnlyObjects)
			{
				g.SetActive(isNight);
			}
		}, true);
	}
}