using System.Collections;
using System.Linq;
using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;

public class Snappable : MonoBehaviour
{
	public SnapPointProvider snapPointProvider;
	public VRGrabbable grabbable;
	public Rigidbody rb;
	private bool wasKinematic;

	[Tooltip("If this is set to true before game start, then it will try to snap on start.")]
	public bool snapped;

	// Start is called before the first frame update
	IEnumerator Start()
	{

		if (!grabbable) grabbable = GetComponent<VRGrabbable>();
		if (!rb) rb = GetComponent<Rigidbody>();

		grabbable.Released += OnRelease;
		grabbable.Grabbed += OnGrabbed;
		if (rb)
		{
			wasKinematic = rb.isKinematic;
		}

		yield return null;

		if (snapped)
		{
			OnRelease();
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Transform closestPoint = snapPointProvider.snapPoints.Aggregate((t1, t2) =>
			Vector3.Distance(t1.position, transform.position) < Vector3.Distance(t2.position, transform.position) ? t1 : t2);

		if (Vector3.Distance(transform.position, closestPoint.position) < snapPointProvider.minDistance)
		{
			// TODO highlight closest point
		}

		if (snapped && rb)
		{
			rb.isKinematic = true;
		}
	}

	void OnRelease()
	{
		Transform closestPoint = snapPointProvider.snapPoints.Aggregate((t1, t2) =>
			Vector3.Distance(t1.position, transform.position) < Vector3.Distance(t2.position, transform.position) ? t1 : t2);

		if (Vector3.Distance(transform.position, closestPoint.position) < snapPointProvider.minDistance)
		{
			transform.position = closestPoint.position;
			transform.rotation = closestPoint.rotation;
			snapped = true;
			if (rb)
			{
				rb.isKinematic = true;
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		} else
		{
			snapped = false;
		}
	}

	void OnGrabbed()
	{
		if (snapped)
		{
			snapped = false;
			if (rb)
			{
				rb.isKinematic = wasKinematic;
			}
		}
	}
}
