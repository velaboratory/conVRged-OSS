using System.IO;
using UnityEngine;
using VelNet;

public class VelNetSyncHand : SyncState
{
	public OVRSkeleton skeleton;
	public Transform[] toSync;
	private Quaternion[] targets;
	public Renderer rend;
	public bool updateRootPose;
	public bool updateRootScale;
	private Vector3 networkPose;
	private Vector3 lastNetworkPose;
	private Vector3 networkScale;
	private Vector3 lastNetworkScale;


	private void Start()
	{
		targets = new Quaternion[toSync.Length];
	}

	protected override void ReceiveState(BinaryReader reader)
	{
		bool valid = reader.ReadBoolean();
		rend.enabled = valid;

		if (valid)
		{
			if (updateRootPose)
			{
				lastNetworkPose = networkPose;
				networkPose = reader.ReadVector3();
			}

			if (updateRootScale)
			{
				lastNetworkScale = networkScale;
				networkScale = Vector3.one * reader.ReadSingle();
			}

			//Wrist
			float x = (float)reader.ReadSByte() / 100;
			float y = (float)reader.ReadSByte() / 100;
			float z = (float)reader.ReadSByte() / 100;
			float w = (float)reader.ReadSByte() / 100;
			targets[0] = new Quaternion(x, y, z, w);

			for (int i = 2; i < toSync.Length; i++)
			{
				float qX = (float)reader.ReadSByte() / 100;
				float qY = (float)reader.ReadSByte() / 100;
				float qZ = (float)reader.ReadSByte() / 100;
				float qW = (float)reader.ReadSByte() / 100;
				targets[i] = new Quaternion(qX, qY, qZ, qW);
			}
		}
	}

	protected override void SendState(BinaryWriter writer)
	{
		bool valid = skeleton != null && skeleton.IsDataHighConfidence && skeleton.IsDataValid;
		writer.Write(valid);

		if (valid)
		{
			if (updateRootPose) writer.Write(skeleton.transform.localPosition);
			if (updateRootScale) writer.Write(skeleton.transform.localScale.x);

			//Wrist
			writer.Write((sbyte)(toSync[0].rotation.x * 100));
			writer.Write((sbyte)(toSync[0].rotation.y * 100));
			writer.Write((sbyte)(toSync[0].rotation.z * 100));
			writer.Write((sbyte)(toSync[0].rotation.w * 100));

			for (int i = 2; i < toSync.Length; i++)
			{
				writer.Write((sbyte)(toSync[i].rotation.x * 100));
				writer.Write((sbyte)(toSync[i].rotation.y * 100));
				writer.Write((sbyte)(toSync[i].rotation.z * 100));
				writer.Write((sbyte)(toSync[i].rotation.w * 100));
			}
		}
	}


	// Update is called once per frame
	private void Update()
	{
		if (IsMine) // local
		{
			// need to set values from tracked hand to this networkobject
			if (skeleton != null)
			{
				if (skeleton.Bones != null && skeleton.IsDataHighConfidence && skeleton.IsDataValid)
				{
					for (int i = 0; i < toSync.Length; i++)
					{
						toSync[i].rotation = skeleton.Bones[i].Transform.rotation;
					}
				}

				rend.enabled = skeleton.IsDataHighConfidence && skeleton.IsDataValid;

				if (updateRootPose)
				{
					transform.localPosition = skeleton.transform.localPosition;
				}

				if (updateRootScale)
				{
					transform.localScale = skeleton.transform.localScale;
				}
			}
		}
		else // remote
		{
			for (int i = 0; i < targets.Length; i++)
			{
				toSync[i].rotation = Quaternion.RotateTowards(
					toSync[i].rotation,
					targets[i],
					Time.deltaTime * Quaternion.Angle(targets[i], toSync[i].rotation) * serializationRateHz
				);
			}

			if (updateRootPose)
			{
				transform.localPosition = Vector3.MoveTowards(
					transform.localPosition,
					networkPose,
					Time.deltaTime * Vector3.Distance(lastNetworkPose, networkPose) * serializationRateHz
				);
			}

			if (updateRootScale)
			{
				transform.localScale = Vector3.MoveTowards(
					transform.localScale,
					networkScale,
					Time.deltaTime * Vector3.Distance(lastNetworkScale, networkScale) * serializationRateHz
				);
			}
		}
	}
}