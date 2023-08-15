using UnityEngine;

public class SlowSpinning : MonoBehaviour {

	public float speed = .1f;

	public Vector3 spinAxis = Vector3.up;
	public Space space;

	// Update is called once per frame
	void Update() {
		transform.Rotate(spinAxis, speed * Time.deltaTime, space);
	}
}
