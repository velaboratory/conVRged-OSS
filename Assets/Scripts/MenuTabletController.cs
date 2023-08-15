using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using conVRged;
using Dissonance;
using MenuTablet;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using unityutilities;
using VelNet;
using Logger = unityutilities.Logger;
#if VUPLEX
using Vuplex.WebView;
#endif
using unityutilities.Interaction.WorldMouse;

/// <summary>
/// 📑
/// </summary>
public class MenuTabletController : MonoBehaviour
{
	public Transform numpad;
	public Text numpadLevelName;
	public InputField numpadValue;
	public InputField passwordField;
	public Selectable passwordFieldButton;
	public Transform levelSelectButtons;
	public Text currentStatusText;
	public Text totalUsers;
	public Text roomUsers;
	public Text roomUsersText;
	public Transform quitButton;
	public RectTransform slideHideObj;
	public Transform browser;
#if VUPLEX
	public CanvasWebViewPrefab browserPrefab;
	private IWebView browserView;
	public CanvasWebViewPrefab presenterPrefab;
	private IWebView presenterView;
#endif
	public Transform presenter;
	private string LastURL;
	public TMP_Text vdoNinjaCode;
	public TMP_Text currentServerLabel;
	public GameObject shareButton;
	private VELShareManager velShareManager;
	public float timeBetweenUserCountUpdates = 5f;
	private float userCountUpdateTimer;
	public GameObject armLengthMeasuringObject;

	private void Start()
	{
		UpdatePasswordField();

#if VUPLEX
		browserPrefab.InitialUrl = "https://ugeorgia.ca1.qualtrics.com/jfe/form/SV_a2xepSUD4Vs7cou?id=" + SystemInfo.deviceUniqueIdentifier;
		browserPrefab.Initialized += BrowserGrabWebView;
		presenterPrefab.Initialized += PresenterGrabWebView;
#endif

		MenuTabletMover.OnShow += (_) =>
		{
			WorldMouseInputModule.FindCanvases();

			userCountUpdateTimer = 100;

			UpdatePasswordField();

			List<string> data = new List<string>
			{
				"show",
				"-1",
				"",
				"toggle_tablet"
			};
			Logger.LogRow("grab_events", data);
		};

		MenuTabletMover.OnHide += (_) =>
		{
			HideNumpad();

			UpdatePasswordField();

			List<string> data = new List<string>
			{
				"hide",
				"-1",
				"",
				"toggle_tablet"
			};
			Logger.LogRow("grab_events", data);
		};

		vdoNinjaCode.text = "Code: " + VelNetWolf3DAvatarController.HeadsetShortCode;
	}

	// Update is called once per frame
	private void Update()
	{
		if (userCountUpdateTimer > timeBetweenUserCountUpdates)
		{
			userCountUpdateTimer = 0;
			UpdateOnlineStatus();
			UpdatePasswordFieldInteractable();
		}

		userCountUpdateTimer += Time.deltaTime;


		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			MenuTabletMover.ToggleTablet();
			WorldMouseInputModule.FindCanvases();
		}

#if VUPLEX
		if (CombinedTVController.instance != null && CombinedTVController.instance.localWebView.WebView != null && presenterPrefab.WebView != null)
		{
			string newURL = CombinedTVController.instance.localWebView.WebView.Url;
			if (LastURL != newURL)
			{
				presenterPrefab.WebView.LoadUrl(newURL);
			}

			LastURL = newURL;
		}
#endif

		// TODO this is kinda slow, esp if there is no velsharemanager in the scene
		if (velShareManager == null)
		{
			velShareManager = FindObjectOfType<VELShareManager>();
		}

		shareButton.SetActive(velShareManager != null && !velShareManager.networkObject.IsMine);
	}

	private void OnEnable()
	{
		//Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable()
	{
		//Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		// tv = FindObjectOfType<TVController>();
	}

	private void UpdatePasswordFieldInteractable()
	{
		SceneSettings.PasswordMode mode = GameManager.instance.SceneSettings.passwordMode;

		if (mode is SceneSettings.PasswordMode.HeadsetSerial or SceneSettings.PasswordMode.None)
		{
			passwordField.interactable = false;
			passwordFieldButton.interactable = false;
		}
		else
		{
			passwordField.interactable = true;
			passwordFieldButton.interactable = true;
		}
	}


	/// <summary>
	/// Not used yet
	/// </summary>
	/// <param name="val">The value from the input field</param>
	public void OnNumpadValueChanged(string val)
	{
		int.TryParse(numpadValue.text, out int password);
		roomUsersText.text = "Room " + password + ":";
	}

	private void UpdateOnlineStatus()
	{
		if (VelNetManager.IsConnected)
		{
			if (VelNetManager.InRoom)
			{
				currentStatusText.text = "Currently Online\nin room: " + VelNetManager.Room;
				roomUsers.text = VelNetManager.PlayerCount.ToString();
			}
			else
			{
				currentStatusText.text = "Currently Online";
				roomUsers.text = "-";
			}

			VelNetManager.GetRooms(rooms => { totalUsers.text = rooms.rooms.Sum(r => r.numUsers).ToString(); });
		}
		else
		{
			currentStatusText.text = "Currently Offline";
			roomUsers.text = "-";
			totalUsers.text = "-";
		}
	}

	public void UpdatePasswordField()
	{
		passwordField.text = SceneSettings.GetLevelPassword();
		currentServerLabel.text = $"Server {SceneSettings.GetLevelPassword()}";
	}


	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	/// <summary>
	/// Closes the numpad and loads the desired scene
	/// </summary>
	public void NumpadFinish()
	{
		string password = numpadValue.text;
		SceneSettings.SetLevelPassword(password);
		UpdatePasswordField();
		HideNumpad();
		LoadLevel(SceneManager.GetActiveScene().name);
	}

	/// <summary>
	/// Shows the numpad to enter a password
	/// </summary>
	/// <param name="levelName"></param>
	public void ShowNumpad(string levelName)
	{
		numpad.gameObject.SetActive(true);
		levelSelectButtons.gameObject.SetActive(false);
		quitButton.gameObject.SetActive(false);
		numpadLevelName.text = levelName;
		numpadValue.text = "";
	}

	/// <summary>
	/// Hides the numpad without loading the scene
	/// </summary>
	public void HideNumpad()
	{
		numpad.gameObject.SetActive(false);
		levelSelectButtons.gameObject.SetActive(true);
		quitButton.gameObject.SetActive(true);
	}

	/// <summary>
	/// Toggles the state of the browser
	/// </summary>
	public void ToggleBrowser()
	{
		browser.gameObject.SetActive(!browser.gameObject.activeSelf);
	}

#if VUPLEX
	public void BrowserGrabWebView(object sender, System.EventArgs e)
	{
		browserView = browserPrefab.WebView;
	}

	public void PresenterGrabWebView(object sender, System.EventArgs e)
	{
		presenterView = presenterPrefab.WebView;
	}
#endif

	public void EnhanceBrowser()
	{
	}

	public void DeEnhanceBrowser()
	{
	}

	public void EnhancePresenter()
	{
	}

	public void DeEnhancePresenter()
	{
	}

	public void TVNetworkedPress(string key)
	{
#if VUPLEX
		if (CombinedTVController.instance != null)
		{
			CombinedTVController.instance.PressKey(key);
		}
#endif
	}

	public void TogglePresenter()
	{
		presenter.gameObject.SetActive(!presenter.gameObject.activeSelf);
	}

	/// <summary>
	/// Loads a desired scene by name
	/// </summary>
	/// <param name="level">String name of the scene</param>
	public void LoadLevel(string level)
	{
		MenuTabletMover.ShowTablet(false);
		SceneMan.LoadScene(level);
	}

	/// <summary>
	/// For clicking number buttons on the numpad
	/// </summary>
	/// <param name="number">The value to add</param>
	public void EnterNumber(int number)
	{
		numpadValue.text += number;
	}

	/// <summary>
	/// The backspace key on the numpad
	/// </summary>
	public void Backspace()
	{
		if (numpadValue.text.Length > 0)
			numpadValue.text = numpadValue.text.Substring(0, numpadValue.text.Length - 1);
	}

	public void Undo()
	{
		GameManager.instance.sceneSaver.Undo();
	}

	public void ReloadMic()
	{
		FindObjectOfType<DissonanceComms>().ResetMicrophoneCapture();
	}

	public void ShareScreen()
	{
		FindObjectOfType<VELShareManager>()?.networkObject.TakeOwnership();
	}

	public void ToggleBecomeMonke()
	{
		if (GameManager.instance.WhichPlayerPrefab == GameManager.PlayerPrefabType.MouseAndKeyboard) return;

		Rigidbody rb = GameManager.instance.player.rig.rb;
		// become monke
		if (!rb.useGravity)
		{
			rb.useGravity = true;
			rb.drag = 0;
			rb.mass = 30;
			// rb.transform.Translate(0, 1, 0);

			GorillaLocomotion.Player monke = rb.GetComponent<GorillaLocomotion.Player>();

			// monke.leftHandFollower.transform.position = GameManager.instance.player.rig.leftHand.position;
			// monke.rightHandFollower.transform.position = GameManager.instance.player.rig.rightHand.position;

			monke.enabled = true;
			monke.InitializeValues();
			monke.bodyCollider.gameObject.SetActive(true);
		}
		// become human
		else
		{
			rb.useGravity = false;
			rb.drag = 10;
			rb.mass = 1;
			// rb.transform.Translate(0, 0, 0);
			GorillaLocomotion.Player monke = rb.GetComponent<GorillaLocomotion.Player>();
			monke.enabled = false;
			monke.bodyCollider.gameObject.SetActive(false);
			rb.transform.Translate(0, 1, 0);
		}
	}

	public void ToggleVideoPanelVisible(bool visible)
	{
		if (VelNetMan.GetLocalPlayerPrefab() != null)
		{
			VelNetMan.GetLocalPlayerPrefab().videoPanelVisible = visible;
		}
	}

	public void SetMicrophoneSetting(int typeIndex)
	{
		VelNetMan.microphoneSetting = (VelNetMan.MicrophoneSetting)typeIndex;
	}


	public void StartArmLengthCalibration()
	{
		armLengthMeasuringObject.transform.position = GameManager.instance.player.rig.head.position +
		                                              GameManager.instance.player.rig.head.forward * .5f;
		armLengthMeasuringObject.SetActive(true);

		MenuTabletMover.ShowTablet(false);
	}

	public void StopArmLengthCalibration()
	{
		StartCoroutine(StopArmLengthCalibrationCo());
	}

	private IEnumerator StopArmLengthCalibrationCo()
	{
		yield return new WaitForSeconds(1);
		float armLength = Vector3.Distance(GameManager.instance.player.rig.head.position, armLengthMeasuringObject.transform.position);
		if (armLength is > .2f and < 2)
		{
			armLengthMeasuringObject.SetActive(false);
			GameManager.instance.player.ArmLength = armLength - .15f;
			Logger.LogRow("arm-length-calib", armLength.ToString(CultureInfo.InvariantCulture), "true");
		}
		else
		{
			Logger.LogRow("arm-length-calib", armLength.ToString(CultureInfo.InvariantCulture), "false");
		}
	}

	public void SpawnCrowd()
	{
		AudienceSpawner audienceSpawner = FindObjectOfType<AudienceSpawner>();
		if (audienceSpawner)
		{
			audienceSpawner.SpawnAudience();
		}
	}
}