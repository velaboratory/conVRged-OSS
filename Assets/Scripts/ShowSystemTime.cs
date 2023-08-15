using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowSystemTime : MonoBehaviour {
	public bool time;
	public bool date;
	public bool batteryPercent;

	private float timeDiff;
	private Text text;

	private void Awake() {
		text = GetComponent<Text>();
	}

	private void Update() {
		if (timeDiff > 1) {
			if (date) {
				//text.text = DateTime.Now.ToLongTimeString() + "\n" + DateTime.Now.ToLongDateString();
			}

			if (time)
				text.text = DateTime.Now.ToString("h:mm tt");

			if (batteryPercent && Math.Abs(SystemInfo.batteryLevel - (-1)) > .01f)
				text.text = "Battery: " + SystemInfo.batteryLevel * 100 + "%";

			timeDiff = 0;
		}

		timeDiff += Time.deltaTime;
	}
}