using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Frisbee : MonoBehaviour {
	public float floatingDrag;
	private Rigidbody rb;

	private void Start() {
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	private void FixedUpdate() {
		Vector3 vel = rb.velocity;
		vel -= Vector3.Project(vel, transform.up * .001f);
	}
}
