using UnityEngine;
using unityutilities;

public class EnableObjectsBasedOnControllerMode : MonoBehaviour
{
	public GameObject[] objects;

	[Header("Enable when:")] public bool controller = true;
	public bool handTracking = true;

	public enum State
	{
		uninitialized,
		Controller,
		HandTracking
	}

	[Header("Current State:")]
	[ReadOnly] public State currentState = State.uninitialized;

	private void Update()
	{
		if (GameManager.instance.player.trackedHandsVisible)
		{
			foreach (GameObject o in objects)
			{
				o.SetActive(handTracking);
			}

			currentState = State.HandTracking;
		}
		else
		{
			foreach (GameObject o in objects)
			{
				o.SetActive(controller);
			}

			currentState = State.Controller;
		}
	}
}