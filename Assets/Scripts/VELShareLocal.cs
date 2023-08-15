using UnityEngine;
using VELConnect;
using VELShareUnity;

public class VELShareLocal : MonoBehaviour
{
	public WebRTCReceiver webRtcManager;
	public string localStreamRoom = "";

	private void Start()
	{
		VELConnectManager.AddDeviceDataListener("streamer_stream_id", this, OnStreamerStreamIdChanged, true);
	}

	private void OnStreamerStreamIdChanged(string s)
	{
		localStreamRoom = s;
		webRtcManager.streamRoom = s;
		webRtcManager.Shutdown();
		webRtcManager.Startup(webRtcManager.streamRoom);
	}
}