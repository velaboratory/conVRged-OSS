using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using VelNet;

public class TimerWithLogging : SyncState
{
	public Button startStopButton;
	public TMP_Text startStopButtonText;
	public TMP_Text timerText;

	[Tooltip("Whether this takes part in the shared timer state.\nCan't change this at runtime")]
	public bool useGlobalSharedTimer = true;

	public static TimerWithLogging instance;

	private readonly Stopwatch timer = new Stopwatch();
	private Stopwatch Timer => useGlobalSharedTimer ? instance.timer : timer;
	public double CurrentTime => Timer.Elapsed.TotalSeconds + offset;
	private double offset;
	public bool IsRunning => Timer.IsRunning;

	// Start is called before the first frame update
	protected override void Awake()
	{
		base.Awake();
		if (useGlobalSharedTimer && instance == null) instance = this;
		UpdateTimerText();
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(CurrentTime);
		binaryWriter.Write(Timer.IsRunning);
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		offset = binaryReader.ReadDouble() - Timer.Elapsed.TotalSeconds;
		bool isRunning = binaryReader.ReadBoolean();
		if (isRunning && !timer.IsRunning)
		{
			StartTimer();
		}
		else if (!isRunning && timer.IsRunning)
		{
			StopTimer();
		}
	}

	private void OnEnable()
	{
		UpdateTimerText();
	}

	// Update is called once per frame
	private void Update()
	{
		if (Timer.IsRunning) UpdateTimerText();
	}

	private void UpdateTimerText()
	{
		if (timerText) timerText.text = TimeSpan.FromSeconds(CurrentTime).ToString(@"mm\:ss\.fff");
		if (startStopButtonText) startStopButtonText.text = Timer.IsRunning ? "Stop" : "Start";
	}

	public void StartStopTimer()
	{
		if (Timer.IsRunning)
		{
			StopTimer();
		}
		else
		{
			StartTimer();
		}
	}


	public void StartTimer()
	{
		networkObject.TakeOwnership();
		Timer.Start();
		unityutilities.Logger.LogRow("timer", "start", CurrentTime.ToString(CultureInfo.InvariantCulture));
		UpdateTimerText();
	}

	public void StopTimer()
	{
		networkObject.TakeOwnership();
		Timer.Stop();
		unityutilities.Logger.LogRow("timer", "stop", CurrentTime.ToString(CultureInfo.InvariantCulture));
		UpdateTimerText();
	}

	public void ResetTimer()
	{
		SendRPC(nameof(ResetTimerRPC), true);
	}

	[VelNetRPC]
	private void ResetTimerRPC()
	{
		unityutilities.Logger.LogRow("timer", "reset", CurrentTime.ToString(CultureInfo.InvariantCulture));
		Timer.Reset();
		offset = 0;
		UpdateTimerText();
	}
}