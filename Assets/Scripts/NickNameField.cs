using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VelNet;
using Logger = unityutilities.Logger;

public class NickNameField : MonoBehaviour
{
	public VRKeyboard keyboard;
	public Text text;
	public Action nameFinished;

	private void Start()
	{
		keyboard.keyPressed += KeyPress;

		text.text = VelNetMan.NickName;
	}

	private void SetTextCallback(string newName)
	{
		// fill in our current nickname on scene load
		if (newName != null && !newName.StartsWith("Player"))
		{
			text.text = newName;
		}
	}

	private void OnEnable()
	{
		VelNetMan.NickNameChanged += SetTextCallback;
	}

	private void OnDisable()
	{
		VelNetMan.NickNameChanged -= SetTextCallback;
	}

	private void KeyPress(CanvasKeyboardKey key)
	{
		string letter = key.shift ? key.shiftLabel : key.normalLabel;
		if (letter == "Backspace")
		{
			if (text.text.Length > 0)
			{
				text.text = text.text[..^1];
			}
		}
		else if (letter == "Enter")
		{
			nameFinished?.Invoke();
		}
		else
		{
			text.text += letter;
		}

		VelNetMan.NickName = text.text;

		List<string> data = new List<string>
		{
			letter,
			"-1",
			"",
			"keyboard_press"
		};
		Logger.LogRow("grab_events", data);
	}
}