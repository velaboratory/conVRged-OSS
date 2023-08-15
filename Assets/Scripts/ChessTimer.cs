using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VelNet;

public class ChessTimer : MonoBehaviour
{
	public UndoGroup undoGroup;
	public TimerWithLogging timerWhite;
	public TimerWithLogging timerBlack;

	public void ButtonClickWhite()
	{
		if (timerWhite.IsRunning) return;

		// TODO check if valid chess state

		timerBlack.StopTimer();
		timerWhite.StartTimer();

		undoGroup.SaveUndoState();
	}
	public void ButtonClickBlack()
	{
		if (timerBlack.IsRunning) return;

		// TODO check if valid chess state

		timerWhite.StopTimer();
		timerBlack.StartTimer();

		undoGroup.SaveUndoState();
	}
}