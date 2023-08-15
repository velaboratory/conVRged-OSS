using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using unityutilities;
using VelNet;

public class SceneMan : MonoBehaviour
{
	public SaveScene sceneSaver;
	private Movement m;
	private static SceneMan instance;
	public string loadingScene;
	public string initialScene;
	public static bool inLoadingScene = true;
	public static string InitialScene => instance.initialScene;
	public static string sceneLoadQueued = "";

	private void Awake()
	{
		instance = this;
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void Start()
	{
		m = GameManager.instance.player.movement;
		//m.SetBlinkOpacity(1);
		//LoadScene(initialScene);
	}

	private void Update()
	{
		// for testing
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			LoadScene(NameFromIndex(1));
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			LoadScene(NameFromIndex(2));
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			LoadScene(NameFromIndex(3));
		}

		if (Input.GetKeyDown(KeyCode.R))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				sceneSaver.SaveDefaultScene();
			}
			else
			{
				sceneSaver.ResetScene();
			}
		}
	}

	private static string NameFromIndex(int buildIndex)
	{
		try
		{
			return SceneUtility.GetScenePathByBuildIndex(buildIndex).Split('/').Last().Split('.')[0];
		}
		catch
		{
			return "";
		}
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == LoadSceneMode.Additive) return;
		inLoadingScene = scene.name == loadingScene;

		StartCoroutine(SceneLoadedCo(scene.name));
	}

	private IEnumerator SceneLoadedCo(string name)
	{
		ControllerHelp.HideAllHints();

		yield return null;
		if (GameManager.instance.SceneSettings.autoLoadOnEnter)
		{
			sceneSaver.Load();
		}

		yield return new WaitForSeconds(1f);

		Resources.UnloadUnusedAssets();
	}

	public static void LoadScene(string sceneName)
	{
		instance.StartCoroutine(nameof(LoadSceneCo), sceneName);
	}

	private IEnumerator LoadSceneCo(string sceneName)
	{
		instance.m.SetBlinkOpacity(1, true);

		if (GameManager.instance.SceneSettings != null && GameManager.instance.SceneSettings.autoSaveOnExit)
		{
			sceneSaver.Save();
		}

		VelNetManager.Leave();

		// wait a tiny bit to make sure scene saver has time to save
		yield return null;
		ActuallyLoadScene(sceneName);
	}

	/// <summary>
	/// Actually loads the scene by name. Ignores Photon stuff
	/// </summary>
	/// <param name="sceneName"></param>
	private static void ActuallyLoadScene(string sceneName)
	{
		Addressables.LoadSceneAsync(sceneName);
		if (Application.CanStreamedLevelBeLoaded("sceneName"))
		{
			// SceneManager.LoadSceneAsync(sceneName);
		}
		else
		{
			// Addressables.LoadSceneAsync(sceneName);
		}
	}

	public void OpenExternApplication(string path)
	{
		System.Diagnostics.Process.Start(path);
		Quit();
	}

	public static void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	private void OnApplicationQuit()
	{
		if (GameManager.instance.SceneSettings != null && GameManager.instance.SceneSettings.autoSaveOnExit)
		{
			instance.sceneSaver.Save();
		}
	}
}