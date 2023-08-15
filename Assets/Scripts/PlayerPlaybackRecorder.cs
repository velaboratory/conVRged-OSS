using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using unityutilities;
using Logger = unityutilities.Logger;


#if UNITY_EDITOR
/// <summary>
/// Sets up the interface for the ReadJSON script.
/// </summary>
[CustomEditor(typeof(PlayerPlaybackRecorder))]
public class PlayerPlaybackRecorderEditor : Editor
{
#if OCULUS_INTEGRATION
	public override void OnInspectorGUI() {
		var reader = target as PlayerPlaybackRecorder;
		if (reader == null) return;

		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (EditorApplication.isPlaying && GUILayout.Button("Start Recording")) {
			reader.StartRecording();
		}

		if (EditorApplication.isPlaying && GUILayout.Button("Stop Recording")) {
			reader.StopRecording();
		}
	}
#endif
}
#endif

public class PlayerPlaybackRecorder : MonoBehaviour
{
#if OCULUS_INTEGRATION
	public Wolf3DAvatar avatar;
	public bool recording;
	private List<OVRPacket> packetList;
	private int sequence;
	public float updateRate = 20;   // updates per second
	public float lastUpdateTime;
	private string filename;


	struct OVRPacket {
		public uint size;
		public byte[] data;

		public OVRPacket(uint size, byte[] data) : this() {
			this.size = size;
			this.data = data;
		}
	}

	// Update is called once per frame
	void Update() {
		if (!avatar) {
			avatar = PhotonMan.instance.localAvatar;
		}

		if (Input.GetKeyDown(KeyCode.PageUp) || InputMan.Button1Down(Side.Left)) {
			StartRecording();
		}
		if (Input.GetKeyDown(KeyCode.PageDown) || InputMan.Button1Down(Side.Right)) {
			StopRecording();
		}

		SavePacketWolf3D();
	}

	private void SavePacketWolf3D()
	{
		if (recording && (Time.time - lastUpdateTime) > 1f / updateRate)
		{
			var packet = avatar.PackData();

			lastUpdateTime = Time.time;
			List<string> logData = new List<string>();

			using (MemoryStream outputStream = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(outputStream);

				writer.Write(sequence++);
				writer.Write(packet);
				writer.Write(avatar.transform.position);
				writer.Write(avatar.transform.rotation);
				foreach (var item in outputStream.ToArray())
				{
					logData.Add(item.ToString());
				}
				Logger.LogRow(filename + "_ovrpackets", logData);
			}
		}
	}

	private void SavePacketOVR(object sender, OvrAvatar.PacketEventArgs e) {
		if (recording && (Time.time - lastUpdateTime) > 1f / updateRate) {
			lastUpdateTime = Time.time;
			var size = Oculus.Avatar.CAPI.ovrAvatarPacket_GetSize(e.Packet.ovrNativePacket);
			byte[] data = new byte[size];
			Oculus.Avatar.CAPI.ovrAvatarPacket_Write(e.Packet.ovrNativePacket, size, data);
			List<string> logData = new List<string>();

			using (MemoryStream outputStream = new MemoryStream()) {
				BinaryWriter writer = new BinaryWriter(outputStream);

				writer.Write(sequence++);
				writer.Write(size);
				writer.Write(data);
				writer.Write(avatar.transform.position);
				writer.Write(avatar.transform.rotation);
				foreach (var item in outputStream.ToArray()) {
					logData.Add(item.ToString());
				}
				Logger.LogRow(filename + "_ovrpackets", logData);
			}
		}
	}

	public void StartRecording() {
		recording = true;
		filename = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
	}

	public void StopRecording() {
		recording = false;
	}
#endif
}
