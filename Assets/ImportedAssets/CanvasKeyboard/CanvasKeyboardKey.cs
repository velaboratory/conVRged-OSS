using System;
using UnityEngine;
using UnityEngine.UI;
public class CanvasKeyboardKey : MonoBehaviour {
	public float width; //all keys are the same height
	public float leftPadding;
	public float rightPadding;
	public string normalLabel;
	public string shiftLabel;
	public Text altText;
	public Text mainText;
	public bool shift;
	public VRKeyboard keyboard;
	
	// Start is called before the first frame update
	private void Start() {
		altText.text = shiftLabel;
		mainText.text = normalLabel;
		setShift(false);
	}

	public void setShift(bool shift) {
		this.shift = shift;
		if (normalLabel == shiftLabel) {
			altText.gameObject.SetActive(false);
		}
		else {

			if (shift) {
				altText.gameObject.SetActive(false);
				mainText.text = shiftLabel;
			}
			else {
				altText.gameObject.SetActive(true);
				altText.text = shiftLabel;
				mainText.text = normalLabel;
			}
		}
	}

	public string getValue() {
		if (!shift) {
			return normalLabel;
		}
		else {
			return shiftLabel;
		}
	}


	public void onDown() {
		if (keyboard != null) {
			keyboard.KeyPress(this);
		}
	}

	public string Type(string text, Action enterAction = null)
	{
		string letter = shift ? shiftLabel : normalLabel;
		switch (letter)
		{
			case "Backspace":
			{
				if (text.Length > 0)
				{
					text = text[..^1];
				}

				break;
			}
			case "Enter":
				enterAction?.Invoke();
				break;
			default:
				text += letter;
				break;
		}

		return text;
	}
}
