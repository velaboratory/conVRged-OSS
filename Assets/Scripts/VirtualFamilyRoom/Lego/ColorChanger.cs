using UnityEngine;

public class ColorChanger : MonoBehaviour
{
	/// <summary>
	/// changes color of lego block 
	/// user dips lego block into color bin to change its color
	/// </summary>
	public Color color;

	// Start is called before the first frame update
	private void OnTriggerEnter(Collider other)
	{
		//Debug.Log("HERE");
		if (!other.gameObject.CompareTag("legoBlock")) return;
		Material m = other.GetComponent<MeshRenderer>().material;
		if (m != null)
		{
			//m.color = this.color;
			m.CopyPropertiesFromMaterial(this.GetComponent<MeshRenderer>().material);
		}
	}
}