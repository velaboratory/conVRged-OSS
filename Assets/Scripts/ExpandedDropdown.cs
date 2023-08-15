using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExpandedDropdown : MonoBehaviour
{
	public Button[] buttons;

	[Tooltip("This will be moved on the y-axis to match the active button index.")]
	public Transform activeIndicator;

	public string playerPrefsKey = "";

	public int SelectedIndex { private set; get; }

	/// <summary>
	/// Function definition for a button click event.
	/// </summary>
	[Serializable]
	public class ExpandedDropdownClickedEvent : UnityEvent<int>
	{
	}

	// Event delegates triggered on value change
	[SerializeField] private ExpandedDropdownClickedEvent selectionChanged = new ExpandedDropdownClickedEvent();

	// Start is called before the first frame update
	private IEnumerator Start()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.AddListener(() => { SetSelectedIndex(j); });
		}

		yield return null;
		
		if (!string.IsNullOrWhiteSpace(playerPrefsKey))
		{
			SetSelectedIndex(PlayerPrefs.GetInt(playerPrefsKey, 0));
		}
	}

	// private void OnEnable()
	// {
	// 	if (!string.IsNullOrWhiteSpace(playerPrefsKey))
	// 	{
	// 		SetSelectedIndex(PlayerPrefs.GetInt(playerPrefsKey, 0));
	// 	}
	// }

	public void SetSelectedIndex(int i)
	{
		SetSelectedIndex(i, false);
	}

	public void SetSelectedIndex(int i, bool bypassListeners)
	{
		SelectedIndex = i;
		Vector3 pos = activeIndicator.transform.localPosition;
		pos.y = buttons[i].transform.localPosition.y;
		activeIndicator.transform.localPosition = pos;
		if (!bypassListeners)
		{
			if (!string.IsNullOrWhiteSpace(playerPrefsKey))
			{
				PlayerPrefs.SetInt(playerPrefsKey, i);
			}

			selectionChanged?.Invoke(SelectedIndex);
		}
	}
}