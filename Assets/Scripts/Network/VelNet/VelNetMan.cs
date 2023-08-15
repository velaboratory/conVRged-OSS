using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using conVRged;
using Dissonance;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using unityutilities;
using VELConnect;
using VelNet;

/// <summary>
/// conVRged's main manager for velnet
/// </summary>
public class VelNetMan : MonoBehaviour
{
	#region Variables

	public GameObject playerPrefab;
	private NetworkObject playerPrefabReference;

	public static Dictionary<int, VelNetWolf3DAvatarController> playerPrefabs = new Dictionary<int, VelNetWolf3DAvatarController>();


	// Nickname-
	public static Action<string> NickNameChanged;

	/// <summary>
	/// Will be empty string if not connected. Should never be null
	/// </summary>
	public static string NickName
	{
		get
		{
			int key = VelNetManager.LocalPlayer?.userid ?? -1;
			return playerPrefabs.TryGetValue(key, out VelNetWolf3DAvatarController prefab) ? prefab != null ? prefab.nickName : "" : "";
		}
		set
		{
			if (VelNetManager.LocalPlayer != null)
			{
				VelNetWolf3DAvatarController player = playerPrefabs[VelNetManager.LocalPlayer.userid];
				if (player != null)
				{
					player.nickName = value;
				}
			}

			NickNameChanged?.Invoke(value);
		}
	}

	// Avatar
	/// <summary>
	/// If our local avatar changed. This also activates to refresh the avatar if the url didn't change
	/// </summary>
	public static Action<string> AvatarURLChanged;

	private static bool avatarLoadedFirstTime;

	/// <summary>
	/// The backing value for this property is velconnect
	/// </summary>
	public static void SetAvatarURL(string value, bool forceReload)
	{
		// if the url is the same and force is not required
		if (!forceReload && avatarLoadedFirstTime && GetAvatarURL() == value) return;
		avatarLoadedFirstTime = true;

		VELConnectManager.SetDeviceData(new Dictionary<string, string>
		{
			{ "avatar_url", value }
		});

		try
		{
			AvatarURLChanged?.Invoke(value);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}

	/// <summary>
	/// The backing value for this property is velconnect
	/// </summary>
	public static string GetAvatarURL()
	{
		// white texture
		return VELConnectManager.GetDeviceData("avatar_url", "");

		// bald red shirt guy
		return VELConnectManager.GetDeviceData("avatar_url", "https://d1a370nemizbjq.cloudfront.net/4c824209-dacc-47d1-b974-30958e12313e.glb");
	}

	public static Action<NetworkObject> OnPlayerPrefabInstantiated;

	private bool hadVoipPerms;
	public DissonanceComms voipRecorder;

	public enum MicrophoneSetting
	{
		On,
		PushToTalk,
		PushToMute,
		Off
	}

	public static MicrophoneSetting microphoneSetting;

	#endregion

	// Start is called before the first frame update
	private void Start()
	{
		VelNetManager.OnConnectedToServer += () => { VelNetManager.Login($"convrged_{Application.version}", Hash128.Compute(SystemInfo.deviceUniqueIdentifier).ToString()); };
		VelNetManager.OnLoggedIn += () =>
		{
			if (SceneMan.inLoadingScene)
			{
				SceneMan.LoadScene(SceneMan.InitialScene);
			}
			else
			{
				VelNetManager.Join(SceneManager.GetActiveScene().name + "_" + SceneSettings.GetLevelPassword());
			}
		};

		SceneManager.sceneLoaded += (scene, mode) =>
		{
			if (mode == LoadSceneMode.Additive) return;
			if (SceneMan.inLoadingScene)
			{
				Debug.LogError("Got sceneLoaded for loading scene. Don't load the loading scene again.");
				return;
			}

			VelNetManager.Join(scene.name + "_" + SceneSettings.GetLevelPassword());
		};

		VelNetManager.OnJoinedRoom += roomId =>
		{
			Debug.Log("Joined VelNet Room: " + roomId);
			playerPrefabReference = VelNetManager.NetworkInstantiate(playerPrefab.name);

			playerPrefabReference.GetComponent<Wolf3DAvatar>().AvatarFinishedLoading += () =>
			{
				// resetting microphone needs to be delayed because of stupid magic
				StartCoroutine(WaitForSeconds(1f, () => { FindObjectOfType<DissonanceComms>().ResetMicrophoneCapture(); }));
				GameManager.instance.player.movement.FadeIn(2);
			};

			StartCoroutine(WaitOneFrame(() =>
			{
				NickName = VELConnectManager.GetDeviceData("nickname", Environment.UserName);

				try
				{
					OnPlayerPrefabInstantiated?.Invoke(playerPrefabReference);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}));

			VelNetManager.GetRoomData(roomId);
		};
		VelNetManager.OnLeftRoom += roomId =>
		{
			Debug.Log("Left VelNet Room: " + roomId);
			if (playerPrefabReference != null) VelNetManager.NetworkDestroy(playerPrefabReference);
		};


		VelNetManager.OnPlayerLeft += player => { Resources.UnloadUnusedAssets(); };

#if UNITY_ANDROID && !UNITY_EDITOR
		hadVoipPerms = Permission.HasUserAuthorizedPermission(Permission.Microphone);
		if (!hadVoipPerms)
		{
			Permission.RequestUserPermission(Permission.Microphone);
		}
#endif

		microphoneSetting = (MicrophoneSetting)PlayerPrefs.GetInt("MicrophoneSetting", 0);
	}

	private void Update()
	{
		voipRecorder.IsMuted = microphoneSetting switch
		{
			MicrophoneSetting.On => false,
			MicrophoneSetting.PushToTalk => !InputMan.Button1(),
			MicrophoneSetting.PushToMute => InputMan.Button1(),
			MicrophoneSetting.Off => true,
			_ => voipRecorder.IsMuted
		};
	}

	private void OnApplicationFocus_BAK(bool focus)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (focus) {
			if (!hadVoipPerms && Permission.HasUserAuthorizedPermission(Permission.Microphone))
			{
				hadVoipPerms = true;
				Debug.Log("Restarting Recording - android");
				// voipRecorder.RestartRecording();
			}
			Reconnect();
		} else {
			//PhotonNetwork.Disconnect();
		}
#endif
	}


	private void OnApplicationPause_BAK(bool pause)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		if (!pause) {
			Reconnect();
		}
#endif
	}

	/// <summary>
	/// Connects to server if we are currently not connected.
	/// </summary>
	public void Reconnect()
	{
		// TODO
	}

	private IEnumerator WaitOneFrame(Action action)
	{
		yield return null;
		yield return null;
		action();
	}

	private IEnumerator WaitForSeconds(float seconds, Action action)
	{
		yield return new WaitForSeconds(seconds);
		action();
	}

	public static VelNetWolf3DAvatarController GetPlayerPrefab(VelNetPlayer player)
	{
		int key = player?.userid ?? -1;
		return playerPrefabs.ContainsKey(key) ? playerPrefabs[key] : null;
	}

	public static VelNetWolf3DAvatarController GetLocalPlayerPrefab()
	{
		int key = VelNetManager.LocalPlayer?.userid ?? -1;
		return playerPrefabs.ContainsKey(key) ? playerPrefabs[key] : null;
	}
}