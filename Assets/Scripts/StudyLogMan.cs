using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VelNet;

namespace ENGREDUVR
{

	public class StudyLogMan : MonoBehaviour
	{
		enum CurrentStudyState : byte
		{
			doingNothing,
			settingUp,
			measuring
		}

		static public StudyLogMan instance;

		static CurrentStudyState curr = CurrentStudyState.doingNothing;

		static float setUpStartTime;

		public List<Transform> levelingPoints;

		public static Vector2 levelness;

		public static float distanceToClosestSurveyPoint = float.MaxValue;

		public unityutilities.VRInteraction.VRGrabbable grabbable;

		private static int stuckCount = 0;

		private void Start()
		{
			grabbable.Grabbed += tripodPickedUp;
			instance = this;

			SceneManager.sceneLoaded += SceneLoaded;
		}

		private static void SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			stuckCount = 0;
			curr = 0;
		}

		static void advanceState()
		{
			CurrentStudyState currTemp = curr;
			curr++;

			//data
			List<string> data = new List<string> {
				"state_change_event",
				currTemp.ToString() + " -> " + curr.ToString(),
			};

			//add time delta if done with set up
			if (curr == CurrentStudyState.measuring)
			{
				data.Add((Time.time - setUpStartTime).ToString());
				data.Add(distanceToClosestSurveyPoint.ToString());
				data.Add(levelness.x.ToString());
				data.Add(levelness.y.ToString());
				data.Add(stuckCount.ToString());
			}

			//log
			unityutilities.Logger.LogRow("study_data", data.ToArray());

		}


		public static void tripodPickedUp()
		{
			if (curr == CurrentStudyState.doingNothing)
			{
				advanceState();
				setUpStartTime = Time.time;
			}
		}

		public static void levelScreenLeft(Vector2 vector)
		{
			if (curr == CurrentStudyState.settingUp)
			{
				levelness = vector * Mathf.Rad2Deg;
				advanceState();
			}
		}

		public static void reset()
		{
			if (curr == CurrentStudyState.measuring)
			{
				curr = CurrentStudyState.doingNothing;
			}
		}

		public static void updateDistanceToSurveyPoints(Vector3 point)
		{
			if (instance != null)
			{
				float shortestDistance = float.MaxValue;
				foreach (Transform t in instance.levelingPoints)
				{
					if (Vector3.Distance(t.position, point) < shortestDistance)
					{
						shortestDistance = Vector3.Distance(t.position, point);
					}
				}
				distanceToClosestSurveyPoint = shortestDistance;
			}
		}

		public static void stickLeg(bool stuck, string objName)
		{
			stuckCount += stuck ? 1 : -1;

			//data
			List<string> data = new List<string> {
				VelNetManager.PlayerCount.ToString(),
				"leg_stick_event",
				curr.ToString(),
				objName,
				stuck?"stuck":"unstuck",
			};

			//log
			unityutilities.Logger.LogRow("study_data", data);
		}


		public static void logSlideShow(int start, int next)
		{
			//data
			List<string> data = new List<string> {
				VelNetManager.PlayerCount.ToString(),
				"slide_show_change_event",
				curr.ToString(),
				start + " -> " + next,
			};

			//log
			unityutilities.Logger.LogRow("study_data", data);
		}


	}

}
