using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePoints : MonoBehaviour
{
	[Tooltip("The mesh to emit arrows from. If null, this calls .GetComponent<MeshFilter>")]
	public MeshFilter referenceMesh;
	private Mesh mesh;
	private Bounds bounds;
	public Shader arrowShader;
	public float waterHeight;
	public Color arrowColor = Color.red;
	public float lengthScale = .3f;
	public float arrowWidth = .01f;
	[Tooltip("For floating")]
	public float radius = 1;
	private ComputeBuffer meshPropertiesBuffer;
	private ComputeBuffer argsBuffer;
	private Material arrowMat;
	private static readonly int testest = Shader.PropertyToID("_testest");
	private static readonly int waterTop = Shader.PropertyToID("_waterTop");
	private static readonly int scale = Shader.PropertyToID("_lengthScale");
	private static readonly int color = Shader.PropertyToID("_arrowColor");


	private bool useRefMesh = false;
	private static readonly int Properties = Shader.PropertyToID("_Properties");


	private struct MeshProperties
	{
		public Matrix4x4 mat;

		public static int Size()
		{
			return
				sizeof(float) * 4 * 4; // matrix;
		}
	}

	private void Start()
	{
		useRefMesh = referenceMesh != null;
		
		arrowMat = new Material(arrowShader);
		mesh = CreateMesh();

		//get the current mesh, or the provided one
		Mesh currentMesh = useRefMesh ? referenceMesh.mesh : GetComponent<MeshFilter>().mesh;
		bounds = currentMesh.bounds;
		//loop through the vertices and normals 
		Vector3[] verts = currentMesh.vertices;
		Vector3[] norms = currentMesh.normals;

		uint[] args = new uint[5] {0, 0, 0, 0, 0};
		args[0] = mesh.GetIndexCount(0);
		args[1] = (uint) verts.Length;
		args[2] = mesh.GetIndexStart(0);
		args[3] = mesh.GetBaseVertex(0);
		argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		argsBuffer.SetData(args);


		// Initialize buffer with the given population.
		MeshProperties[] properties = new MeshProperties[verts.Length];
		for (int i = 0; i < verts.Length; i++)
		{
			MeshProperties props = new MeshProperties();
			Vector3 position = verts[i];
			Quaternion rotation = Quaternion.LookRotation(norms[i]);
			Vector3 scale = Vector3.one;

			props.mat = Matrix4x4.TRS(position, rotation, scale);
			properties[i] = props;
		}

		meshPropertiesBuffer = new ComputeBuffer(verts.Length, MeshProperties.Size());
		meshPropertiesBuffer.SetData(properties);
		arrowMat.SetBuffer(Properties, meshPropertiesBuffer);
		meshPropertiesBuffer.Dispose();
	}

	// Update is called once per frame
	private void Update()
	{
		arrowMat.SetMatrix(testest, useRefMesh ? referenceMesh.transform.localToWorldMatrix : transform.localToWorldMatrix);
		arrowMat.SetFloat(waterTop, waterHeight);
		arrowMat.SetFloat(scale, lengthScale);
		arrowMat.SetColor(color, arrowColor);
		bounds.center = transform.position;

		Graphics.DrawMeshInstancedIndirect(mesh, 0, arrowMat, bounds, argsBuffer);
	}

	private void OnDestroy()
	{
		argsBuffer.Dispose();
	}

	private Mesh CreateMesh()
	{
		//square mesh of length 1 on z, but adjustable width
		Mesh mesh = new Mesh();
		float w = arrowWidth;
		Vector3[] vertices = new Vector3[8]
		{
			new Vector3(-w, w, 0),
			new Vector3(w, w, 0),
			new Vector3(w, -w, 0),
			new Vector3(-w, -w, 0),
			new Vector3(-w, w, 1),
			new Vector3(w, w, 1),
			new Vector3(w, -w, 1),
			new Vector3(-w, -w, 1)
		};

		int[] tris = new int[36]
		{
			0, 1, 2,
			0, 2, 3,
			6, 2, 1,
			6, 1, 5,

			5, 0, 4,
			0, 5, 1,
			0, 3, 7,
			0, 7, 4,
			7, 5, 4,
			6, 5, 7,
			6, 3, 2,
			6, 7, 3
		};

		mesh.vertices = vertices;
		mesh.triangles = tris;
	
		mesh.RecalculateBounds();

		return mesh;
	}
}