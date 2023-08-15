using UnityEngine;
public class DontDestroyOnLoadOther : MonoBehaviour {
	public GameObject[] others;

	void Awake() {
		foreach (GameObject g in others) {
			DontDestroyOnLoad(g);
		}
	}
}