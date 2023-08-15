using System.Collections.Generic;
using UnityEngine;

public class VisualizeCloseness : MonoBehaviour
{
	public Transform[] objects;
	private List<LineRenderer> lines = new List<LineRenderer>();
	private Material lineMaterial;
	private MeshRenderer rend;
	public SpriteRenderer icon;
	private static readonly int Fade = Shader.PropertyToID("_Fade");

	// Start is called before the first frame update
	private void Start()
	{
		lineMaterial = new Material(Shader.Find("Unlit/Color"))
		{
			color = Color.black
		};
		rend = GetComponent<MeshRenderer>();


		foreach (Transform _ in objects)
		{
			GameObject g = new GameObject("Line");
			g.transform.SetParent(transform);
			lines.Add(g.AddComponent<LineRenderer>());
		}
	}

	// Update is called once per frame
	private void Update()
	{
		float closestHand = 100f;
		Vector3 closestHandPos = Vector3.zero;
		float radius = transform.localScale.x / 2f;
		float maxLineDistance = radius + .1f;
		for (int i = 0; i < objects.Length; i++)
		{
			float distance = Vector3.Distance(objects[i].position, transform.position);
			if (distance < closestHand)
			{
				closestHand = distance;
				closestHandPos = objects[i].position;
			}

			if (distance < maxLineDistance)
			{
				lines[i].enabled = true;

				float width = Mathf.Lerp(.005f, .001f, distance / maxLineDistance);

				lines[i].SetPositions(new Vector3[]
				{
					objects[i].position, transform.position
				});
				lines[i].startWidth = 0;
				lines[i].endWidth = width;

				lines[i].material = lineMaterial;
			}
			else
			{
				lines[i].enabled = false;
			}
		}

		Color color = rend.material.color;
		color.a = Mathf.Lerp(.5f, 0, closestHand / radius);
		rend.enabled = color.a != 0;
		// rend.material.color = color;
		rend.material.SetFloat(Fade, color.a);

		Color iconColor = icon.color;
		iconColor.a = Mathf.Lerp(.05f, .5f, -closestHand * 2 + 1f);
		icon.color = iconColor;
	}
}