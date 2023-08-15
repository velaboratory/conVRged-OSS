using UnityEngine;
using UnityEngine.XR;

public class EnableObjectsBasedOnVR2DMode : MonoBehaviour
{
	[System.Serializable]
	public class ObjAndModePair
	{
		public Transform obj;
		public bool vrEnabled;
	}

	public ObjAndModePair[] objects;

	private void Start()
	{
		Refresh();
	}

	private void Refresh()
	{
		bool vrEnabled = XRSettings.enabled;
		foreach (ObjAndModePair obj in objects)
		{
			obj.obj.gameObject.SetActive(obj.vrEnabled == vrEnabled);
		}
	}
}