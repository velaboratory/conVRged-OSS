using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Audio.Playback;
using UnityEngine;
using VelNet.Dissonance;

public class SetAudioFalloff : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		SetDistance(1000);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void SetDistance(float distance)
	{
		foreach (AudioSource audioSource in FindObjectsOfType<VoicePlayback>().Select(e => e.GetComponent<AudioSource>()))
		{
			audioSource.maxDistance = distance;
		}
	}
}