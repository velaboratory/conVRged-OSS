using UnityEngine;

public class ElevatorController : MonoBehaviour
{
	public ElevatorDoorOpener doorOpener;

	public float range = 1;
	private bool wasInside;

	// Update is called once per frame
	private void Update()
	{
		if (GameManager.instance == null) return;
		
		Vector3 head = GameManager.instance.player.rig.head.position;
		float distance = Vector2.Distance(new Vector2(head.x, head.z), new Vector2(transform.position.x, transform.position.z));
		if (distance < range)
		{
			if (!wasInside)
			{
				doorOpener.OpenDoor();
				wasInside = true;
			}
		}
		else if (wasInside)
		{
			doorOpener.CloseDoor();
			wasInside = false;
		}
	}
}