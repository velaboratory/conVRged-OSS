using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VELConnect;
using VelNet;

namespace Network
{
	public class VelConnectConvrged : MonoBehaviour
	{
		// public static VelConnectConvrged instance;

		public Material carpetMaterial;
		private Color originalColor;

		public static Action<string> OnStreamerStreamIdChanged;
		public static Action<string> OnStreamerControlIdChanged;

		#region Unity Callbacks

		private void Awake()
		{
			// if (instance != null) Debug.LogError("VelConnectConvrged instance not null", this);
			// instance = this;
		}

		private void Start()
		{
			originalColor = carpetMaterial.color;

			VelNetMan.AvatarURLChanged += SetHeadsetAvatarUrl;
			// SetHeadsetAvatarUrl(VelNetMan.GetAvatarURL());

			VelNetMan.NickNameChanged += nickName =>
			{
				VELConnectManager.SetDeviceData(new Dictionary<string, string>
				{
					{ "nickname", nickName }
				});
			};

			VELConnectManager.AddDeviceDataListener("join_room_request_" + Application.productName, this, value =>
			{
				if (!string.IsNullOrEmpty(value))
				{
					StartCoroutine(WaitForLeaveLoadingScene(value));
				}
			}, true);

			VELConnectManager.AddDeviceFieldListener("current_room", this, value =>
			{
				// changed room
				if (!string.IsNullOrEmpty(value))
				{
					if (value != VelNetManager.Room)
					{
						SceneSettings.SetLevelPassword(value.Split('_').Last());
						SceneMan.LoadScene(SceneManager.GetActiveScene().name);
						Debug.Log("[VELCONNECT] Changed room");
					}
				}
			});
			VELConnectManager.AddDeviceDataListener("nickname", this, value =>
			{
				if (value == null || value == "null")
				{
					value = Environment.UserName;
				}

				VelNetMan.NickName = value;

				Debug.Log("[VELCONNECT] Set player name");
			}, true);

			VELConnectManager.AddDeviceDataListener("avatar_url", this, value =>
			{
				// if the string is a valid url
				if (Uri.TryCreate(value, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps)
				{
					VelNetMan.SetAvatarURL(value, true);
					Debug.Log("[VELCONNECT] Set player avatar");
				}
				else
				{
					Debug.Log("Failed to parse player avatar url");
				}
			}, true);
			VELConnectManager.AddDeviceDataListener("streamer_stream_id", this, value =>
			{
				try
				{
					OnStreamerStreamIdChanged?.Invoke(value);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}, true);
			VELConnectManager.AddDeviceDataListener("streamer_control_id", this, value =>
			{
				try
				{
					OnStreamerControlIdChanged?.Invoke(value);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}, true);
			VELConnectManager.AddRoomDataListener("tv_url", this, value =>
			{
#if VUPLEX
				if (!string.IsNullOrWhiteSpace(value) && CombinedTVController.instance != null && CombinedTVController.instance.urlInputText.text != value)
				{
					CombinedTVController.instance.SetURL(value, false);
					Debug.Log($"[VELCONNECT] Set TV URL");
				}
#endif
			}, true);
			VELConnectManager.AddRoomDataListener("carpet_color", this, value =>
			{
				if (ColorUtility.TryParseHtmlString(value, out Color color))
				{
					carpetMaterial.color = color;
					Debug.Log("[VELCONNECT] Set carpet color");
				}
				else
				{
					Debug.Log("Failed to parse server carpet color");
				}
			}, true);
		}

		private IEnumerator WaitForLeaveLoadingScene(string targetRoom)
		{
			while (SceneManager.GetActiveScene().name == "_Loading" || string.IsNullOrEmpty(VelNetManager.Room))
			{
				yield return null;
			}

			if (targetRoom != VelNetManager.Room)
			{
				SceneSettings.SetLevelPassword(targetRoom.Split('_').Last());
				SceneMan.LoadScene(targetRoom.Split('_').First());
				Debug.Log("[VELCONNECT] Changed to requested room");

				// reset the request so we don't join the same room later
				VELConnectManager.SetDeviceData(new Dictionary<string, string>()
				{
					{ "join_room_request_" + Application.productName, string.Empty }
				});
			}
		}


		private void OnApplicationQuit()
		{
			carpetMaterial.color = originalColor;
		}

		#endregion

		private static void SetHeadsetAvatarUrl(string value)
		{
			VELConnectManager.SetDeviceData(new Dictionary<string, string>
			{
				{ "avatar_url", value }
			});
		}
	}
}