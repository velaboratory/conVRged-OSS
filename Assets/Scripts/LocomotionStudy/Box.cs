using UnityEngine;
using VelNet;

namespace LocomotionStudy
{
	public class Box : MonoBehaviour
	{
		private void OnTriggerExit(Collider other)
		{
			BoxObject bo = other.attachedRigidbody.GetComponent<BoxObject>();
			if (bo)
			{
				bo.OnFound?.Invoke();
				bo.grabbable.GrabbedBy.Release();
				VelNetManager.NetworkDestroy(bo.networkObject);
			}
		}
	}
}