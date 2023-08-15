using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using VelNet;
using Wolf3D.ReadyPlayerMe.AvatarSDK;
using Random = UnityEngine.Random;

public class SyncRPMe : SyncState
{
	public string url;
	public GameObject defaultAvatar;
	private GameObject avatar;
	private int index;
	private Transform lookTarget;
	private Transform head;
	private Quaternion headRot = Quaternion.identity;

	/// <summary>
	/// tells future spawns to immediately delete themselves
	/// </summary>
	private bool destroyed = false;
	/// <summary>
	/// Is currently downloading from rpme
	/// </summary>
	private bool downloading = false;

	struct PlayerData
	{
		public string url;
		public string lookTarget;
	}

	public void Initialize(string url, Transform lookTarget)
	{
		this.url = url;
		this.lookTarget = lookTarget;
		// VELConnect.VELConnectManager.AddRoomDataListener($"audience_member_{index}", this, s =>
		// {
		// 	PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(s);
		// 	if (playerData.url != null && playerData.url != url)
		// 	{
		// 		SetUrl(playerData.url);
		// 	}
		// });
		RefreshAvatar();
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(url);
		binaryWriter.Write(headRot);
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		string newUrl = binaryReader.ReadString();
		if (newUrl != url)
		{
			url = newUrl;
			RefreshAvatar();
		}

		if (head) head.localRotation = headRot;
	}

	private void RefreshAvatar()
	{
		if (string.IsNullOrEmpty(url))
		{
			GameObject a = Instantiate(defaultAvatar, transform);
			// fake the downloading of an avatar
			downloading = true;
			AvatarImportedCallback(a);
			AvatarLoadedCallback(a, null);
			// StartCoroutine(DelayInvoke(.1f, () => { AvatarLoadedCallback(a, null); }));
		}
		else
		{
			AvatarLoader avatarLoader = new AvatarLoader();
			downloading = true;
			avatarLoader.LoadAvatar(url, AvatarImportedCallback, AvatarLoadedCallback);
		}
	}

	private void Update()
	{
		if (networkObject.IsMine)
		{
			if (head != null && Vector3.Angle(transform.forward, lookTarget.position - transform.position) < 30)
			{
				head.LookAt(lookTarget.position);
				headRot = head.localRotation;
			}
		}
	}

	private static IEnumerator DelayInvoke(float seconds, Action callback)
	{
		yield return new WaitForSeconds(seconds);
		callback();
	}
	

	private void AvatarImportedCallback(GameObject newAvatar)
	{
		newAvatar.transform.parent = transform;
		newAvatar.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
	}

	private void AvatarLoadedCallback(GameObject newAvatar, AvatarMetaData metaData)
	{
		Debug.Log("Avatar Loaded");
		downloading = false;
		
		if (avatar != null) Destroy(avatar);
		avatar = newAvatar;
		
		if (destroyed)
		{
			VelNetManager.NetworkDestroy(networkObject);
			return;
		}

		Transform lHand = newAvatar.transform.Find("Armature/Hips/Spine/LeftHand");
		Transform rHand = newAvatar.transform.Find("Armature/Hips/Spine/RightHand");
		if (lHand) lHand.position -= Vector3.up * 5;
		else Debug.LogError("Failed to find avatar hands");
		if (rHand) rHand.position -= Vector3.up * 5;
		head = newAvatar.transform.Find("Armature/Hips/Spine/Neck/Head");
		if (!head)
		{
			Debug.LogError("Failed to find avatar head");
		}

		EyeAnimationHandler eyeMovement = newAvatar.GetComponent<EyeAnimationHandler>();
		if (eyeMovement == null)
		{
			eyeMovement = newAvatar.AddComponent<EyeAnimationHandler>();
		}
	}

	public void SetUrl(string newUrl)
	{
		networkObject.TakeOwnership();
		url = newUrl;
		RefreshAvatar();
	}

	/// <summary>
	/// Either destroys immediately or waits for avatar to spawn, then destroys
	/// </summary>
	public void Destroy()
	{
		destroyed = true;

		if (downloading) return;
		
		VelNetManager.NetworkDestroy(networkObject);
	}

	// private void OnDestroy()
	// {
	// 	Destroy(avatar);
	// }
}