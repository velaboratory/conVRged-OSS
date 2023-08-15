using System;
using System.Collections.Generic;
using System.Linq;
using conVRged;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using unityutilities;
#if VUPLEX
using Vuplex.WebView;
#endif

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public SaveScene sceneSaver;
	private SceneSettings sceneSettings;
	private bool sceneSettingsDirty = true;
	public string lastLaunchedVersion = "";

	public SceneSettings SceneSettings
	{
		get
		{
			if (sceneSettingsDirty)
			{
				if (sceneSettings == null)
				{
					sceneSettings = FindObjectOfType<SceneSettings>();
				}

				if (sceneSettings == null)
				{
					sceneSettings = new GameObject("Scene Settings (default)", typeof(SceneSettings)).GetComponent<SceneSettings>();
				}

				sceneSettingsDirty = false;
			}

			return sceneSettings;
		}
	}

	public enum PlayerPrefabType
	{
		Oculus,
		MouseAndKeyboard,
		XR,
		PicoNeo2
	}

	public PlayerPrefabType whichPlayerPrefabEditor;
	public PlayerPrefabType whichPlayerPrefabBuild;

	public PlayerPrefabType WhichPlayerPrefab
	{
		get
		{
#if UNITY_EDITOR
			return whichPlayerPrefabEditor;
#else
			return whichPlayerPrefabBuild;
#endif
		}
	}


	[ReadOnly] public VELPlayer player;
	public VELPlayer[] availableVelPlayers;


	private void Awake()
	{
		instance = this;

		// get launch arg (to force into 2d mode)
		List<string> args = Environment.GetCommandLineArgs().ToList();
		if (args.Contains("-2dmode"))
		{
			whichPlayerPrefabBuild = PlayerPrefabType.MouseAndKeyboard;
		}

		XRSettings.enabled = WhichPlayerPrefab != PlayerPrefabType.MouseAndKeyboard;

		foreach (VELPlayer prefab in availableVelPlayers)
		{
			if (prefab.prefabType == WhichPlayerPrefab)
			{
				player = prefab;
				prefab.gameObject.SetActive(true);
			}
			else
			{
				prefab.gameObject.SetActive(false);
			}
		}


		SceneManager.sceneLoaded += SceneLoaded;

#if VUPLEX
		// Web.SetAutoplayEnabled(true);
		Web.SetUserAgent(false);
		Web.SetCameraAndMicrophoneEnabled(true);
		// Web.ClearAllData();
#endif

		OVRManager.useDynamicFixedFoveatedRendering = true;
		// Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(72);
		// Unity.XR.Oculus.Utils.SetFoveationLevel(4);
		// Unity.XR.Oculus.Utils.EnableDynamicFFR(true);

		// OVRPlugin.useDynamicFixedFoveatedRendering = true;
		// OVRPlugin.fixedFoveatedRenderingLevel = OVRPlugin.FixedFoveatedRenderingLevel.HighTop;
		// if (OVRPlugin.systemDisplayFrequenciesAvailable.Contains(90))
		// {
		// 	OVRPlugin.systemDisplayFrequency = 90;
		// }


		lastLaunchedVersion = PlayerPrefs.GetString("LastLaunchedVersion", "");
		PlayerPrefs.SetString("LastLaunchedVersion", Application.version);
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		sceneSettingsDirty = true;
	}
}