using UnityEngine;
using TMPro;
using VelNet;

public class ShowVelNetRoomName : MonoBehaviour {

	public TMP_Text text;
	public float updateInterval = 1;
	
	private float timeDiff;
	
	private void Update() {
		if (timeDiff > updateInterval)
		{
	
			text.text = VelNetManager.Room;
	
			timeDiff = 0;
		}
	
		timeDiff += Time.deltaTime;
	}
}