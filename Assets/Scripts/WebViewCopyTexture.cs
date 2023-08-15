#if VUPLEX
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuplex.WebView;

public class WebViewCopyTexture : MonoBehaviour
{
	public CanvasWebViewPrefab canvasWebView;
	public RawImage thisImage;

	public bool getFromTVController;

	// Start is called before the first frame update
	private IEnumerator Start()
	{
		if (getFromTVController)
		{
			StartCoroutine(LoadFromTV());
			SceneManager.sceneLoaded += (scene, mode) => { StartCoroutine(LoadFromTV()); };
		}
		else
		{
			while (canvasWebView.WebView != null && !canvasWebView.WebView.IsInitialized)
			{
				yield return null;
			}

			while (canvasWebView.WebView?.Texture == null) yield return null;

			thisImage.material = CombinedTVController.instance.localWebView.Material;

			thisImage.material = canvasWebView.Material;
			thisImage.texture = canvasWebView.WebView.Texture;
		}
	}

	private void OnEnable()
	{
		if (getFromTVController)
		{
			StartCoroutine(LoadFromTV());
		}
	}

	private IEnumerator LoadFromTV()
	{
		yield return null;
		if (CombinedTVController.instance == null) yield break;
		while (!CombinedTVController.instance.localWebView.WebView.IsInitialized)
		{
			yield return null;
		}
		while (CombinedTVController.instance.localWebView.WebView?.Texture == null) yield return null;
		thisImage.material = CombinedTVController.instance != null ? CombinedTVController.instance.localWebView.Material : null;
	}
}
#endif