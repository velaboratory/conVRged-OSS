using UnityEngine;
using unityutilities;

/// <summary>
/// Checks camera position only on teleport
/// </summary>
public class LODFast : MonoBehaviour
{
	private Movement m;
	public float distance;
	public GameObject[] objects;

	// Start is called before the first frame update
	private void Start()
	{
		m = GameManager.instance.player.movement;
		m.TeleportEnd += UpdateLOD;
	}

	private void UpdateLOD(Side side, float timeHeld, Vector3 vector)
	{
		foreach (GameObject t in objects)
		{
			t.SetActive(Vector3.Distance(m.transform.position, t.transform.position) < distance);
		}
	}
}