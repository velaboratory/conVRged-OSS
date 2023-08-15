using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VelNet;

namespace MindMap
{
	[XmlRoot("graphml")]
	public class MindMapController : MonoBehaviour
	{
		public string mapId;
		public NetworkObject netObj;
		public GameObject nodePrefab;
		public GameObject edgePrefab;

		[XmlArray("graph"), XmlArrayItem("node")]
		public List<MindMapNode> nodes = new List<MindMapNode>();

		public List<MindMapEdge> edges = new List<MindMapEdge>();
		private MindMapNode linkingNode;
		public Transform defaultNodePosition;
		public Button confirmButton;
		public GameObject confirmPopup;


		public void AddNode()
		{
			AddNode(defaultNodePosition.position, defaultNodePosition.rotation);
		}

		public MindMapNode AddNode(Vector3 position, Quaternion rotation)
		{
			MindMapNode n = VelNetManager.NetworkInstantiate(nodePrefab.name).GetComponent<MindMapNode>();
			n.transform.SetPositionAndRotation(position, rotation);
			n.controller = this;
			nodes.Add(n);
			return n;
		}

		public MindMapEdge Connect(MindMapNode n1, MindMapNode n2)
		{
			if (!nodes.Contains(n1)) return null;
			if (!nodes.Contains(n2)) return null;

			MindMapEdge existing = edges.Find(e => e.n1 == n1 && e.n2 == n2);
			if (existing != null) return existing;


			MindMapEdge e = VelNetManager.NetworkInstantiate(edgePrefab.name).GetComponent<MindMapEdge>();
			e.controller = this;
			e.transform.position = n1.transform.position;
			e.n1 = n1;
			e.n2 = n2;
			n1.edges.Add(e);
			n2.edges.Add(e);
			edges.Add(e);
			return e;
		}

		public void RemoveNode(MindMapNode node)
		{
			if (!nodes.Contains(node))
			{
				Debug.LogError("Can't remove node if it isn't already added.");
				return;
			}

			edges.Where(e => node == e.n1 || node == e.n2).ToList().ForEach(e =>
			{
				VelNetManager.NetworkDestroy(e.networkObject);
			});
			VelNetManager.NetworkDestroy(node.networkObject);
			nodes.Remove(node);
		}

		public void LinkOne(MindMapNode node)
		{
			if (linkingNode == null)
			{
				linkingNode = node;
			}
			else
			{
				Connect(linkingNode, node);
				linkingNode = null;
			}
		}

		public void RemoveLink(MindMapEdge edge)
		{
			if (!edges.Contains(edge))
			{
				Debug.LogError("Can't remove node if it isn't already added.");
				return;
			}

			VelNetManager.NetworkDestroy(edge.networkObject);


			// List<MindMapNode> edgeNodes = nodes.FindAll(n => n == edge.n1 || n == edge.n2);

			// foreach (MindMapNode n in edgeNodes)
			// {
			// 	VelNetManager.NetworkDestroy(n.networkObject);
			// 	nodes.Remove(n);
			// }
		}
		public void DestroyAllButton()
		{
			confirmPopup.SetActive(true);
			confirmButton.onClick.RemoveAllListeners();
			confirmButton.onClick.AddListener(() =>
			{
				confirmPopup.SetActive(false);
				DestroyAll();
			});
		}

		public void DestroyAll()
		{
			foreach (MindMapEdge e in edges.ToArray())
			{
				VelNetManager.NetworkDestroy(e.networkObject);
			}

			foreach (MindMapNode n in nodes.ToArray())
			{
				VelNetManager.NetworkDestroy(n.networkObject);
			}
		}

		public void LoadPresetButton(int presetIndex)
		{
			confirmPopup.SetActive(true);
			confirmButton.onClick.RemoveAllListeners();
			confirmButton.onClick.AddListener(() =>
			{
				confirmPopup.SetActive(false);
				LoadPreset(presetIndex);
			});
		}

		public void LoadPreset(int presetIndex)
		{
			DestroyAll();

			// TODO make this load from XML/JSON
			switch (presetIndex)
			{
				case 0:
				{
					MindMapNode node1 = AddNode(defaultNodePosition.position, defaultNodePosition.rotation);
					node1.text = "Future of Journalism";
					node1.textObj.text = node1.text;
					node1.SetColor(Color.red);
					node1.Locked = true;
					break;
				}
				case 1:
				{
					MindMapNode node1 = AddNode(defaultNodePosition.position, defaultNodePosition.rotation);
					node1.text = "Future of Transportation";
					node1.textObj.text = node1.text;
					node1.SetColor(Color.red);
					node1.Locked = true;
					break;
				}
			}
		}

		public void Log()
		{
			unityutilities.Logger.LogRow("mind_map", ToJson());
			// _ = File.WriteAllTextAsync(Path.Combine(unityutilities.Logger.GetCurrentLogFolder(), DateTime.UtcNow.ToString("u") + "_mind_map.xml"), ToXml());
			unityutilities.Logger.LogRow("mind_map_xml", ToXml());
		}

		public string ToJson()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>()
			{
				{ "map_id", mapId },
				{ "nodes", nodes.Select(n => n.ToDict()) },
				{ "edges", edges.Select(e => e.ToDict()) }
			};
			return JsonConvert.SerializeObject(dict);
		}

		public string ToXml()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>()
			{
				{ "map_id", mapId },
				{ "nodes", nodes.Select(n => n.ToDict()) },
				{ "edges", edges.Select(e => e.ToDict()) }
			};

			XmlSerializer serializer = new XmlSerializer(typeof(GraphML));

			using StringWriter stream = new StringWriter();
			serializer.Serialize(stream, new GraphML(this));
			return stream.ToString();
		}

		[XmlRoot("graphml")]
		public class GraphML
		{
			public GraphML()
			{
			}

			public GraphML(MindMapController c)
			{
				int edgeId = 0;
				graph = new Graph
				{
					map_id = c.mapId,
					nodes = c.nodes.Select(n => new Node()
					{
						id = n.id,
						mainText = n.textObj.text,
						positionX = n.transform.position.x * 100,
						positionY = n.transform.position.y * 100,
						positionZ = n.transform.position.z * 100,
					}).ToArray(),
					edges = c.edges.Select(e => new Edge()
					{
						id = edgeId++,
						source = e.n1.id,
						target = e.n2.id,
					}).ToArray()
				};
			}

			[XmlElement("graph")] public Graph graph;

			public class Graph
			{
				[XmlElement("node")] public Node[] nodes;
				[XmlElement("edge")] public Edge[] edges;
				[XmlAttribute] public string map_id;
				[XmlAttribute] public int uidGraph => nodes.Max(n => n.id) + 1;
				[XmlAttribute] public int uidEdge => edges.Max(n => n.id) + 1;
			}

			public class Node
			{
				[XmlAttribute] public float positionX;
				[XmlAttribute] public float positionY;
				[XmlAttribute] public float positionZ;
				[XmlAttribute] public int id;
				[XmlAttribute] public string mainText;
			}

			public class Edge
			{
				[XmlAttribute] public int source;
				[XmlAttribute] public int target;
				[XmlAttribute] public int id;
			}
		}
	}
}