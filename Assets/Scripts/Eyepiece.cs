using UnityEngine;

public class Eyepiece : MonoBehaviour
{
	public Camera cam;
	private RenderTexture rend;
	public int res = 256;

	private void Start()
	{
		if (cam)
		{
			rend = new RenderTexture(res, res, 16);
			cam.targetTexture = rend;
			GetComponent<Renderer>().material.mainTexture = rend;
		}
	}
}