using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
/// <summary>
/// Sets up the interface for the ReadJSON script.
/// </summary>
[CustomEditor(typeof(AvatarPlayback))]
public class PlayerPlaybackEditor : Editor
{
#if OCULUS_INTEGRATION
	public override void OnInspectorGUI()
	{
		var reader = target as AvatarPlayback;
		if (reader == null) return;

		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (EditorApplication.isPlaying && GUILayout.Button("Play"))
		{
			reader.Play();
		}
	}
#endif
}
#endif


public class AvatarPlayback : MonoBehaviour
{
	public List<List<string>> lines;

#if OCULUS_INTEGRATION
	public OvrAvatarRemoteDriver demoAvatar;
	public OvrAvatar ovrAvatar;
#else
	public Wolf3DAvatar demoAvatar;
#endif
	public string fileName;
	private int playbackSequence;
	public float updateRate = 20;
	private bool playing;
	bool init = false;
	public Color playerColor = Color.red;

	public void ReadLog()
	{

		// load the file. Assumes .txt file ending
		TextAsset textFile = Resources.Load<TextAsset>(fileName);

		if (textFile == null) return;

		string[] rawLines = textFile.text.Split('\n');
		lines = new List<List<string>>();
		foreach (var line in rawLines)
		{
			List<string> row = new List<string>(line.Split('\t'));
			if (row.Count > 3)
			{
				row.RemoveAt(0);
				row.RemoveAt(0);
				row.RemoveAt(row.Count - 1);
				lines.Add(row);
			}
		}
		Debug.Log("Read " + lines.Count + " lines.");

	}

	public void Play()
	{
		if (lines == null)
		{
			ReadLog();
		}

		if (!playing)
			StartCoroutine(PlaybackLogCoroutine());
	}

	private IEnumerator PlaybackLogCoroutine()
	{
		playing = true;

		// interpret lines obj
		foreach (var line in lines)
		{
			List<byte> bytes = new List<byte>();
			foreach (var elem in line)
			{
				if (elem.Length > 0)
				{
					bytes.Add(byte.Parse(elem));
				}
			}

			demoAvatar.gameObject.SetActive(true);

#if OCULUS_INTEGRATION
			while (ovrAvatar.HandLeft == null)
			{
				yield return null;
			}
#endif
			init = true;

			SetPlayerColor(playerColor);



			using (MemoryStream inputStream = new MemoryStream(bytes.ToArray()))
			{
				BinaryReader binReader = new BinaryReader(inputStream);

				// oculus avatar packet
				int remoteSequence = binReader.ReadInt32();

				int size = binReader.ReadInt32();
				byte[] sdkData = binReader.ReadBytes(size);


#if OCULUS_INTEGRATION
				Vector3 position = binReader.ReadVector3();
				Quaternion rotation = binReader.ReadQuaternion();

				IntPtr packet = Oculus.Avatar.CAPI.ovrAvatarPacket_Read((uint)sdkData.Length, sdkData);
				demoAvatar.QueuePacket(playbackSequence++, new OvrAvatarPacket { ovrNativePacket = packet });
				demoAvatar.transform.position = position;
				demoAvatar.transform.rotation = rotation;
#else
				// TODO non-oculus avatar playback
#endif


				yield return new WaitForSeconds(1f / updateRate);
			}

			demoAvatar.gameObject.SetActive(false);
		}
		playing = false;
	}

	private void SetPlayerColor(Color playerColor)
	{

#if OCULUS_INTEGRATION
		ovrAvatar.Body.RenderParts[2].GetComponent<Renderer>().material.SetColor("_BaseColor", playerColor);        // headset
		ovrAvatar.Body.RenderParts[1].GetComponent<Renderer>().material.SetColor("_BaseColor", playerColor);        // shirt
		ovrAvatar.HandLeft.RenderParts[0].GetComponent<Renderer>().material.SetColor("_BaseColor", playerColor);    // left hand
		ovrAvatar.HandRight.RenderParts[0].GetComponent<Renderer>().material.SetColor("_BaseColor", playerColor);   // right hand
#endif
	}
}
