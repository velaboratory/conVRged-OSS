using UnityEngine;

/// <summary>
/// Checks cameera position every frame
/// </summary>
public class LODSlow : MonoBehaviour
{
	public Transform[] objects;
	public float distance = 5;
	public bool useEachObjectPosition = true;

	// Update is called once per frame
	private void Update()
	{
		if (GameManager.instance == null) return;
		
		foreach (Transform obj in objects)
		{
			if (useEachObjectPosition)
			{
				obj.gameObject.SetActive(Vector3.Distance(obj.position, GameManager.instance.player.rig.head.position) < distance);
			}
			else
			{
				obj.gameObject.SetActive(Vector3.Distance(transform.position, GameManager.instance.player.rig.head.position) < distance);
			}
		}
	}
}