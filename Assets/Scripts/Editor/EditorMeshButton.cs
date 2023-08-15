using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(MeshButton))]
public class EditorMeshButton : ButtonEditor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		MeshButton script = (MeshButton)target;

		script.targetMeshRenderer = (MeshRenderer)EditorGUILayout.ObjectField("Mesh Renderer", script.targetMeshRenderer, typeof(MeshRenderer), true);

		if (GUILayout.Button("Find mesh renderer")) {
			script.targetMeshRenderer = script.GetComponent<MeshRenderer>();
		}

		if (GUILayout.Button("Select Button"))
		{
			script.Select();
			Debug.Log("Select button");
		}
		
		if (GUILayout.Button("Click Button"))
		{
			script.Select();
			script.onClick.Invoke();
			Debug.Log("Click Button");
		}
	}
}
