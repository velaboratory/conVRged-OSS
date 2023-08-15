using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using unityutilities;

public class RoomCreator : MonoBehaviour
{
	Rig rig;

	List<Vector3> wallPoints = new List<Vector3>();
	GameObject wall;
	public Material wallMaterial;
	public bool useOculusBounds;

	private bool wallGenerated = false;

	private void Start()
	{
		rig = GameManager.instance.player.rig;

		if (useOculusBounds)
		{
#if OCULUS_INTEGRATION
			if (OVRManager.boundary != null && OVRManager.boundary.GetConfigured()) {
				wallPoints.AddRange(OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary));
				int count = wallPoints.Count * 4;
				for (int i = 0; i < count; i += 4) {
					Vector3 ceilingPoint = wallPoints[i];
					ceilingPoint.y = 3;
					wallPoints.Insert(i + 1, ceilingPoint);
					wallPoints.Insert(i + 2, wallPoints[i]);
					wallPoints.Insert(i + 3, wallPoints[i+1]);

				}

				GenerateWall(true);
			}
#endif
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!wallGenerated)
		{
			if (InputMan.MenuButtonDown(Side.Left))
			{
				Vector3 point = rig.leftHand.position;
				point.y = 0;
				wallPoints.Add(point);
				point.y = 3;
				wallPoints.Add(point);

				// doubled for flat shading
				point.y = 0;
				wallPoints.Add(point);
				point.y = 3;
				wallPoints.Add(point);

				GenerateWall(false);
			}

			if (InputMan.MenuButtonDown(Side.Right))
			{
				GenerateWall(false);
				wallGenerated = true;
			}
		}

	}

	void GenerateWall(bool isOculusBounds)
	{


		if (wallPoints.Count < 6) return;

		Destroy(wall);

		List<Vector3> wallPoints2 = new List<Vector3>();
		Vector3[] wappoint = wallPoints.ToArray();
		wallPoints2.AddRange(wappoint);

		//backface duplciate
		int wpCount = wallPoints2.Count;
		for (int i = 0; i < wpCount; i += 2)
		{

			wallPoints2.Add(wallPoints2[i + 1]);
			wallPoints2.Add(wallPoints2[i]);

		}


		Mesh mesh = new Mesh();
		mesh.SetVertices(wallPoints2);

		List<int> indiceise = new List<int>();
		for (int i = 2; i <= wallPoints2.Count / 2 - 4; i += 4)
		{
			indiceise.Add(i);
			indiceise.Add(i + 1);
			indiceise.Add(i + 3);
			indiceise.Add(i + 2);

		}

		indiceise.Add(wallPoints2.Count / 2 - 1);
		indiceise.Add(wallPoints2.Count / 2 - 2);
		indiceise.Add(0);
		indiceise.Add(1);

		for (int i = wallPoints2.Count / 2 + 2; i <= wallPoints2.Count - 4; i += 4)
		{
			indiceise.Add(i);
			indiceise.Add(i + 1);
			indiceise.Add(i + 3);
			indiceise.Add(i + 2);

		}

		indiceise.Add(wallPoints2.Count - 1);
		indiceise.Add(wallPoints2.Count - 2);
		indiceise.Add(wallPoints2.Count / 2 + 0);
		indiceise.Add(wallPoints2.Count / 2 + 1);



		mesh.SetIndices(indiceise.ToArray(), MeshTopology.Quads, 0);
		wall = new GameObject();
		MeshRenderer rend = wall.AddComponent<MeshRenderer>();
		MeshFilter filter = wall.AddComponent<MeshFilter>();
		filter.mesh = mesh;
		mesh.RecalculateNormals();
		rend.sharedMaterial = wallMaterial;
		wall.transform.SetParent(rig.transform);
		if (isOculusBounds)
		{
			wall.transform.localPosition = Vector3.zero;
			wall.transform.localEulerAngles = Vector3.zero;
		}
		wall.AddComponent<CopyTransform>().SetTarget(rig.transform);

	}
}
