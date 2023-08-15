#if VUPLEX
using UnityEngine;
using VelNet;
using Vuplex.WebView;

/// <summary>
/// TODO I don't like this being separate from the url syncing
/// </summary>
public class TVScreen : NetworkComponent
{
	public CanvasWebViewPrefab webViewPrefab;

	public void SetUrl(string url)
	{
		networkObject.TakeOwnership();
		webViewPrefab.WebView?.LoadUrl(url);
	}

	public override void ReceiveBytes(byte[] message)
	{
		throw new System.NotImplementedException();
	}
}
#endif