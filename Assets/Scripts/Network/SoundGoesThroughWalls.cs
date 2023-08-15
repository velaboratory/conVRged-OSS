using Dissonance;
using UnityEngine;

public class SoundGoesThroughWalls : MonoBehaviour
{
	private DissonanceComms comms;
	private int roundRobinIndex = 0;

	private void Start()
	{
		comms = FindObjectOfType<DissonanceComms>();
	}

	private void Update()
	{
		// we use a round robin here so this algo scales perfectly with number of users
		// we would just have less frequent updates with more users, but no more cpu usage
		if (roundRobinIndex >= comms.Players.Count) roundRobinIndex = 0;

		bool obscured = false;
		Vector3 ourPosition = GameManager.instance.player.rig.head.position;
		VoicePlayerState p = comms.Players[roundRobinIndex];
		if (!p.IsLocalPlayer)
		{
			Vector3 position = p.Tracker?.Position ?? Vector3.zero;

			Debug.DrawLine(ourPosition, position);
			if (Physics.Raycast(ourPosition, position - ourPosition, Vector3.Distance(ourPosition, position), LayerMask.GetMask("Ground")))
			{
				obscured = true;
			}

			p.Volume = obscured ? .2f : 1f;
		}

		roundRobinIndex++;
	}
}