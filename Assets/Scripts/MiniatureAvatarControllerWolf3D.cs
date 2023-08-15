using UnityEngine;
using UnityEngine.UI;
using VelNet;

public class MiniatureAvatarControllerWolf3D : MonoBehaviour
{
	public Wolf3DAvatar avatar;

	public Transform nameTagObject;
	public Text nameTagText;

	public float nametagHeight = .66f;
	private string nickName;
	public Transform avatarPos;
	public bool maintainPos;

	
	private void OnEnable()
	{
		VelNetMan.NickNameChanged += UpdateNickName;
		VelNetMan.AvatarURLChanged += UpdateAvatar;
		VelNetMan.OnPlayerPrefabInstantiated += _ =>
		{
			UpdateAvatar(VelNetMan.GetAvatarURL());
			UpdateNickName(VelNetMan.NickName);
		};
	}

	private void OnDisable()
	{
		VelNetMan.NickNameChanged -= UpdateNickName;
		VelNetMan.AvatarURLChanged -= UpdateAvatar;
	}

	private void UpdateAvatar(string url)
	{
		avatar.SetAvatarURL(url);
	}

	private void UpdateNickName(string newNickName)
	{
		if (nickName != newNickName)
		{
			nameTagText.text = newNickName;
		}

		nickName = newNickName;
	}

	private void Update()
	{
		// if the avatar has been initialized
		if (avatar.BodyPart.Transform == null) return;

		// update the nametag position
		nameTagObject.position = avatar.BodyPart.Transform.position + Vector3.up * (nametagHeight * transform.localScale.x);
		nameTagObject.LookAt(GameManager.instance.player.rig.head);
		nameTagObject.Rotate(0, 180, 0, Space.Self);
	}
}