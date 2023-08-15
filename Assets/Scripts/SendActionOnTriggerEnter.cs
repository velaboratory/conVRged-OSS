using System;
using UnityEngine;
using UnityEngine.Events;

public class SendActionOnTriggerEnter : MonoBehaviour {
	[Tooltip("Only other objects with this tag will activate the trigger.")]
	public new string tag;
	public Action triggerEntered;

	[Serializable]
	public class TriggerEnterEvent : UnityEvent { }

	public TriggerEnterEvent triggerEnteredEvent;

	private void OnTriggerEnter(Collider other) {
		if (tag == string.Empty || other.CompareTag(tag)) {
			triggerEntered?.Invoke();
			triggerEnteredEvent?.Invoke();
		}
	}
}
