using System.Collections;
using UnityEngine;

public class Unparent : MonoBehaviour
{
	public bool destroyWhenOldParentDestroyed;
	private Transform oldParent;
	private bool doDestroy;

	// Start is called before the first frame update
	private IEnumerator Start()
	{
		yield return null;
		oldParent = transform.parent;
		doDestroy = oldParent != null && destroyWhenOldParentDestroyed;
		transform.SetParent(null);
	}

	private void Update()
	{
		if (doDestroy && oldParent == null)
		{
			Destroy(gameObject);
		}
	}
}