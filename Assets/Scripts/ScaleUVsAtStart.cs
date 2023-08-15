using UnityEngine;

public class ScaleUVsAtStart : MonoBehaviour
{
	public SkinnedMeshRenderer rend;

	public float scale = .1f;
	public Vector2 offset = Vector2.zero;

	private void Start()
	{
		Vector2[] uvs = rend.sharedMesh.uv;
		for (int i = 0; i < uvs.Length; i++)
		{
			uvs[i] *= scale;
			uvs[i] += offset;
		}

		rend.sharedMesh.SetUVs(0, uvs);
	}
}