using TMPro;
using UnityEngine;
using VELConnect;

[RequireComponent(typeof(TMP_Text))]
public class ShowPairingCode : MonoBehaviour
{
	private TMP_Text text;

	private const int n = 10;

	private void Awake()
	{
		text = GetComponent<TMP_Text>();
	}

	private void Update()
	{
		// only update every nth frame
		if (Time.frameCount % n == 0)
		{
			text.text = VELConnectManager.PairingCode.ToString();
		}
	}
}