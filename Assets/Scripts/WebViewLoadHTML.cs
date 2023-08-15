#if VUPLEX
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

public class WebViewLoadHTML : MonoBehaviour
{
    public string html;
    public CanvasWebViewPrefab webView;
    
    // Start is called before the first frame update
    private void Start()
    {
        webView.Initialized += (o, e) =>
        {
            webView.WebView.LoadHtml(html);
        };
    }
}
#endif