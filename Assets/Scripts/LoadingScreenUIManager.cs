using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using unityutilities;
using VelNet;

public class LoadingScreenUIManager : MonoBehaviour
{
	public Text[] texts;

	private IEnumerator Start()
	{
		VelNetManager.OnConnectedToServer += () =>
		{
			SetAllTexts("Loading...");
		};
		
		yield return null;
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			SetAllTexts("Could not connect to the internet.\nPress A/B/X/Y to try again.");

			while (!VelNetManager.IsConnected)
			{
				yield return null;
				if (InputMan.Button1Down() || InputMan.Button2Down())
				{
					// reload the current scene
					SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
				}
			}
		}
	}

	private void SetAllTexts(string text)
	{
		foreach (Text item in texts)
		{
			item.text = text;
		}
	}
}