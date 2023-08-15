using System;
using System.Collections;
using System.Collections.Generic;
using conVRged;
using UnityEngine;
using UnityEngine.Events;
using VelNet;

public class PlayerTrigger : MonoBehaviour
{
	[Serializable]
	public class PlayerTriggerEvent : UnityEvent<GameObject, VelNetPlayer>
	{
	}

	public PlayerTriggerEvent OnTriggerEnterEvent;
	public PlayerTriggerEvent OnTriggerExitEvent;

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			VelNetWolf3DAvatarController player = other.GetComponent<VelNetWolf3DAvatarController>();
			if (player == null)
			{
				Debug.LogError("Player that isn't a player.");
				return;
			}

			OnTriggerEnterEvent?.Invoke(player.gameObject, player.networkObject.owner);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			VelNetWolf3DAvatarController player = other.GetComponent<VelNetWolf3DAvatarController>();
			if (player == null)
			{
				Debug.LogError("Player that isn't a player.");
				return;
			}

			OnTriggerExitEvent?.Invoke(player.gameObject, player.networkObject.owner);
		}
	}
}