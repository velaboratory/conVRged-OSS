using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VelNet;

public class RoomUserCounts : MonoBehaviour
{
	private double lastUpdate = 0;
	public float updateInterval = 2;

	/// <summary>
	/// room_name, text object
	/// </summary>
	private Dictionary<string, TMP_Text> textObjects = new Dictionary<string, TMP_Text>();

	private void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			textObjects[child.name] = child.GetComponentInChildren<TMP_Text>();
		}
	}

	// Update is called once per frame
	public void Update()
	{
		if (!(Time.timeAsDouble - lastUpdate > updateInterval)) return;

		VelNetManager.GetRooms(rooms =>
		{
			// clear the text values first, because there is no return for empty rooms
			foreach (KeyValuePair<string, TMP_Text> keyValuePair in textObjects)
			{
				keyValuePair.Value.text = "0";
			}

			rooms.rooms.ForEach(r =>
			{
				string sceneName = r.name[..r.name.LastIndexOf('_')];
				string server = r.name[(r.name.LastIndexOf('_')+1)..];
				if (server == SceneSettings.GetLevelPassword())
				{
					if (textObjects.ContainsKey(sceneName))
					{
						textObjects[sceneName].text = r.numUsers.ToString("N0");
					}
				}
			});
		});
		lastUpdate = Time.timeAsDouble;
	}
}