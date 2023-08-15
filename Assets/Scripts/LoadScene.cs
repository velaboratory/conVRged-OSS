using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public int scene = 1;

	private AsyncOperation loadingOp;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		loadingOp = SceneManager.LoadSceneAsync(scene);
	}

	private void Update()
	{
		if (loadingOp.isDone)
		{
			Destroy(gameObject);
		}
	}
}