#if VUPLEX
using System.IO;
using VelNet;
using Vuplex.WebView;

public class URLSyncer : SyncState
{
	public CanvasWebViewPrefab webViewPrefab;

	protected override void SendState(BinaryWriter writer)
	{
		writer.Write(webViewPrefab.WebView?.Url ?? "");
	}

	protected override void ReceiveState(BinaryReader reader)
	{
		string newUrl = reader.ReadString();
		if (webViewPrefab.WebView == null) return;
		string currentUrl = webViewPrefab.WebView?.Url;

		if (string.IsNullOrEmpty(newUrl) || currentUrl == newUrl) return;

		if (currentUrl != null)
		{
			// exception for neko password redirect
			if (currentUrl.Contains("neko.ugavel.com") && newUrl.Contains("neko.ugavel.com")) return;

			// if the ? is just replaced with a #, don't sync
			string currentUrlCut = currentUrl.Replace("present?slide", "present#slide");
			string newUrlCut = newUrl.Replace("present?slide", "present#slide");

			if (newUrlCut == currentUrlCut) return;
		}

		webViewPrefab.WebView?.LoadUrl(newUrl);
	}
}
#endif