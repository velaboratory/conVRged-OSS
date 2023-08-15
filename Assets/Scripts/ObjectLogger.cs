using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;
using VelNet;
using Logger = unityutilities.Logger;

public class ObjectLogger : MonoBehaviour
{
	public float updateRateHz = 4;

	// Start is called before the first frame update
	private IEnumerator Start()
	{
		while (true)
		{
			LogObjects();
			yield return new WaitForSeconds(1f / updateRateHz);
		}
	}

	private void LogObjects()
	{
		try
		{
			// finds all vrgrabbable objects and logs their state
			VRGrabbable[] objs = FindObjectsOfType<VRGrabbable>();
			foreach (VRGrabbable g in objs)
			{
				NetworkObject netObj = g.GetComponent<NetworkObject>();
				if (netObj == null) continue;
				StringList l = new StringList(new List<dynamic>()
				{
					g.name,
					g.networkGrabbed,
					g.GrabbedBy != null,
					g.transform.position,
					g.transform.rotation,
					netObj.networkId,
					netObj.IsMine,
					netObj.owner?.userid,
				});

				Logger.LogRow("objects", l.List);
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e, this);
		}
	}
}