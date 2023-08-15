using UnityEngine;
using VelNet;

public class KinematicWhenGrabbed : MonoBehaviour
{
	public NetworkObject netObj;
	public Rigidbody rb;
	private bool originalKinematic;

	// Start is called before the first frame update
	private void Start()
	{
		netObj = GetComponent<NetworkObject>();
		rb = GetComponent<Rigidbody>();
		originalKinematic = rb.isKinematic;
	}

	// Update is called once per frame
	private void Update()
	{
		if (netObj.IsMine)
		{
			rb.isKinematic = originalKinematic;
		}
		else
		{
			rb.isKinematic = true;
		}
	}
}
