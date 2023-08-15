using UnityEditor;
using UnityEngine;

public class Builder
{
	private static readonly string[] scenes = {
		"Assets/Scenes/_Loading.unity",
		"Assets/Scenes/MonkeLand (addon).unity",
		"Assets/Scenes/MiniGolf (addon).unity",
	};
	
	// This function will be called from the build process
	public static void Build()
	{
		BuildWindows();
		BuildAndroid();
	}

	public static void BuildAndroid()
	{
		string pw = Resources.Load<TextAsset>("android_keystore_pw").ToString().Trim();
		PlayerSettings.Android.keystoreName = "user.keystore";
		PlayerSettings.Android.keystorePass = pw;
		PlayerSettings.Android.keyaliasName = "vfr";
		PlayerSettings.Android.keyaliasPass = pw;
		BuildPipeline.BuildPlayer(scenes, "Builds/convrged.apk", BuildTarget.Android, BuildOptions.None);
	}

	public static void BuildWindows()
	{
		BuildPipeline.BuildPlayer(scenes, "Builds/Windows/conVRged.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
	}
}