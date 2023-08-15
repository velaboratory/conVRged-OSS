using System.IO;
using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;
using VelNet;

[RequireComponent(typeof(VRGrabbable))]
public class VRGrabbableSetNetworkVelNet : SyncState
{
	[ReadOnly] private bool networkGrabbed;
	private VRGrabbable grabbable;

	protected override void Awake()
	{
		base.Awake();
		grabbable = GetComponent<VRGrabbable>();
	}

	public void FixedUpdate()
	{
		grabbable.networkGrabbed = networkGrabbed;
		grabbable.locallyOwned = IsMine;
	}

	protected override void SendState(BinaryWriter binaryWriter)
	{
		binaryWriter.Write(grabbable.GrabbedBy != null);
	}

	protected override void ReceiveState(BinaryReader binaryReader)
	{
		networkGrabbed = binaryReader.ReadBoolean();
	}
}