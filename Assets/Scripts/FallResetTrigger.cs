using UnityEngine;

public class FallResetTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("checkersPiece"))
		{
			other.transform.localPosition = Vector3.zero;
		}
	}
}