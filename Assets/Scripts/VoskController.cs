using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yetibyte.Unity.SpeechRecognition;

public class VoskController : MonoBehaviour
{
	public static VoskController instance;
	public VoskListener voskListener;
	public string currentText;

	private void Awake()
	{
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		voskListener.PartialResultFound += PartialResultFound;
		voskListener.ResultFound += ResultFound;
	}

	private void ResultFound(object sender, VoskResultEventArgs e)
	{
		currentText = e.Result.Text;
	}

	private void PartialResultFound(object sender, VoskPartialResultEventArgs e)
	{
		currentText = e.PartialResult.Text;
	}
}