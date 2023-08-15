using UnityEngine;
using UnityEngine.Serialization;

public class SetCenterOfMass : MonoBehaviour
{
	public Transform centerOfMass;
	[FormerlySerializedAs("rigidbody")]
	public Rigidbody rb;

	void Start()
	{
		rb.centerOfMass = centerOfMass.position;
	}
}