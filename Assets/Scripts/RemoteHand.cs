using System.Collections.Generic;
using UnityEngine;
using unityutilities;

namespace conVRged
{
	public class RemoteHand : HandBase
	{
		private LineRenderer teleporterLineRenderer;
		private Movement.Teleporter teleporter;

		public Wolf3DAvatar avatar;

		// Start is called before the first frame update
		private void Start()
		{
			teleporter = new Movement.Teleporter(GameManager.instance.player.movement.teleporter);
			GameObject teleporterLineRendObj = new GameObject("Teleporter Line");
			teleporterLineRendObj.transform.position = Vector3.up * -100;
			teleporterLineRenderer = teleporterLineRendObj.AddComponent<LineRenderer>();
			teleporterLineRenderer.widthMultiplier = teleporter.lineRendererWidth;
			teleporterLineRenderer.enabled = false;
			if (teleporter.lineRendererMaterialOverride != null)
			{
				teleporterLineRenderer.material = teleporter.lineRendererMaterialOverride;
			}
			else
			{
				Material material;
				(material = teleporterLineRenderer.material).shader = Shader.Find("Unlit/Color");
				material.color = Color.black;
			}
		}

		// Update is called once per frame
		private void Update()
		{
			if (TeleporterShowing)
			{
				ShowRemoteTeleporter(); // just to update the position
			}

			if (LASERShowing) ResizeLaser(10, 1);
		}

		public override void ShowRemoteTeleporter(bool show = true)
		{
			if (!teleporterLineRenderer) return;

			if (!show && teleporterLineRenderer)
			{
				teleporterLineRenderer.enabled = false;
				teleporter.Active = false;
				return;
			}

			teleporterLineRenderer.enabled = true;

			// simulate the curved ray
			Vector3 lastPos;
			Vector3 lastDir;
			float velocity = teleporter.teleportArcInitialVel;
			List<Vector3> points = new List<Vector3>();

			lastPos = transform.position;
			lastDir = transform.forward;

			Vector3 xVelocity = Vector3.ProjectOnPlane(lastDir, Vector3.up) * velocity;
			float yVelocity = Vector3.Dot(lastDir, Vector3.up) * velocity;

			float segmentLength = .25f;
			const float numSegments = 200f;

			RaycastHit teleportHit;

			// the teleport line will stop at a max distance
			for (int i = 0; i < numSegments; i++)
			{
				if (Physics.Raycast(lastPos, lastDir, out teleportHit, segmentLength, teleporter.validLayers))
				{
					points.Add(teleportHit.point);

					// if the hit point is valid
					if (Vector3.Angle(teleportHit.normal, Vector3.up) < teleporter.maxTeleportableSlope)
					{
						// define the point as a good teleportable point
						teleporter.Pos = teleportHit.point;
						Vector3 dir = avatar != null ? avatar.HeadPart.Transform.forward : Vector3.forward;
						dir = Vector3.ProjectOnPlane(dir, Vector3.up);

						teleporter.Dir = dir;
						teleporter.Active = true;
					}
					else
					{
						// if the hit point is close enough to the last valid point
						teleporter.Active = false;
					}


					break;
				}
				else
				{
					// add the point to the line renderer
					points.Add(lastPos);

					// calculate the next ray
					Vector3 newPos = lastPos + xVelocity + Vector3.up * yVelocity;
					lastDir = newPos - lastPos;
					segmentLength = (newPos - lastPos).magnitude;
					lastPos = newPos;
					yVelocity -= .01f;
				}
			}


			teleporterLineRenderer.positionCount = points.Count;
			teleporterLineRenderer.SetPositions(points.ToArray());
		}
	}
}