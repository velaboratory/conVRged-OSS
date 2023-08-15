#if VUPLEX
using UnityEngine;
using Vuplex.WebView;

public class RefreshButton : MonoBehaviour
{
	public CanvasWebViewPrefab webViewPrefab;

	public void Refresh()
	{
#if UNITY_STANDALONE || (UNITY_EDITOR && !UNITY_EDITOR_LINUX)
		// this won't actually work. It needs to be in Awake or destroy the WebViews first
		Web.ClearAllData();
		StandaloneWebView.ClearAllData();
#else
		// for android/ios
		Web.ClearAllData();
#endif
		webViewPrefab.WebView.Reload();
	}
}
#endif