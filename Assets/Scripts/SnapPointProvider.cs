using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPointProvider : MonoBehaviour
{
	public List<Transform> snapPoints;
	public float minDistance = .1f;

	public bool createGrid;

	public Transform startingPos;
	public float spacing;
	public int count;

	private void Start()
	{
		for (int i = 0; i < count; i++)
		{
			for (int j = 0; j < count; j++)
			{
				Transform point = new GameObject("snapPoint").transform;
				point.position = startingPos.position + startingPos.forward * spacing * -i + startingPos.right * spacing * j;
				point.forward = startingPos.forward;
				snapPoints.Add(point);
			}
		}
	}
}
