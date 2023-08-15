using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;
using VelNet;
using unityutilities.Interaction.WorldMouse;

namespace MindMap
{
	public class MindMapNode : SyncState
	{
		[XmlIgnore] public MindMapController controller;
		public int id;
		[XmlIgnore] public List<MindMapEdge> edges;
		[XmlIgnore] public TMP_Text textObj;
		[HideInInspector] public string text = "";
		public Color color;
		[XmlIgnore] private string controllerNetworkId;
		[XmlIgnore] public VRKeyboard keyboard;
		[XmlIgnore] public MeshRenderer rend;
		[XmlIgnore] public GameObject lockedGUIItems;
		[XmlIgnore] public VRMoveable vrMoveable;
		[XmlIgnore] public Rigidbody rb;
		public bool locked;

		[XmlIgnore]
		public bool Locked
		{
			get => locked;
			set
			{
				lockedGUIItems.SetActive(!value);
				vrMoveable.enabled = !value;
				if (vrMoveable != null) vrMoveable.enabled = !value;
				rb.isKinematic = value;
				locked = value;
			}
		}

		[XmlIgnore] public GameObject unlockConfirmPanel;
		[XmlIgnore] public static MindMapNode activeKeyboardNode;

		private void Start()
		{
			keyboard.keyPressed += key =>
			{
				networkObject.TakeOwnership();
				text = key.Type(text);
				textObj.text = text;
			};

			WorldMouseInputModule.FindCanvases();

			// if this was instantiated as a network object instead of locally
			if (controller != null)
			{
				// generate a new unique id
				// find the first unused value
				controller.nodes.RemoveAll(n => n == null);
				int[] used = controller.nodes.Select(o => o.id).ToArray();
				int available = -1;
				for (int i = 1; i <= used.Max() + 1; i++)
				{
					if (!used.Contains(i))
					{
						available = i;
						break;
					}
				}

				id = available;
			}
		}

		private void Update()
		{
			if (activeKeyboardNode == this)
			{
				foreach (char c in Input.inputString)
				{
					switch (c)
					{
						// has backspace/delete been pressed?
						case '\b':
						{
							if (text.Length != 0)
							{
								text = text[..^1];
							}

							break;
						}
						case '\n':
						// enter/return
						case '\r':
							print("User entered their name: " + text);
							break;
						default:
							text += c;
							break;
					}
				}

				textObj.text = text;
			}
		}

		protected override void SendState(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(text);
			binaryWriter.Write(color);
			binaryWriter.Write(controller.netObj.networkId);
			binaryWriter.Write(id);
			binaryWriter.Write(locked);
			// binaryWriter.Write(edges.Select(e=>e.networkObject.networkId).ToList());
		}

		protected override void ReceiveState(BinaryReader binaryReader)
		{
			string newText = binaryReader.ReadString();
			Color newColor = binaryReader.ReadColor();
			string newNetworkId = binaryReader.ReadString();
			id = binaryReader.ReadInt32();
			Locked = binaryReader.ReadBoolean();

			if (newText != text)
			{
				text = newText;
				textObj.text = text;
			}

			if (!newColor.Equals(color))
			{
				SetColor(newColor);
			}

			if (controllerNetworkId != newNetworkId)
			{
				controllerNetworkId = newNetworkId;
				if (controller != null)
				{
					controller.nodes.Remove(this);
				}

				controller = VelNetManager.instance.objects[controllerNetworkId].GetComponent<MindMapController>();
				controller.nodes.Add(this);
			}
		}

		private void OnDestroy()
		{
			if (controller != null)
			{
				controller.nodes.Remove(this);
			}
		}

		public void AddNode()
		{
			MindMapNode node = controller.AddNode(transform.position + Vector3.up * .5f, transform.rotation);
			controller.Connect(this, node);
		}

		public void RemoveNode()
		{
			controller.RemoveNode(this);
		}

		public void SetColor(string htmlCode)
		{
			networkObject.TakeOwnership();
			ColorUtility.TryParseHtmlString(htmlCode, out color);
			rend.material.color = color;
		}

		public void SetColor(Color newColor)
		{
			color = newColor;
			rend.material.color = color;
		}

		public void LinkOne()
		{
			controller.LinkOne(this);
		}

		public void KeyboardToggled(bool isOn)
		{
			activeKeyboardNode = isOn ? this : null;
		}

		/// <summary>
		/// Returns a list of points that the edges can connect to. 
		/// </summary>
		public List<Vector3> GetLinkPoints()
		{
			return new List<Vector3>
			{
				rend.transform.TransformPoint(.4f * Vector3.forward),
				rend.transform.TransformPoint(.4f * -Vector3.forward),
				rend.transform.TransformPoint(.4f * Vector3.up),
				rend.transform.TransformPoint(.4f * -Vector3.up),
			};
		}

		public void LockButton()
		{
			if (Locked)
			{
				unlockConfirmPanel.SetActive(true);
			}
			else
			{
				Locked = true;
			}
		}

		public void Unlock()
		{
			Locked = false;
		}

		public Dictionary<string, object> ToDict()
		{
			return new Dictionary<string, object>()
			{
				{ "id", id },
				{ "text", text },
				{
					"position", new Dictionary<string, float>()
					{
						{ "x", transform.position.x },
						{ "y", transform.position.y },
						{ "z", transform.position.z },
					}
				},
				{
					"rotation", new Dictionary<string, float>()
					{
						{ "x", transform.eulerAngles.x },
						{ "y", transform.eulerAngles.y },
						{ "z", transform.eulerAngles.z },
					}
				},
				{ "color", ColorUtility.ToHtmlStringRGB(color) },
			};
		}
	}
}