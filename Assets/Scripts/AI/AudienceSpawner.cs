using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VelNet;
using Random = UnityEngine.Random;

public class AudienceSpawner : NetworkObject
{
	public Transform[] seats;
	private List<SyncRPMe> spawnedAvatars = new List<SyncRPMe>();

	public GameObject nonRandomAvatarPrefab;
	public GameObject randomAvatarPrefab;
	public Transform spawnSurface;
	public Transform speakerPosition;

	private bool initialized;


	private struct AudienceRoomData
	{
		public string audienceAvatars;
		public float audiencePositionVariation;
		public float audienceRotationVariation;
		public List<Vector3> audiencePositionsList;
		public bool audienceRandomAvatars;
	}

	private AudienceRoomData roomData = new AudienceRoomData()
	{
		audienceAvatars = "https://models.readyplayer.me/6456989b4900918462ba1dcc.glb\nhttps://models.readyplayer.me/64569c57703eae1cea262ed9.glb\nhttps://models.readyplayer.me/64569ce264cdd18c593dca09.glb\nhttps://models.readyplayer.me/64569cef043c3d76766d4fa4.glb\nhttps://models.readyplayer.me/64569d02ff55712fd09e77a6.glb\nhttps://models.readyplayer.me/64569d104900918462ba215c.glb\nhttps://models.readyplayer.me/64569d1e043c3d76766d4fc5.glb\nhttps://models.readyplayer.me/64569d2c4715767b7a9d97ad.glb",
		audiencePositionVariation = 0.2f,
		audienceRotationVariation = 10f,
		audienceRandomAvatars = false,
	};

	private IEnumerator Start()
	{
		VELConnect.VELConnectManager.AddRoomDataListener($"audienceAvatars", this, s =>
		{
			roomData.audienceAvatars = s;
			SpawnAudience();
		});
		VELConnect.VELConnectManager.AddRoomDataListener($"audiencePositionVariation", this, s =>
		{
			roomData.audiencePositionVariation = float.Parse(s);
			SpawnAudience();
		});
		VELConnect.VELConnectManager.AddRoomDataListener($"audienceRandomAvatars", this, s =>
		{
			roomData.audienceRandomAvatars = bool.Parse(s);
			SpawnAudience();
		});
		VELConnect.VELConnectManager.AddRoomDataListener($"audienceRotationVariation", this, s =>
		{
			roomData.audienceRotationVariation = float.Parse(s);
			SpawnAudience();
		});
		VELConnect.VELConnectManager.AddRoomDataListener($"audiencePositionsList", this, s =>
		{
			roomData.audiencePositionsList = s.Split('\n').Select(line =>
			{
				string[] parts = line.Split(',');
				return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
			}).ToList();
			SpawnAudience();
		});

		yield return new WaitForSeconds(2);
		initialized = true;
		SpawnAudience();
	}

	public void SpawnAudience()
	{
		if (!initialized) return;
		if (!IsMine) return;
		if (roomData.audiencePositionsList == null) return;

		foreach (SyncRPMe spawnedAvatar in spawnedAvatars)
		{
			spawnedAvatar.Destroy();
		}

		spawnedAvatars.Clear();

		string[] avatarList = roomData.audienceAvatars.Split("\n");

		Vector3 offsetPos = Vector3.zero;
		foreach (Vector3 pos in roomData.audiencePositionsList)
		{
			offsetPos.x = pos.x;
			offsetPos.y = pos.y;
			offsetPos.z = pos.z;
			Quaternion rot = Quaternion.LookRotation(speakerPosition.position - pos);
			rot = Quaternion.AngleAxis(Random.Range(-roomData.audienceRotationVariation, roomData.audienceRotationVariation), Vector3.up) * rot;
			// pos.x += Random.Range(-roomData.audiencePositionVariation, roomData.audiencePositionVariation);

			// find the floor
			{
				// pre-allocate
				Vector3 origin = Vector3.zero;
				Vector3 direction = -Vector3.up;
				for (float h = -10; h < 10; h += 1)
				{
					origin.x = offsetPos.x;
					origin.y = h;
					origin.z = offsetPos.z;
					if (Physics.Raycast(origin, direction, out RaycastHit hit))
					{
						offsetPos.y = hit.point.y + .5f;
						break;
					}
				}
			}
			offsetPos.y += Random.Range(-roomData.audiencePositionVariation, roomData.audiencePositionVariation);
			// pos.z += Random.Range(-roomData.audiencePositionVariation, roomData.audiencePositionVariation);
			SpawnAvatar(offsetPos, rot, roomData.audienceRandomAvatars ? avatarList[Random.Range(0, avatarList.Length)] : "");
		}
	}

	private void SpawnAvatar(Vector3 position, Quaternion rotation, string url)
	{
		GameObject obj = VelNetManager.NetworkInstantiate(randomAvatarPrefab.name, position, rotation).gameObject;
		obj.transform.parent = transform;
		SyncRPMe rpme = obj.GetComponent<SyncRPMe>();
		rpme.Initialize(url, speakerPosition);

		spawnedAvatars.Add(rpme);
	}

	/// <summary>
	/// https://stackoverflow.com/a/1262619
	/// </summary>
	/// <param name="list"></param>
	/// <typeparam name="T"></typeparam>
	public static void Shuffle<T>(IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n);
			(list[k], list[n]) = (list[n], list[k]);
		}
	}
}