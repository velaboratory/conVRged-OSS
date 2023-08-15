using UnityEngine;

namespace LocomotionStudy
{
	public class Platform : MonoBehaviour
	{
		public PlatformsManager manager;
		public Renderer rend;
		public AudioSource audio;

		// Start is called before the first frame update
		private void Start()
		{
			rend ??= GetComponent<Renderer>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				manager.HighlightPlatform(this);
			}
		}
	}
}