using UnityEngine;

public class SceneUIMan : MonoBehaviour
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
