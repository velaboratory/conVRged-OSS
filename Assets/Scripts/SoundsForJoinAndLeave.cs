using UnityEngine;
using VelNet;

public class SoundsForJoinAndLeave : MonoBehaviour
{
	public AudioClip joinClip;
	public AudioClip leaveClip;
	private AudioSource source;

	private void Start()
	{
		VelNetManager.OnPlayerJoined += (_, alreadyInRoom) =>
		{
			if (alreadyInRoom) return;
			if (joinClip == null) return;

			MoveToSpawnLoc();

			if (source == null) source = FindObjectOfType<AudioSource>();
			source.PlayOneShot(joinClip);
		};

		VelNetManager.OnPlayerLeft += _ =>
		{
			if (leaveClip == null) return;
			MoveToSpawnLoc();
			if (source == null) source = FindObjectOfType<AudioSource>();
			source.PlayOneShot(leaveClip);
		};
	}

	private void MoveToSpawnLoc()
	{
		GameObject spawnLoc = GameObject.FindWithTag("spawn_location");
		if (spawnLoc != null)
		{
			transform.position = spawnLoc.transform.position;
		}
	}
}