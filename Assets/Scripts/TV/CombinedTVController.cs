#if VUPLEX
using System;
using Vuplex.WebView;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using unityutilities;
using VelNet;
using Logger = unityutilities.Logger;
using unityutilities.Interaction.WorldMouse;

public class CombinedTVController : SyncState
{
	public bool useSFU = true;
	private bool lastUseSFU;

	[Space] public GameObject localOnlyObjects;

	public GameObject remoteOnlyObjects;

	public CanvasWebViewPrefab localWebView;

	public CanvasWebViewPrefab sfuOutputWebView;

	// public WebRTCManager webRtcManager;
	public Selectable urlInput;
	public TMP_Text urlInputText;
	public VRKeyboard vrKeyboard;
	private string lastUrl;
	public bool isGlobalInstance;
	public static CombinedTVController instance;
	public bool presenting = false;

	public GameObject[] useSFUObjects;
	public GameObject[] noSFUObjects;

	[ReadOnly] public bool sendInputToURLBar;

	protected override void Awake()
	{
		base.Awake();

		if (isGlobalInstance) instance = this;

		networkObject.OwnershipChanged += player =>
		{
			if (!player.isLocal)
			{
				presenting = false;
			}

			UpdatePresenting();
		};

		UpdatePresenting();
	}

	private void UpdatePresenting()
	{
		// show if we are presenting or not using SFU
		localOnlyObjects.gameObject.SetActive(networkObject.IsMine && presenting || !useSFU);
		remoteOnlyObjects.SetActive(!networkObject.IsMine || !presenting);

		foreach (GameObject obj in useSFUObjects)
		{
			obj.SetActive(useSFU);
		}

		foreach (GameObject obj in noSFUObjects)
		{
			obj.SetActive(!useSFU);
		}

		lastUseSFU = useSFU;
		WorldMouseInputModule.FindCanvases();
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(useSFU);
		if (!useSFU)
		{
			binaryWriter.Write(lastUrl);
		}
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		useSFU = binaryReader.ReadBoolean();
		if (!useSFU)
		{
			string newUrl = binaryReader.ReadString();
			if (newUrl != lastUrl)
			{
				SetURL(newUrl, false);
				lastUrl = newUrl;
			}
		}
	}

	public void OnEnable()
	{
		vrKeyboard.keyPressed += KeyPressed;
		StartCoroutine(DelayedFindWorldMouse());

		VelNetManager.OnJoinedRoom += OnJoinedRoom;
	}

	private void OnJoinedRoom(string roomName)
	{
		// webRtcManager.streamRoom = "convrged_" + roomName.ToLower();
		StartCoroutine(WaitForWebView(sfuOutputWebView, () => { sfuOutputWebView.WebView.LoadUrl("https://kjjohnsen.github.io/test_sfu/?room=convrged_" + roomName.ToLower()); }));

		UpdatePresenting();
	}

	private IEnumerator WaitForWebView(BaseWebViewPrefab webViewPrefab, Action callback)
	{
		for (int i = 0; i < 1000; i++)
		{
			if (webViewPrefab.WebView != null)
			{
				callback();
				yield break;
			}

			yield return null;
		}
	}

	public void OnDisable()
	{
		vrKeyboard.keyPressed -= KeyPressed;
		// WorldMouseInputModule.Instance.worldMice.ForEach(m =>
		// {
		// 	m.ClickUp -= AnyWorldMouseClicked;
		// });

		VelNetManager.OnJoinedRoom -= OnJoinedRoom;
	}

	private IEnumerator DelayedFindWorldMouse()
	{
		yield return null;
		// WorldMouseInputModule.Instance.worldMice.ForEach(m =>
		// {
		// 	m.ClickUp += AnyWorldMouseClicked;
		// });
	}


	private void AnyWorldMouseClicked(GameObject obj)
	{
		if (obj == localWebView.gameObject)
		{
			if (!IsMine) VelNetManager.TakeOwnership(networkObject.networkId);
			sendInputToURLBar = false;
		}

		if (obj == urlInput.gameObject)
		{
			sendInputToURLBar = true;
		}
	}

	private void KeyPressed(CanvasKeyboardKey key)
	{
		if (sendInputToURLBar)
		{
			string letter = key.shift ? key.shiftLabel : key.normalLabel;
			switch (letter)
			{
				case "Backspace":
				{
					if (urlInputText.text.Length > 0)
					{
						urlInputText.text = urlInputText.text[..^1];
					}

					break;
				}
				case "Enter":
					InputFieldGo();
					break;
				default:
					urlInputText.text += letter;
					break;
			}

			List<string> data = new List<string>
			{
				letter,
				"-1",
				"",
				"keyboard_press"
			};
			Logger.LogRow("grab_events", data);
		}
		else
		{
			PressKey(key.shift ? key.shiftLabel : key.normalLabel);
		}

		VelNetManager.TakeOwnership(networkObject.networkId);
	}

	private void Update()
	{
		if (localWebView.WebView != null)
		{
			string newURL = localWebView.WebView.Url;
			if (lastUrl != newURL)
			{
				WebViewUrlChanged(newURL);
			}

			lastUrl = newURL;
		}

		if (lastUseSFU != useSFU)
		{
			UpdatePresenting();
		}
	}

	private void WebViewUrlChanged(string newURL)
	{
		urlInputText.text = newURL;
	}

	/// <summary>
	/// Sets the url of the TV
	/// </summary>
	/// <param name="url">Must be a valid http/s url</param>
	/// <param name="setOwnership">If this is false, the url will only be changed if there is no owner set.</param>
	public void SetURL(string url, bool setOwnership = true)
	{
		if (setOwnership)
		{
			VelNetManager.TakeOwnership(networkObject.networkId);
		}

		urlInputText.text = url;
		localWebView.WebView?.LoadUrl(url);
	}

	public void InputFieldGo()
	{
		localWebView.WebView.LoadUrl(urlInputText.text);
	}

	public void InputFieldClear()
	{
		urlInputText.text = "";
		sendInputToURLBar = true;
	}

	/// <summary>
	/// Presses a key with a name as found in:
	/// https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key/Key_Values
	/// </summary>
	/// <param name="keyName"></param>
	public void PressKey(string keyName)
	{
		VelNetManager.TakeOwnership(networkObject.networkId);

		// TODO maybe sync keys idk
		// photonView.RPC(nameof(PressKeyRPC), RpcTarget.All, keyName);

		StartCoroutine(PressKeyCo(keyName));
	}

	private IEnumerator PressKeyCo(string keyName)
	{
		if (localWebView.WebView is not IWithKeyDownAndUp keyThing) yield break;
		keyThing.KeyDown(keyName, KeyModifier.None);
		yield return null;
		keyThing.KeyUp(keyName, KeyModifier.None);
	}

	public void StopPresenting()
	{
		presenting = false;
		UpdatePresenting();
	}

	public void StartPresenting()
	{
		presenting = true;
		networkObject.TakeOwnership();
	}
}
#endif