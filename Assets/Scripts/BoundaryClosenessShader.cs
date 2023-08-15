using System;
using UnityEngine;

public class BoundaryClosenessShader : MonoBehaviour
{
	public Transform lHand;
	public Transform rHand;
	private Material closenessShader;
	private static readonly int LHandPos = Shader.PropertyToID("_LHandPos");
	private static readonly int RHandPos = Shader.PropertyToID("_RHandPos");

	private void Start()
	{
		closenessShader = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		// set hand positions
		closenessShader.SetVector(LHandPos, GameManager.instance.player.leftHandPosition);
		closenessShader.SetVector(RHandPos, GameManager.instance.player.rightHandPosition);
	}
}