using UnityEngine;

public class EnableDisableWithKey : MonoBehaviour
{
	public KeyCode key;
	public GameObject[] objs;

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown(key))
		{
			foreach (GameObject obj in objs)
			{
				obj.gameObject.SetActive(!obj.gameObject.activeSelf);
			}
		}
	}
}
