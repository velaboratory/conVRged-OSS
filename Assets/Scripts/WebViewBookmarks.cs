#if VUPLEX
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WebViewBookmarks : MonoBehaviour
{
	public string fetchURL = "https://";
	public Transform container;
	public GameObject buttonPrefab;
	public TVScreen tv;

	private void Start()
	{
		RefreshBookmarks();
	}

	public void RefreshBookmarks()
	{
		StartCoroutine(DoRefresh());
	}

	private IEnumerator DoRefresh()
	{
		using (UnityWebRequest www = UnityWebRequest.Get(fetchURL))
		{
			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log($"Error for downloading bookmarks \n{www.error}");
			}
			else
			{
				Debug.Log("Downloaded bookmarks");
				JToken resp = JsonConvert.DeserializeObject<JToken>(www.downloadHandler.text);

				// clear the old entries
				foreach (Transform child in container)
				{
					Destroy(child.gameObject);
				}

				foreach (JToken entry in resp)
				{
					GameObject buttonObj = Instantiate(buttonPrefab, container);
					Button button = buttonObj.GetComponent<Button>();
					button.onClick.AddListener(() => { SetTVURL(entry); });
					TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
					text.text = entry["display_name"]?.ToString();
				}
			}
		}
	}

	private void SetTVURL(JToken entry)
	{
		tv.SetUrl(entry["url"]?.ToString());
	}
}
#endif