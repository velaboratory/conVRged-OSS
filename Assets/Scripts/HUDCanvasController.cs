using Dissonance;
using UnityEngine;
using UnityEngine.UI;

public class HUDCanvasController : MonoBehaviour
{
	public Toggle microphoneMuted;
	public Slider microphoneOutputLevel;
	private DissonanceComms comms;

	// Start is called before the first frame update
	private void Start()
	{
		comms = FindObjectOfType<DissonanceComms>();
	}

	// Update is called once per frame
	private void Update()
	{
		microphoneMuted.isOn = comms.IsMuted;
		microphoneOutputLevel.value = comms.FindPlayer(comms.LocalPlayerName)?.Amplitude ?? 0;
	}
}