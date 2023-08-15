using UnityEngine;

public class Wolf3DFollowHead : MonoBehaviour
{
	public Wolf3DAvatar avatar;

	// Update is called once per frame
	private void Update()
	{
		if (avatar != null && avatar.Head != null)
		{
			transform.position = avatar.Head.position;
		}
	}
}