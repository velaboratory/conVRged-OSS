using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using unityutilities.VRInteraction;

[RequireComponent(typeof(VRGrabbable))]
public class SoundsOnGrabAndRelease : MonoBehaviour
{
	private VRGrabbable g;

	public AudioSource grabSound;
	public AudioSource releaseSound;

	private void OnEnable()
	{
		g = GetComponent<VRGrabbable>();
		g.Grabbed += PlayGrabSound;
		g.Released += PlayReleaseSound;
	}

	private void OnDisable()
	{
		g.Grabbed -= PlayGrabSound;
		g.Released -= PlayReleaseSound;
	}

	private void PlayGrabSound()
	{
		grabSound.Play();
	}

	private void PlayReleaseSound()
	{
		releaseSound.Play();
	}

#if UNITY_EDITOR
	/// <summary>
	/// Sets up the interface for the CopyTransform script.
	/// </summary>
	[CustomEditor(typeof(SoundsOnGrabAndRelease))]
	public class SoundsOnGrabAndReleaseEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			SoundsOnGrabAndRelease t = target as SoundsOnGrabAndRelease;

			EditorGUILayout.Space();

			if (GUILayout.Button("Add 2 default AudioSource"))
			{
				AudioSource audio1 = t.gameObject.AddComponent<AudioSource>();
				AudioSource audio2 = t.gameObject.AddComponent<AudioSource>();
				audio1.spatialBlend = 1;
				audio2.spatialBlend = 1;
				audio1.playOnAwake = false;
				audio2.playOnAwake = false;
				SerializedObject so = new SerializedObject(t);
				so.FindProperty("grabSound").objectReferenceValue = audio1;
				so.FindProperty("releaseSound").objectReferenceValue = audio2;
				so.ApplyModifiedProperties();
			}

			EditorGUILayout.Space();

			DrawDefaultInspector();
		}
	}
#endif
}