using UnityEngine;
using VelNet;
using VRPen;
using Debug = UnityEngine.Debug;

public class VRPenInterfaceNetworkConnectorController : MonoBehaviour
{
	public VelNetInterface networkInterface;

	private bool freshRoom = true;

	private void OnEnable()
	{
		VelNetManager.OnJoinedRoom += JoinedRoom;
		VelNetManager.RoomDataReceived += JoinVRPen;
		VelNetManager.CustomMessageReceived += CustomPacketReceived;
	}

	private void OnDisable()
	{
		VelNetManager.OnJoinedRoom -= JoinedRoom;
		VelNetManager.RoomDataReceived -= JoinVRPen;
		VelNetManager.CustomMessageReceived -= CustomPacketReceived;
	}

	private void JoinedRoom(string roomName)
	{
		VelNetManager.GetRoomData(roomName);
		freshRoom = true;
	}

	private void JoinVRPen(VelNetManager.RoomDataMessage roomData)
	{
		if (!freshRoom)
		{
			return;
		}
		freshRoom = false;

		//first in room?
		bool isAloneInRoom = roomData.members.Count == 1;

		//Connect to room, request cache if not first person in room
		if (isAloneInRoom)
		{
			networkInterface.connectedToServer((ulong)VelNetManager.LocalPlayer.userid, false);
		}
		else
		{
			ulong randomOtherPlayerInRoom = 0;
			foreach ((int playerId, string _) in roomData.members)
			{
				if (playerId != VelNetManager.LocalPlayer.userid)
				{
					randomOtherPlayerInRoom = (ulong)playerId;
					break;
				}
			}

			Debug.Log("Requesting VRPen cache from user ID: " + randomOtherPlayerInRoom);
			networkInterface.connectedToServer((ulong)VelNetManager.LocalPlayer.userid, true, randomOtherPlayerInRoom);
		}
	}

	private void CustomPacketReceived(int senderId, byte[] dataWithCategory)
	{
		// vrpen packet
		if (VectorDrawing.OfflineMode) return;

		//get data
		NetworkInterface.PacketCategory cat = (NetworkInterface.PacketCategory)dataWithCategory[0];
		byte[] data = new byte[dataWithCategory.Length - 1];
		for (int x = 0; x < data.Length; x++)
		{
			data[x] = dataWithCategory[x + 1];
		}

		ulong id = (ulong)senderId;

		//pipe the data
		networkInterface.receivePacket(cat, data, id);
	}
}