using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class EnableObjectsBasedOnScene : MonoBehaviour {
	
	[System.Serializable]
	public class ObjAndScenePair {
		public Transform obj;
		public string sceneName;
	}

	[FormerlySerializedAs("objectsByBuildIndex")] public ObjAndScenePair[] objectsBySceneName;

	private void Start() {
		SceneManager.activeSceneChanged += SceneChanged;
		SceneChanged(new Scene(), SceneManager.GetActiveScene());
	}

	private void SceneChanged(Scene oldScene, Scene newScene) {
		foreach (ObjAndScenePair obj in objectsBySceneName) {
			obj.obj.gameObject.SetActive(obj.sceneName == newScene.name);
		}
	}

	private void OnEnable()
	{
		SceneChanged(new Scene(), SceneManager.GetActiveScene());
	}

	private void OnDisable()
	{
		SceneChanged(new Scene(), SceneManager.GetActiveScene());
	}
}
