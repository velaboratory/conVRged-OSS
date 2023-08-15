using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
	public void LoadLevel(string levelName)
	{
		SceneMan.LoadScene(levelName);
	}

	public void Quit()
	{
		SceneMan.Quit();
	}
}
