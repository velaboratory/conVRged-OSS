using System.Collections.Generic;
using UnityEngine;
using unityutilities;
using unityutilities.VRInteraction;
using unityutilities.Interaction.WorldMouse;
using UnityEngine.Serialization;

namespace conVRged
{
	public class HandBase : MonoBehaviour
	{
		[Header("Tools")] public Tool currentTool;

		public enum Tool : byte
		{
			/// <summary>
			/// ✋
			/// </summary>
			AvatarHand,
			None,

			/// <summary>
			/// 🎯
			/// </summary>
			UICursor,
		}

		public bool sharedLaserShowing;
		public bool LASERShowing { get; private set; }
		public bool TeleporterShowing;

		public Transform LASERParent;
		private LineRenderer laserLine;


		public VRGrabbableHand hand;

		public GameObject uiCursorTool;
		public GameObject controllerModel;
		[FormerlySerializedAs("worldMouse")] public WorldMouseWithLaser worldMouseWithLaser;
		
		public AudioSource grabAudio;
		public AudioSource releaseAudio;


		/// <summary>
		/// Switches the hand model to the desired tool
		/// </summary>
		/// <param name="tool">The hand model of choice</param>
		/// <param name="local">True if the command was not sent over the network</param>
		public void SwitchTool(Tool tool)
		{
			if (currentTool == tool) return;

			// disable all tools
			if (uiCursorTool) uiCursorTool.SetActive(false);
			if (controllerModel) controllerModel.SetActive(false);
			if (worldMouseWithLaser) worldMouseWithLaser.enabled = false;
			if (VelNetMan.GetLocalPlayerPrefab()?.avatar != null)
			{
				VelNetMan.GetLocalPlayerPrefab().avatar.HandsMesh.Transform.GetComponent<SkinnedMeshRenderer>().enabled = false;
			}

			// enable the new tool
			switch (tool)
			{
				case Tool.AvatarHand:
					// if (controllerModel) controllerModel.SetActive(true);
					if (worldMouseWithLaser) worldMouseWithLaser.enabled = true;
					if (VelNetMan.GetLocalPlayerPrefab()?.avatar != null)
					{
						VelNetMan.GetLocalPlayerPrefab().avatar.HandsMesh.Transform.GetComponent<SkinnedMeshRenderer>().enabled = true;
					}

					break;
				case Tool.UICursor:
					if (uiCursorTool) uiCursorTool.SetActive(true);
					break;
			}

			currentTool = tool;

			// TODO network tool switching
			//PhotonMan.instance.photonView.RPC(nameof(SwitchTool), RpcTarget.Others, currentTool, false);

			if (hand != null)
			{
				Debug.Log($"Switched to tool '{tool}' on {hand.side}");
			}
			else
			{
				Debug.Log($"Switched to tool '{tool}'");
			}
		}

		/// <summary>
		/// Set actual visuals of laser 
		/// </summary>
		/// <param name="distance">Length of the laser</param>
		/// <param name="trigger">How far along the laser the gradient is.</param>
		protected void ResizeLaser(float distance, float trigger)
		{
			if (!laserLine) return;
			Vector3 pos = LASERParent.position;
			Vector3 dir = LASERParent.forward;
			laserLine.positionCount = 3;
			laserLine.SetPosition(0, pos);
			laserLine.SetPosition(1, pos + dir * distance * (1 - trigger));
			laserLine.SetPosition(2, pos + dir * distance);

			laserLine.widthMultiplier = .005f;

			Gradient grad = new Gradient();

			grad.SetKeys(
				new[]
				{
					new GradientColorKey(Color.red, 0)
				},
				new[]
				{
					new GradientAlphaKey(0, 0),
					new GradientAlphaKey(1, Mathf.Clamp(1 - trigger, 0.0001f, .99999f)),
					new GradientAlphaKey(1, 1)
				}
			);

			laserLine.colorGradient = grad;
		}

		/// <summary>
		/// Set actual visuals of laser 
		/// </summary>
		/// <param name="distance">Length of the laser</param>
		/// <param name="triggerValue">How far along the laser the gradient is.</param>
		protected void ResizeLaser(Vector3 goal, float triggerValue)
		{
			if (!laserLine) return;
			Vector3 origPos = LASERParent.position;
			Vector3 pos = origPos;
			Vector3 dir = LASERParent.forward;

			bool ended = false;
			List<Vector3> positions = new List<Vector3>();
			const float increment = .1f; // how often to make a new segment
			float angleIncrementRad = .03f / Vector3.Distance(pos, origPos); // how much to bend the ray
			while (!ended)
			{
				positions.Add(pos);
				pos += dir * increment;
				Vector3 finalDir = goal - pos;
				dir = Vector3.RotateTowards(dir, finalDir, angleIncrementRad, 0);

				if (Vector3.Distance(pos, origPos) > Vector3.Distance(goal, origPos))
				{
					positions.Add(goal);
					ended = true;
				}
			}

			laserLine.positionCount = positions.Count;
			laserLine.SetPositions(positions.ToArray());
			laserLine.widthMultiplier = .005f;

			Gradient grad = new Gradient();

			grad.SetKeys(
				new[]
				{
					new GradientColorKey(Color.black, 0)
				},
				new[]
				{
					new GradientAlphaKey(0, .1f),
					new GradientAlphaKey(1, Mathf.Clamp(1 - triggerValue, 0.1f, .9999f)),
					new GradientAlphaKey(1, 1)
				}
			);

			laserLine.colorGradient = grad;
		}


		/// <summary>
		/// Shows and hides the LASER pointer by destroying or creating it.
		/// </summary>
		/// <param name="show">Whether to show or hide the LASER.</param>
		public void ShowLASER(bool show)
		{
			if (show && laserLine == null)
			{
				laserLine = gameObject.AddComponent<LineRenderer>();
				laserLine.sortingOrder = 1;
				laserLine.material = new Material(Shader.Find("Sprites/Default"));
				laserLine.enabled = false;
			}

			if (laserLine)
			{
				laserLine.enabled = show || sharedLaserShowing;
			}

			LASERShowing = show || sharedLaserShowing;
		}

		/// <summary>
		/// Shows and hides the teleporter arc by destroying or creating it.
		/// This is a copy of the same function from Movement.cs, but only for remote hands.
		/// </summary>
		/// <param name="show">Whether to show or hide the teleporter arc.</param>
		public virtual void ShowRemoteTeleporter(bool show = true)
		{
		}
	}
}