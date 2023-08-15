using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using unityutilities;
using VelNet;
using unityutilities.Interaction.WorldMouse;


namespace MindMap
{
	public class MindMapEdge : SyncState
	{
		public MindMapController controller;
		public MindMapNode n1;
		public MindMapNode n2;
		public Transform mesh;
		public Transform closeButton;
		public Transform arrowTip;
		private string controllerNetworkId;

		private void Start()
		{
			WorldMouseInputModule.FindCanvases();
		}

		private void Update()
		{
			if (n1 == null || n2 == null) return;

			// find the closest link points
			Vector3 linkPoint1 = n1.GetLinkPoints().Aggregate((min, next) => Vector3.Distance(min, n2.transform.position) < Vector3.Distance(next, n2.transform.position) ? min : next);
			Vector3 linkPoint2 = n2.GetLinkPoints().Aggregate((min, next) => Vector3.Distance(min, n1.transform.position) < Vector3.Distance(next, n1.transform.position) ? min : next);

			Vector3 center = (linkPoint1 + linkPoint2) / 2;
			mesh.position = center;
			mesh.LookAt(linkPoint1);
			mesh.Rotate(Vector3.right, 90, Space.Self);
			Vector3 s = mesh.localScale;
			s.y = Vector3.Distance(linkPoint1, linkPoint2) / 2;
			mesh.localScale = s;

			closeButton.position = center;
			closeButton.LookAt(GameManager.instance.player.rig.head);

			arrowTip.position = linkPoint2;
			arrowTip.LookAt(linkPoint1);
		}

		protected override void SendState(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(n1 != null ? n1.networkObject.networkId : "");
			binaryWriter.Write(n2 != null ? n2.networkObject.networkId : "");
			binaryWriter.Write(controller != null ? controller.netObj.networkId : "");
		}

		protected override void ReceiveState(BinaryReader binaryReader)
		{
			string newN1 = binaryReader.ReadString();
			string newN2 = binaryReader.ReadString();
			string newNetworkId = binaryReader.ReadString();

			if (newN1 != "" && (n1 != null ? n1.networkObject.networkId : null) != newN1)
			{
				n1 = VelNetManager.instance.objects[newN1].GetComponent<MindMapNode>();
			}

			if (newN2 != "" && (n2 != null ? n2.networkObject.networkId : null) != newN2)
			{
				n2 = VelNetManager.instance.objects[newN2].GetComponent<MindMapNode>();
			}

			if (controllerNetworkId != newNetworkId)
			{
				controllerNetworkId = newNetworkId;
				if (controller != null)
				{
					controller.edges.Remove(this);
				}
				controller = VelNetManager.instance.objects[controllerNetworkId].GetComponent<MindMapController>();
				controller.edges.Add(this);
			}
		}

		private void OnDestroy()
		{
			if (controller != null)
			{
				controller.edges.Remove(this);
			}
		}

		public Dictionary<string, object> ToDict()
		{
			return new Dictionary<string, object>()
			{
				{ "n1", n1.id },
				{ "n2", n2.id },
			};
		}


		public void RemoveEdge()
		{
			controller.RemoveLink(this);
		}
	}
}