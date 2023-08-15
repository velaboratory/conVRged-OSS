using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VelNet;

public class ConferenceRoomController : SyncState
{
	public bool whiteboardVisible;

	public Transform whiteboard;
	public Transform projector;

	public float speed = .01f;


	private void Update()
	{
		if (whiteboardVisible)
		{
			whiteboard.transform.localPosition = Vector3.Lerp(whiteboard.transform.localPosition, Vector3.zero, speed);
			projector.transform.localPosition = Vector3.Lerp(projector.transform.localPosition, Vector3.up * 3, speed);
		}
		else
		{
			whiteboard.transform.localPosition = Vector3.Lerp(whiteboard.transform.localPosition, Vector3.up * 3, speed);
			projector.transform.localPosition = Vector3.Lerp(projector.transform.localPosition, Vector3.zero, speed);
		}
	}

	public void SetWhiteboardVisible(bool visible)
	{
		networkObject.TakeOwnership();
		whiteboardVisible = visible;
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(whiteboardVisible);
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		whiteboardVisible = binaryReader.ReadBoolean();
	}
}