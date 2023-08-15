using UnityEngine;

public class DisableIfHandTracking : MonoBehaviour
{
	public OVRSkeleton skeleton;
	public GameObject obj;

	[Header("Enable if hand tracking is:")]
	public bool handTracking;

	private void Update()
	{
		obj.SetActive(skeleton.IsDataHighConfidence && handTracking);
	}
}