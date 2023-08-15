using System;
using System.Collections.Generic;
using UnityEngine;

public class VRKeyboard : MonoBehaviour {
	public CanvasKeyboardKey[] row1;
	public CanvasKeyboardKey[] row2;
	public CanvasKeyboardKey[] row3;
	public CanvasKeyboardKey[] row4;
	public CanvasKeyboardKey[] row5;
	List<CanvasKeyboardKey[]> rows = new List<CanvasKeyboardKey[]>();
	public float rowHeight = 60;
	public float rowPadding = 10;
	public bool capsLock = false;
	public bool shift = false;
	public Action<CanvasKeyboardKey> keyPressed;


	public void KeyPress(CanvasKeyboardKey key) {
		if (key.normalLabel == "Shift") {
			shift = true;
			SetShift();
		} else if (key.normalLabel == "Caps Lock") {
			capsLock = !capsLock;
			SetShift();
		} else {
			keyPressed?.Invoke(key);
			shift = false;
			SetShift();
		}
	}

	void Start() {
		rows.Add(row1);
		rows.Add(row2);
		rows.Add(row3);
		rows.Add(row4);
		rows.Add(row5);
		float currentY = 0;
		for (int r = 0; r < rows.Count; r++) {
			CanvasKeyboardKey[] row = rows[r];
			float currentX = 0;
			for (int i = 0; i < row.Length; i++) {

				CanvasKeyboardKey key = row[i];
				key.keyboard = this;
				key.transform.localPosition = Vector3.zero;
				RectTransform rect = key.GetComponent<RectTransform>();
				Vector2 bl = new Vector2(currentX + key.leftPadding, currentY);
				Vector2 tr = new Vector2(currentX + key.leftPadding + key.rightPadding + key.width, currentY + rowHeight - rowPadding);
				rect.anchorMin = new Vector2(0, 0);
				rect.anchorMax = new Vector2(0, 0);
				rect.offsetMin = bl;
				rect.offsetMax = tr;
				currentX += key.width + key.leftPadding + key.rightPadding;
			}
			currentY += rowHeight;
		}
	}

	void SetShift() {
		for (int r = 0; r < rows.Count; r++) {
			CanvasKeyboardKey[] row = rows[r];

			for (int i = 0; i < row.Length; i++) {

				CanvasKeyboardKey key = row[i];
				key.setShift(capsLock || shift);

			}
		}
	}
}
