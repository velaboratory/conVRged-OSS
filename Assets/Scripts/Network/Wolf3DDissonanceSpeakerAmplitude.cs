using Dissonance;
using UnityEngine;
using VelNet.Dissonance;

public class Wolf3DDissonanceSpeakerAmplitude : Wolf3DSpeakerAmplitude
{
	private const int AmplitudeMultiplier = 100;
	private float lastAmplitude;
	private float lastAmplitudeTime;
	private VoicePlayerState state;
	public VelNetDissonancePlayer dissonancePlayer;
	public float timeBetweenMeasurements = .1f;

	public override float GetAmplitude()
	{
		// cache the amplitude calculation
		if (Time.time - lastAmplitudeTime < timeBetweenMeasurements)
		{
			return lastAmplitude;
		}

		state ??= FindObjectOfType<DissonanceComms>().FindPlayer(dissonancePlayer.PlayerId);

		if (state == null) return 0;

		// float amplitude = Mathf.Max(Mathf.Clamp(Mathf.Pow(state.Amplitude, 0.175f), 0.25f, 1), _intensity - Time.unscaledDeltaTime);
		float amplitude = state.Amplitude * AmplitudeMultiplier;
		lastAmplitude = amplitude;
		lastAmplitudeTime = Time.time;
		return amplitude;
	}
}