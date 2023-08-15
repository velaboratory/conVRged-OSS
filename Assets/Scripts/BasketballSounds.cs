using UnityEngine;

public class BasketballSounds : MonoBehaviour
{
	public AudioSource dribble;
	public AudioSource swoosh;

	public AnimationCurve volumeCurve;

	[Tooltip("Is set automatically if not set here")]
	public Rigidbody rb;

	private void Start()
	{
		rb ??= GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		float v = Mathf.Clamp(rb.velocity.magnitude, 0, 5);
		v /= 5f;

		float volume = volumeCurve.Evaluate(v);


		dribble.volume = volume;
		dribble.Play();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("net"))
		{
			float v = Mathf.Clamp(rb.velocity.magnitude, 0, 1);
			v /= 2f;

			float volume = volumeCurve.Evaluate(v);


			swoosh.volume = volume;
			swoosh.Play();
		}
	}
}