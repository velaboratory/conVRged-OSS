using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneListUI : MonoBehaviour
{
	public GameObject buttonPrefab;
	public float buttonHeight;
	private List<GameObject> buttonList = new List<GameObject>();

	private void Awake()
	{
		int count = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		string[] scenes = new string[count];
		for (int i = 0; i < count; i++)
		{
			scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));

			if (i > 1)
			{
				// add a button to the list
				GameObject g = Instantiate(buttonPrefab, transform);
				g.transform.localPosition = new Vector3(0, -i * buttonHeight + (buttonHeight * 2), 0);
				buttonList.Add(g);
				Text text = g.GetComponentInChildren<Text>();
				if (text != null) text.text = scenes[i];
				string sceneName = scenes[i];
				g.GetComponent<Button>().onClick.AddListener(() => { SceneMan.LoadScene(sceneName); });
			}
		}
	}
}