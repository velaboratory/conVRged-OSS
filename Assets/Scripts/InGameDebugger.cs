using UnityEngine;
using UnityEngine.UI;

public class InGameDebugger : MonoBehaviour
{
	public Transform parent;
	public Text countText;
	public int count = 0;
	public GameObject countObj;
	public GameObject textPrefab;
	public bool allowDebugs;

	// Start is called before the first frame update
	private void Start()
	{
		Application.logMessageReceived += LogMessage;
	}

	private void LogMessage(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && (!allowDebugs || type != LogType.Log)) return;

		if (type != LogType.Log)
		{
			countObj.SetActive(true);
			count++;
			countText.text = count + " Errors";
		}

		if (count <= 5)
		{
			GameObject newObj = Instantiate(textPrefab, parent);
			newObj.GetComponent<Text>().text = condition + stackTrace;
			newObj.transform.SetSiblingIndex(0);
		}
	}
}