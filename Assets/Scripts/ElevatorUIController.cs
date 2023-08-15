using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUIController : MonoBehaviour
{
	public ElevatorDoorOpener doorOpener;
	public InputField numpadValue;

	public void LoadLevel(string levelName)
	{
		StartCoroutine(LoadLevelCo(levelName));
	}

	private IEnumerator LoadLevelCo(string levelName)
	{
		string password = numpadValue.text;
		// SceneSettings.SetLevelPassword(password, levelName);
		doorOpener.CloseDoor();
		yield return new WaitForSeconds(1);
		GameManager.instance.player.movement.FadeOut(1);
		yield return new WaitForSeconds(1);
		SceneMan.LoadScene(levelName);
	}

	/// <summary>
	/// For clicking number buttons on the numpad
	/// </summary>
	/// <param name="number">The value to add</param>
	public void EnterNumber(int number)
	{
		numpadValue.text += number;
	}

	/// <summary>
	/// The backspace key on the numpad
	/// </summary>
	public void Backspace()
	{
		if (numpadValue.text.Length > 0)
		{
			numpadValue.text = numpadValue.text[..^1];
		}
	}
}