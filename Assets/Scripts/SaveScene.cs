using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using unityutilities;
using unityutilities.VRInteraction;
using VelNet;
using Logger = unityutilities.Logger;


#if UNITY_EDITOR
[CustomEditor(typeof(SaveScene))]
public class SaveSceneEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var ss = target as SaveScene;
		if (ss == null) return;

		DrawDefaultInspector();

		EditorGUILayout.Space();

		if (GUILayout.Button("Save"))
		{
			ss.Save();
		}

		if (GUILayout.Button("Load"))
		{
			ss.Load();
		}
	}
}
#endif

public class SaveScene : MonoBehaviour
{
	public enum Source { local, remote }
	public Source source;
	public string webPostURL = "https://";
	public string webGetURL = "https://";
	public string webLogPassword;
	public string appName = "";
	private const string passField = "password";
	private const string fileField = "file";
	private const string dataField = "data";
	private const string appField = "app";

	/// <summary>
	/// sceneName, List of saves
	/// </summary>
	private Dictionary<string, List<string>> saveHistory = new Dictionary<string, List<string>>();

	/// <summary>
	/// int: viewId
	/// </summary>
	public Dictionary<string, NetworkObject> networkObjects;

	/// <summary>
	/// not used
	/// int: some unique identifier
	/// </summary>
	public Dictionary<int, INetworkPack> packablesDict;

	public List<INetworkPack> packables;

	/// <summary>
	/// A scene file. Serialized to JSON for saving.
	/// </summary>
	[Serializable]
	public class SceneFile
	{
		[Serializable]
		public class NetworkPackData
		{
			/// <summary>
			/// Instance id of the MonoBehavior
			/// </summary>
			public int id;

			public string name;
			public byte[] data;

			public NetworkPackData(string name, byte[] data)
			{
				this.name = name;
				this.data = data;
			}
		}

		/// <summary>
		/// Full name of the save
		/// </summary>
		public string saveName;
		public string sceneName;
		public List<NetworkPackData> networkPackData;
	}

	public void SaveDefaultScene()
	{
		FindPackables();
		SceneFile file = new SceneFile();
		file.sceneName = SceneManager.GetActiveScene().name;
		file.saveName = file.sceneName + "_default";

		// deal with VRGrabbables
		file.networkPackData = new List<SceneFile.NetworkPackData>();
		foreach (var item in packables)
		{
			file.networkPackData.Add(new SceneFile.NetworkPackData((item as MonoBehaviour).name, item.PackData()));
		}

		string data = JsonUtility.ToJson(file);
		StartCoroutine(Upload(file.saveName + ".sav", data));
	}

	/// <summary>
	/// Saves the current scene to a file
	/// </summary>
	/// <param name="onlyInternal">Just save this to an internal variable. (good for undo)</param>
	public void Save(bool onlyInternal = false)
	{
		FindPackables();

		string sceneName = SceneManager.GetActiveScene().name;
		SceneFile file = new SceneFile
		{
			sceneName = sceneName,
			saveName = sceneName + "_" + SceneSettings.GetLevelPassword()
		};

		// deal with VRGrabbables
		file.networkPackData = new List<SceneFile.NetworkPackData>();
		foreach (var item in packables)
		{
			file.networkPackData.Add(new SceneFile.NetworkPackData((item as MonoBehaviour).name, item.PackData()));
		}

		string data = JsonUtility.ToJson(file);

		if (!onlyInternal)
		{
			if (source == Source.local)
			{
				StreamWriter writer = new StreamWriter(
					Path.Combine(Application.persistentDataPath, file.saveName + ".sav"));
				writer.Write(data);
				writer.Close();
			}
			else
			{
				StartCoroutine(Upload(file.saveName + ".sav", data));
			}
		}

		// Save internally
		if (!saveHistory.ContainsKey(file.saveName))
		{
			saveHistory.Add(file.saveName, new List<string> { data });
		}
		else
		{
			saveHistory[file.saveName].Add(data);
		}
	}

	/// <summary>
	/// Loads a save "num" saves ago
	/// </summary>
	public void Undo()
	{
		string fileName = SceneManager.GetActiveScene().name + "_" + SceneSettings.GetLevelPassword();

		if (saveHistory.ContainsKey(fileName) && saveHistory[fileName].Count > 0)
		{
			saveHistory[fileName].RemoveAt(saveHistory[fileName].Count - 1);
			Load(saveHistory[fileName][saveHistory[fileName].Count - 1]);
		}

		List<string> data = new List<string> {
			"",
			"-1",
			"",
			"undo"
		};
		Logger.LogRow("grab_events", data);
	}

	/// <summary>
	/// Loads a scene file from disk/remote and applies it to the current scene
	/// </summary>
	public void Load()
	{

		// read file
		string fileName = SceneManager.GetActiveScene().name + "_" + SceneSettings.GetLevelPassword() + ".sav";
		if (source == Source.local)
		{
			string data;
			string filePath = Path.Combine(Application.persistentDataPath, fileName);

			StreamReader streamReader = new StreamReader(filePath);
			data = streamReader.ReadToEnd();
			streamReader.Close();
			Load(data);
		}
		else
		{
			StartCoroutine(Download(fileName));
		}
	}

	public void Load(string data)
	{
		SceneFile file = JsonUtility.FromJson<SceneFile>(data);


		FindPhotonViews();
		FindPackables();

		// deal with photon views
		foreach (KeyValuePair<string, NetworkObject> view in networkObjects)
		{
			if (!view.Value.IsMine)
				view.Value.TakeOwnership();
		}


		// find which network objects aren't found locally yet
		// match network objects with local objects
		Dictionary<SceneFile.NetworkPackData, INetworkPack> matching = new Dictionary<SceneFile.NetworkPackData, INetworkPack>();

		// add empty dictionary slots for all the network objects
		foreach (SceneFile.NetworkPackData item in file.networkPackData)
		{
			matching.Add(item, null);
		}

		// loop through all the local objects to match them to network ones
		foreach (INetworkPack item in packables)
		{
			bool found = false;

			// find which network object it matches to
			foreach (SceneFile.NetworkPackData netPack in file.networkPackData)
			{
				// if we found the right one
				if (matching[netPack] == null && ((MonoBehaviour) item).name == netPack.name)
				{
					matching[netPack] = item;
					found = true;
					break;
				}
			}

			// couldn't find a spot for this object
			if (!found)
				Debug.LogError("Found local object that doesn't match an object in the save.", item as MonoBehaviour);
		}

		// now which network objects don't have a local one? then instantiate them if appropriate
		foreach (var item in matching)
		{
			// this object does not exist locally
			if (item.Value != null) continue;
			
			// instantiate this object locally
			if (item.Key.name.Contains("Clone"))
			{
				GameObject obj = VelNetManager.NetworkInstantiate(item.Key.name.Substring(0, item.Key.name.Length - 7)).gameObject;
				obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
				packables.Add(obj.GetComponent<INetworkPack>());
			}
			else
			{
				Debug.LogError("LOAD FAILED: This saved object doesn't exist locally, but wasn't instantiated at runtime: " + item.Key.name);
				return;
			}
		}

		// re-sort, since we added new objects
		packables.Sort((i, j) => string.Compare(((MonoBehaviour) i).name, ((MonoBehaviour) j).name, StringComparison.Ordinal));

		for (int i = 0; i < packables.Count; i++)
		{
			packables[i].UnpackData(file.networkPackData[i].data);
		}

		Debug.Log("Loaded scene: " + file.sceneName);
	}

	/// <summary>
	/// Loads a default scene file
	/// </summary>
	public void ResetScene()
	{
		// read file
		string fileName = SceneManager.GetActiveScene().name + "_default.sav";
		StartCoroutine(Download(fileName));
	}

	/// <summary>
	/// Finds valid objects with PhotonView components.
	/// Sorts them by ViewID
	/// </summary>
	private void FindPhotonViews()
	{
		networkObjects = new Dictionary<string, NetworkObject>();
		List<NetworkObject> views = new List<NetworkObject>(FindObjectsOfType<NetworkObject>());
#if OCULUS_INTEGRATION
		views.RemoveAll(i => i.GetComponent<OvrAvatar>() || i.GetComponent<PhotonMan>());
#endif
		views.ForEach(i => networkObjects.Add(i.networkId, i));
	}

	/// <summary>
	/// Finds valid objects with INetworkPack components
	/// </summary>
	private void FindPackables()
	{
		packables = new List<INetworkPack>();
		// Find all MonoBehaviours
		List<MonoBehaviour> monos = new List<MonoBehaviour>(FindObjectsOfType<MonoBehaviour>());
		// Only include MonoBehaviours that implement INetworkPack
		monos.ForEach(i => { if (i is INetworkPack pack) { packables.Add(pack); } });
		// Remove ones that shouldn't be saved
		packables = packables.Where(i => !(i is VRGrabbable {includeInSave: false})).ToList();
		// Sort
		packables.Sort((i, j) => string.Compare(((MonoBehaviour) i).name, ((MonoBehaviour) j).name, StringComparison.Ordinal));
	}

	private IEnumerator Upload(string fileName, string data)
	{
		WWWForm form = new WWWForm();
		form.AddField(passField, webLogPassword);
		form.AddField(fileField, fileName);
		form.AddField(dataField, data);
		form.AddField(appField, appName);
		using (UnityWebRequest www = UnityWebRequest.Post(webPostURL, form))
		{
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError)
			{
				Debug.Log(www.error);
			}
			else
			{
				Debug.Log("Saved scene: " + fileName);
			}
		}

	}

	private IEnumerator Download(string fileName)
	{
		using (UnityWebRequest www = UnityWebRequest.Get(webGetURL + fileName))
		{
			yield return www.SendWebRequest();
			
			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error for downloading scene " + fileName + "\n" + www.error);
			}
			else
			{
				Debug.Log("Downloaded scene: " + fileName);
				Load(www.downloadHandler.text);
			}
		}
	}
}
