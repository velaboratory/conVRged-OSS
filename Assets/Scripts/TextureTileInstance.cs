using UnityEngine;

namespace unityutilities {
	public class TextureTileInstance : MonoBehaviour {
		[Tooltip("The width of the texture. Larger numbers make larger textures.")]
		public float width = 1;

		[Tooltip("The height of the texture. Larger numbers make larger textures.")]
		public float height = 1;

		void Awake() {
			GetComponent<Renderer>().material.mainTextureScale = new Vector2(width, height);
		}
	}
}