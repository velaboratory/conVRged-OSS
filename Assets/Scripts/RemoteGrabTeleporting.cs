using UnityEngine;
using unityutilities.VRInteraction;

public class RemoteGrabTeleporting : VRGrabbable
{
	public LineRenderer lineRend;
	public int actorNum;

	private new void Awake()
	{
		base.Awake();
		includeInSave = false;
	}

	public override byte[] PackData()
	{
		return null;
	}

	public override void UnpackData(byte[] data)
	{
	}

	public override void HandleGrab(VRGrabbableHand h)
	{
		base.HandleGrab(h);

	}

	public override void HandleRelease(VRGrabbableHand h = null)
	{
		base.HandleRelease(h);

		// where is the user pointing?
		if (Physics.Raycast(h.transform.position, h.transform.forward, out RaycastHit hit))
		{
			// the point to teleport to
			Vector3 destinationPoint = hit.point;

			// TODO use velnet
			// PhotonNetwork.RaiseEvent(
			// 	(byte)PhotonMan.MessageType.RequestTeleport,
			// 	destinationPoint,
			// 	new RaiseEventOptions { TargetActors = new int[] { actorNum } },
			// 	new SendOptions());
		}
	}


	private void Update()
	{
		if (GrabbedBy != null)
		{
			// where is the user pointing?
			if (Physics.Raycast(GrabbedBy.transform.position, GrabbedBy.transform.forward, out RaycastHit hit))
			{
				// the point to teleport to
				Vector3 destinationPoint = hit.point;

				lineRend.SetPositions(new Vector3[] { transform.position, destinationPoint });

				lineRend.enabled = true;
			}
		}
		else
		{
			lineRend.enabled = false;
		}
	}
}
