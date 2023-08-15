using System.Collections;
using UnityEngine;
using VelNet;

public class ResetObjectsToLocalOrigin : NetworkComponent
{
	public GameObject[] objects;

	private float lastResetTime = 0;

	public void ResetBoard()
	{
		if (Time.time - lastResetTime < 1)
		{
			return;
		}

		lastResetTime = Time.time;
		
		// take ownership over this board so we can send a message
		networkObject.TakeOwnership();

		// 1 means reset board. This message will only be delivered to the corresponding component on other players,
		// so no more specificity is needed.
		SendBytes(new byte[] { 1 });
		ResetBoardCallback();
	}

	private void ResetBoardCallback()
	{
		// find all pieces in game and reset them
		foreach (GameObject g in objects)
		{
			NetworkObject netObj = g.GetComponent<NetworkObject>();
			
			// move pieces if we own them
			// if (netObj.IsMine)
			// {
			// 	VelNetManager.TakeOwnership(netObj.networkId);
			// }

			g.transform.localPosition = Vector3.zero;
			g.transform.localRotation = Quaternion.identity;

			Rigidbody r = g.GetComponent<Rigidbody>();
			r.velocity = Vector3.zero;
			r.angularVelocity = Vector3.zero;
		}

		// StartCoroutine(ResetAgain());
	}

	private IEnumerator ResetAgain()
	{
		yield return new WaitForSeconds(1);

		foreach (GameObject g in objects)
		{
			g.transform.localPosition = Vector3.zero;
			g.transform.localRotation = Quaternion.identity;

			Rigidbody r = g.GetComponent<Rigidbody>();
			r.velocity = Vector3.zero;
			r.angularVelocity = Vector3.zero;
		}
	}

	public override void ReceiveBytes(byte[] message)
	{
		if (message[0] == 1)
		{
			ResetBoardCallback();
		}
	}
}