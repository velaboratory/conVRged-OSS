using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VelNet;

public class VelNetSyncMusicPlaying : SyncState
{
	public AudioSource audioSource;

	private bool wasPlaying;

	private void Start()
	{
		wasPlaying = audioSource.playOnAwake;
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(audioSource.isPlaying);
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		bool isPlaying = binaryReader.ReadBoolean();
		if (isPlaying && !wasPlaying)
		{
			audioSource.Play();
		}
		else if (!isPlaying && wasPlaying)
		{
			audioSource.Stop();
		}

		wasPlaying = isPlaying;
	}
}