#if VUPLEX
using System;
using UnityEngine;
using Vuplex.WebView;

public class WebviewMirrorOtherWebview : MonoBehaviour
{
	public CanvasWebViewPrefab thisWebView;
	public CanvasWebViewPrefab otherWebView;
	private string thisLastURL;
	private string otherLastURL;

	public float updateRateHz = 1;
	private float nextTime = 0;

	// Update is called once per frame
	private void Update()
	{
		if (!(Time.time > nextTime)) return;
		while (nextTime < Time.time)
		{
			nextTime += 1f/updateRateHz;
		}


		if (otherWebView == null || otherWebView.WebView == null ||
		    !(thisWebView != null & thisWebView.WebView != null)) return;


		// if the other webview changed, change this one
		string otherNewURL = otherWebView.WebView.Url;
		if (otherLastURL != otherNewURL)
		{
			thisWebView.WebView.LoadUrl(otherNewURL);
			otherLastURL = otherNewURL;
			thisLastURL = otherNewURL;
			Debug.Log("Copied from main");
			return;
		}

		// if this webview changed, change the other one.
		string thisNewURL = thisWebView.WebView.Url;
		if (thisLastURL != thisNewURL)
		{
			otherWebView.WebView.LoadUrl(thisNewURL);
			thisLastURL = thisNewURL;
			otherLastURL = thisNewURL;
			Debug.Log("Copied to main");
			return;
		}
	}
}
#endif