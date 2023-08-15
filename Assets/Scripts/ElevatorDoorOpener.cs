using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoorOpener : MonoBehaviour
{
	[Serializable]
	public class DoorState
	{
		public Transform door;
		public Vector3 goal;
		public Vector3 closedPos;
		public Vector3 openPos;
	}

	public DoorState[] states;
	public string compareTag;
	private List<Collider> nearObjects = new List<Collider>();

	private void Start()
	{
		CloseDoor();
	}

	private void Update()
	{
		foreach (DoorState state in states)
		{
			state.door.localPosition = Vector3.Lerp(state.door.localPosition, state.goal, Time.deltaTime);
		}
	}

	public void OpenDoor()
	{
		foreach (DoorState state in states)
		{
			state.goal = state.openPos;
		}
	}

	public void CloseDoor()
	{
		foreach (DoorState state in states)
		{
			state.goal = state.closedPos;
		}
	}

	// private void OnTriggerStay(Collider other)
	// {
	// 	if (string.IsNullOrEmpty(compareTag)) return;
	// 	if (other.CompareTag(compareTag))
	// 	{
	// 		if (!nearObjects.Contains(other))
	// 		{
	// 			// if this is the first object added
	// 			if (nearObjects.Count == 0)
	// 			{
	// 				OpenDoor();
	// 			}
	// 			nearObjects.Add(other);
	// 		}
	// 	}
	// }
	//
	// private void OnTriggerExit(Collider other)
	// {
	// 	if (string.IsNullOrEmpty(compareTag)) return;
	// 	if (other.CompareTag(compareTag))
	// 	{
	// 		if (nearObjects.Contains(other))
	// 		{
	// 			nearObjects.Remove(other);
	// 			
	// 			// if this is the last object to leave
	// 			if (nearObjects.Count == 0)
	// 			{
	// 				CloseDoor();
	// 			}
	// 		}
	// 	}
	// }
}