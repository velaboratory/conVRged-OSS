#if VUPLEX
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class NateWebviewAvatarCreator : MonoBehaviour
{
	public CanvasWebViewPrefab webView;
	private string lastLastUrl;
	private string lastUrl;
	public Button setAvatarButton;

	private IEnumerator Start()
	{
		while (true)
		{
			//Task.Run(async () =>
			//{
			//	string url = await webView.WebView.ExecuteJavaScript(";");
			//	//string url = await webView.WebView.ExecuteJavaScript("document.getElementById('avatarGlbUrl').value;");
			//	// can set lastUrl to null if the box isn't up
			//	//lastUrl = url;
			//});

			webView.WebView?.ExecuteJavaScript(@"
urlField = document.getElementById('avatarGlbUrl');
if (urlField !== null) {
	urlField.value;
}", (ret) =>
			{
				lastUrl = ret;
			});


			webView.WebView?.ExecuteJavaScript(@"
emailButton = document.getElementById('skip-email');
if (emailButton !== null) {
	emailButton.click();
}");

			if (lastUrl != lastLastUrl)
			{
				// only if the url has been changed to something non-null
				// and new
				// and is an https url
				if (lastUrl != null &&
				    Uri.TryCreate(lastUrl, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps &&
				    lastUrl.EndsWith(".glb"))
				{
					if (setAvatarButton != null)
					{
						setAvatarButton.onClick.RemoveAllListeners();
						setAvatarButton.onClick.AddListener(() =>
						{
							VelNetMan.SetAvatarURL(lastUrl, true);
							setAvatarButton.gameObject.SetActive(false);
						});
						setAvatarButton.gameObject.SetActive(true);
					}
					else
					{
						VelNetMan.SetAvatarURL(lastUrl, true);
					}
				}
				else
				{
					if (setAvatarButton != null) setAvatarButton.gameObject.SetActive(false);
				}
			}

			lastLastUrl = lastUrl;

			yield return new WaitForSeconds(.5f);
		}
	}
}
#endif