using System.IO;
using VELConnect;
using VelNet;
using VELShareUnity;
using VRPen;

public class VELShareManager : SyncState
{
	public WebRTCReceiver webRtcManager;
	public string localStreamRoom = "";

	private void Start()
	{
		VELConnectManager.AddDeviceDataListener("streamer_stream_id", this, OnStreamerStreamIdChanged, true);

		// networkObject.OwnershipChanged += p =>
		// {
		// 	if (IsMine)
		// 	{
		// 		webRtcManager.streamRoom = localStreamRoom;
		// 		webRtcManager.Shutdown();
		// 		webRtcManager.Startup(webRtcManager.streamRoom);
		// 	}
		// };
	}

	private void OnStreamerStreamIdChanged(string s)
	{
		Debug.Log($"Streamer id changed: {s}");
		localStreamRoom = s;
		webRtcManager.streamRoom = s;
		webRtcManager.Shutdown();
		webRtcManager.Startup(webRtcManager.streamRoom);
		networkObject.TakeOwnership();
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(webRtcManager.streamRoom);
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		string newStreamRoom = binaryReader.ReadString();
		if (webRtcManager.streamRoom != newStreamRoom)
		{
			webRtcManager.streamRoom = newStreamRoom;
			webRtcManager.Shutdown();
			webRtcManager.Startup(webRtcManager.streamRoom);
		}
	}
}