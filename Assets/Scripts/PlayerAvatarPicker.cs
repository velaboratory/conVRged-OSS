using UnityEngine;

public class PlayerAvatarPicker : MonoBehaviour
{
	public void SetAvatarURL(string url)
	{
		VelNetMan.SetAvatarURL(url, true);
	}
}