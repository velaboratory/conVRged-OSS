using UnityEngine;

public class DisableOnQuest : MonoBehaviour {

	public bool disableObj = true;
	public MonoBehaviour disableScript;
	private void Awake() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (disableObj) {
			gameObject.SetActive(false);
		}

		if (disableScript != null) {
			disableScript.enabled = false;
		}
		#endif
	}
}