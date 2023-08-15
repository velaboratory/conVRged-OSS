using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UnkinematicAfterSeconds : MonoBehaviour
{
	public float seconds = 1;

	private IEnumerator Start()
	{
		GetComponent<Rigidbody>().useGravity = false;
		yield return new WaitForSeconds(seconds);
		GetComponent<Rigidbody>().useGravity = true;
	}
}