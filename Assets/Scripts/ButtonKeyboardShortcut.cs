using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonKeyboardShortcut : MonoBehaviour
{
	public KeyCode input;
	private Button elem;

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown(input))
		{
			if (elem == null)
			{
				elem = GetComponent<Button>();
			}
			elem.onClick?.Invoke();
		}
	}
}
