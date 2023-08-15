using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSettings : MonoBehaviour
{
	public enum PasswordMode
	{
		None, // 0 by default - always multiplayer
		HeadsetSerial, // 4 digits generated from serial number
		PlayerPrefsMultiplayer, // default is 0
		PlayerPrefsSinglePlayer, // default is based on headset serial if no value saved
		PlayerPrefsMultiplayerGlobal, // default is 0. Used for all scenes
		PlayerPrefsSinglePlayerGlobal, // default is based on headset serial if no value saved. Used for all scenes. This has kinda weird behavior quirks. Probably don't use.
	}

#if OCULUS_INTEGRATION
	public OVRManager.FixedFoveatedRenderingLevel ffrLevel = OVRManager.FixedFoveatedRenderingLevel.Off;
	public bool useDynamicFFR = true;
#else
	public int ffrLevel = 0;
#endif
	[Range(.5f, 2f)] public float resolutionScale = 1.2f;
	public PasswordMode passwordMode = PasswordMode.PlayerPrefsMultiplayerGlobal;
	public bool autoSaveOnExit = false;
	public bool autoLoadOnEnter = false;
	public bool useVideoChat = false;


	public static void SetLevelPassword(string password, string sceneName)
	{
		PlayerPrefs.SetString("LevelPassword_" + sceneName, password);
		PlayerPrefs.SetString("LevelPassword", password);
	}

	public static void SetLevelPassword(string password)
	{
		SetLevelPassword(password, SceneManager.GetActiveScene().name);
	}

	/// <summary>
	/// Either generates the password from serial, loads it from playerprefs, or sets it to 0
	/// </summary>
	/// <returns>The scene password</returns>
	public static string GetLevelPassword()
	{
		PasswordMode mode = PasswordMode.HeadsetSerial;
		if (GameManager.instance.SceneSettings != null)
		{
			mode = GameManager.instance.SceneSettings.passwordMode;
		}

		switch (mode)
		{
			case PasswordMode.None:
				return "0";
			case PasswordMode.HeadsetSerial:
				return Generate4DigitUniqueSystemNumber();
			case PasswordMode.PlayerPrefsMultiplayer:
				return PlayerPrefs.GetString("LevelPassword_" + SceneManager.GetActiveScene().name, "0");
			case PasswordMode.PlayerPrefsSinglePlayer:
				return PlayerPrefs.GetString("LevelPassword_" + SceneManager.GetActiveScene().name, Generate4DigitUniqueSystemNumber());
			case PasswordMode.PlayerPrefsMultiplayerGlobal:
				return PlayerPrefs.GetString("LevelPassword", "0");
			case PasswordMode.PlayerPrefsSinglePlayerGlobal:
				return PlayerPrefs.GetString("LevelPassword", Generate4DigitUniqueSystemNumber());
			default:
				Debug.LogError("Fix this");
				return "0";
		}
	}


	private static string Generate4DigitUniqueSystemNumber()
	{
		string serialNum = SystemInfo.deviceUniqueIdentifier;
		if (serialNum.Length <= 4) return "0000";
		int sum = serialNum.Aggregate(0, (current, item) => current + item);
		return sum.ToString();
	}
}