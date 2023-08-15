using System.IO;
using UnityEditor;
using UnityEngine;
using unityutilities.VRInteraction;

namespace VelNet
{
	/// <summary>
	/// A simple class that will sync the position and rotation of a network object
	/// </summary>
	[AddComponentMenu("VelNet/VelNet Sync VRGrabbable")]
	public class SyncVRGrabbable : SyncState
	{
		public VRGrabbable grabbable;

		/// <summary>
		/// This gets called at serializationRateHz when the object is locally owned
		/// </summary>
		protected override void SendState(BinaryWriter writer)
		{
			byte[] bytes = grabbable.PackData();
			writer.Write(bytes.Length);
			writer.Write(bytes);
			writer.Write(grabbable.GrabbedBy != null);
		}

		/// <summary>
		/// This gets called whenever a message about the state of this object is received.
		/// Usually at serializationRateHz.
		/// </summary>
		protected override void ReceiveState(BinaryReader reader)
		{
			byte[] bytes = reader.ReadBytes(reader.ReadInt32());
			grabbable.networkGrabbed = reader.ReadBoolean();
			grabbable.UnpackData(bytes);
		}
	}
	
#if UNITY_EDITOR
	/// <summary>
	/// Sets up the interface for the CopyTransform script.
	/// </summary>
	[CustomEditor(typeof(SyncVRGrabbable))]
	public class SyncVRGrabbableEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			SyncVRGrabbable t = target as SyncVRGrabbable;

			EditorGUILayout.Space();

			if (t == null) return;

			EditorGUILayout.Space();

			if (GUILayout.Button("Find VR Grabbable"))
			{
				VRGrabbable g = t.GetComponentInChildren<VRGrabbable>();
				t.grabbable = g;
				PrefabUtility.RecordPrefabInstancePropertyModifications(t);
			}

			EditorGUILayout.Space();

			DrawDefaultInspector();
		}
	}
#endif
}