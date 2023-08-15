using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using unityutilities;
using Logger = unityutilities.Logger;

namespace LocomotionStudy
{
	/// <summary>
	/// The colors are not synced across the network, but this is not worth the problems
	/// </summary>
	public class PlatformsManager : MonoBehaviour
	{
		public Platform[] platforms;
		public Material normalColor;
		public Material currentColor;
		public Material nextColor;
		private int currentPlatform;
		private int lastPlatform;
		private int nextPlatform;
		private Material startFinishDefaultMaterial;
		public Renderer startFinish;

		private const string platformsFileName = "platforms";

		public LineRenderer line;

		// Start is called before the first frame update
		private void Start()
		{
			startFinishDefaultMaterial = startFinish.material;
			foreach (Platform platform in platforms)
			{
				platform.manager = this;
			}

			// draw the lines between platforms
			line.positionCount = platforms.Length + 1;
			List<Vector3> points = platforms.Select(p => p.transform.position).ToList();
			points.Add(platforms[0].transform.position);
			line.SetPositions(points.ToArray());
			
			// log the platform positions to csv so we can copy it out
			StringBuilder sb = new StringBuilder();
			for (var i = 0; i < platforms.Length; i++)
			{
				sb.Append(i);
				sb.Append(',');
				sb.Append(platforms[i].transform.position.x);
				sb.Append(',');
				sb.Append(platforms[i].transform.position.y);
				sb.Append(',');
				sb.Append(platforms[i].transform.position.z);
				sb.Append('\n');
			}
			Debug.Log(sb.ToString());
		}

		private void OnEnable()
		{
			GameManager.instance.player.movement.TeleportStart += OnMovementTeleportStart;
			GameManager.instance.player.movement.TeleportEnd += OnMovementTeleportEnd;
			GameManager.instance.player.movement.SnapTurn += OnMovementSnapTurn;

			// hand loco doesn't exist on 2d player
			if (GameManager.instance.player.handLoco)
			{
				GameManager.instance.player.handLoco.TeleportStart += OnHandLocoTeleportStart;
				GameManager.instance.player.handLoco.TeleportEnd += OnHandLocoTeleportEnd;
				GameManager.instance.player.handLoco.SnapTurn += OnHandLocoSnapTurn;
			}
		}

		private void OnDisable()
		{
			GameManager.instance.player.movement.TeleportStart -= OnMovementTeleportStart;
			GameManager.instance.player.movement.TeleportEnd -= OnMovementTeleportEnd;
			GameManager.instance.player.movement.SnapTurn -= OnMovementSnapTurn;

			if (GameManager.instance.player.handLoco)
			{
				GameManager.instance.player.handLoco.TeleportStart -= OnHandLocoTeleportStart;
				GameManager.instance.player.handLoco.TeleportEnd -= OnHandLocoTeleportEnd;
				GameManager.instance.player.handLoco.SnapTurn -= OnHandLocoSnapTurn;
			}
		}

		private void OnHandLocoSnapTurn(Side side, string direction)
		{
			SnapTurn(side, "hand", direction);
		}

		private void OnHandLocoTeleportEnd(Side side, float time, Vector3 vector3)
		{
			TeleportEnd(side, "hand", time, vector3);
		}

		private void OnHandLocoTeleportStart(Side side)
		{
			TeleportStart(side, "hand");
		}

		private void OnMovementSnapTurn(Side side, string direction)
		{
			SnapTurn(side, "controller", direction);
		}

		private void OnMovementTeleportEnd(Side side, float time, Vector3 vector3)
		{
			TeleportEnd(side, "controller", time, vector3);
		}

		private void OnMovementTeleportStart(Side side)
		{
			TeleportStart(side, "controller");
		}

		private void TeleportStart(Side side, string loco)
		{
			StringList l = new StringList(new List<dynamic>()
			{
				"teleport-start",
				side,
				loco,
				GameManager.instance.player.rig.head.position,
				currentPlatform.ToString(),
				nextPlatform.ToString(),
			});

			Logger.LogRow(platformsFileName, l.List);
		}

		private void TeleportEnd(Side side, string loco, float time, Vector3 translation)
		{
			Vector3 headPos = GameManager.instance.player.rig.head.position;
			StringList l = new StringList(new List<dynamic>()
			{
				"teleport-end",
				side,
				loco,
				time,
				translation,
				headPos,
				Vector3.Distance(headPos, platforms[currentPlatform].transform.position).ToString(CultureInfo.InvariantCulture),
				Vector3.Distance(headPos, platforms[nextPlatform].transform.position).ToString(CultureInfo.InvariantCulture),
				currentPlatform.ToString(),
				nextPlatform.ToString(),
			});


			Logger.LogRow(platformsFileName, l.List);
		}

		private void SnapTurn(Side side, string loco, string direction)
		{
			List<string> l = new StringList(new List<dynamic>
			{
				"snap-turn",
				side,
				loco,
				direction,
				GameManager.instance.player.rig.head.position,
				currentPlatform.ToString(),
				nextPlatform.ToString(),
			}).List;

			Logger.LogRow(platformsFileName, l);
		}

		public void HighlightPlatform(Platform p)
		{
			lastPlatform = currentPlatform;

			foreach (Platform platform in platforms)
			{
				platform.rend.material = normalColor;
			}

			currentPlatform = platforms.ToList().IndexOf(p);
			nextPlatform = (currentPlatform + 1) % platforms.Length;

			platforms[currentPlatform].rend.material = currentColor;
			platforms[nextPlatform].rend.material = nextColor;
			platforms[nextPlatform].audio.Play();

			startFinish.material = currentPlatform == 0 ? currentColor : startFinishDefaultMaterial;
		}
	}
}