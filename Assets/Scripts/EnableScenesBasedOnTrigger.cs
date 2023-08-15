using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableScenesBasedOnTrigger : MonoBehaviour
{
	public string sceneName;
	public bool disableOnStart = true;

	private void Start()
	{
		// if (disableOnStart)
		// {
		// 	SceneManager.UnloadSceneAsync(sceneName);
		// }
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("MainCamera")) return;
		SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("MainCamera")) return;
		SceneManager.UnloadSceneAsync(sceneName);
	}
}