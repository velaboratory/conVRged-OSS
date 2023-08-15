using System;
using UnityEngine;
using UnityEngine.UI;
using unityutilities;

namespace ENGREDUVR
{
	public class TotalStationUI : MonoBehaviour
	{
		[HideInInspector]
		public TotalStation ts;
		[NonSerialized]
		public bool[] buttonInputs = new bool[32];

		[Header("Leveling Screen")]
		public Transform levelingScreen;
		public Text laserOn;
		public Text laserOnBrightness;
		public Text levelingX;
		public Text levelingY;
		public Transform bubble;
		public float bubbleMoveRadius = 250f;
		public float bubbleMoveMultiplier = 1000f;
		private Vector2 levelingValue = Vector2.zero;
		public float settlingSpeed = .5f;
		public float levelAsteriskCutoff = 0.2f;
		public bool levelReversed;

		[Header("OBS Screen")]
		public Transform obsScreen;
		public Transform anglesScreen;
		public Transform distancesScreen;
		public Transform P1BottomButtons;
		public Transform P2BottomButtons;
		public Transform P3BottomButtons;
		public Text pageIndicator;


		public Text SD;
		public Text ZA;
		public Transform HA_L;
		public Text HA_L_val;
		public Transform HA_R;
		public Text HA_R_val;



		public Text[] HD;

		public Text VD;



		[NonSerialized]
		public int pTNum = 2;
		[NonSerialized]
		public int stationPTNum = 1;

		public Transform oset;
		public Transform f_m;
		public Transform hold;

		[Header("Home Screen")]
		public Transform homeScreen;

		[Header("Splash Screen")]
		public Transform splashScreen;
		private float splashScreenTimer;
		private const float splashScreenTime = 2;

		[Header("Measuring Screen")]
		public Transform measuringScreen;

		[Header("EDM Screen")]
		public Transform edmScreen;
		public Transform[] edmPages = new Transform[2];

		[Header("Data Screen")]
		public Transform dataScreen;

		[Header("Menu Screen")]
		public Transform menuScreen;
		public TotalStationUIList menuScreenList;

		[Header("Coord Screen")]
		public Transform coordScreen;
		public TotalStationUIList coordScreenList;

		[Header("OccOrien Screen")]
		public Transform occOrienScreen;
		public TotalStationUIList occOrienScreenList;
		public Text occOrienPT;
		private string occOrientPTString;
		public Text occOrienHI;

		[Header("Backsight Screen")]
		public Transform backsightScreen;
		public Text backsightZA;
		public Text backsightHAR;
		public Text backsightHARSet;
		private string backsightHARString;

		[Header("Observation Screen")]
		public Transform observationScreen;
		public TotalStationUIList observationScreenList;
		public Text observationHR;
		public Text observationPT;
		public Transform recordedText;
		public Text N_text;
		public Text E_text;
		public Text Z_text;


		private const float blinkingHalfPeriod = .5f;
		private float blinkTimer;

		private const float measuringTime = 2;  // How long to stay on the Measuring... screen


		public void Start()
		{
			DisableAllScreens();

			ts.ui.screenStateTransition += StateTransition;
		}

		private void DisableAllScreens()
		{
			splashScreen.gameObject.SetActive(false);
			levelingScreen.gameObject.SetActive(false);
			measuringScreen.gameObject.SetActive(false);
			obsScreen.gameObject.SetActive(false);
			edmScreen.gameObject.SetActive(false);
			dataScreen.gameObject.SetActive(false);
			menuScreen.gameObject.SetActive(false);
			coordScreen.gameObject.SetActive(false);
			occOrienScreen.gameObject.SetActive(false);
			backsightScreen.gameObject.SetActive(false);
			observationScreen.gameObject.SetActive(false);
			homeScreen.gameObject.SetActive(false);
		}

		public void StateTransition(TotalStation.UI.ScreenState screenState)
		{
			DisableAllScreens();

			switch (screenState)
			{
				case TotalStation.UI.ScreenState.Off:
					break;
				case TotalStation.UI.ScreenState.Splash:
					splashScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Leveling:
					levelingScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.OBS:
					obsScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Home:
					homeScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Measuring:
					measuringScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.EDM:
					edmScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Data:
					dataScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Menu:
					menuScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Coord:
					coordScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.OccOrien:
					occOrienScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Backsight:
					backsightScreen.gameObject.SetActive(true);
					break;
				case TotalStation.UI.ScreenState.Observation:
					observationScreen.gameObject.SetActive(true);
					break;
			}
		}

		public void Update()
		{

			switch (ts.ui.Screen)
			{
				case TotalStation.UI.ScreenState.Off:
					break;
				case TotalStation.UI.ScreenState.Splash:
					splashScreenTimer += Time.deltaTime;
					if (splashScreenTimer > splashScreenTime)
					{
						splashScreenTimer = 0;
						ts.ui.Screen = TotalStation.UI.ScreenState.Leveling;
					}
					break;
				case TotalStation.UI.ScreenState.Leveling:
					UpdateLevelingScreen();
					break;
				case TotalStation.UI.ScreenState.OBS:
					if (ts.ui.ShowAnglesInsteadOfDistances)
					{
						ZA.text = AngleToDegMinSec(ts.VerticalAngle());
						if (!ts.ui.haHold)
						{
							HA_R_val.text = AngleToDegMinSec(ts.HorizontalAngle(Side.Right));
							HA_L_val.text = AngleToDegMinSec(ts.HorizontalAngle(Side.Left));
						}
						else
						{

						}
					}
					else
					{

					}

					switch (ts.ui.OBSPage)
					{
						case 0:
							Debug.LogError("Total Station Error. This state shouldn't be possible.");
							break;
						// P1
						case 1:
							// 0SET blinking
							if (ts.ui.osetBlinking)
							{
								blinkTimer += Time.deltaTime;
								if (blinkTimer > blinkingHalfPeriod)
								{
									oset.gameObject.SetActive(!oset.gameObject.activeSelf);
									blinkTimer = 0;
								}
							}
							else if (!oset.gameObject.activeSelf)
							{
								oset.gameObject.SetActive(true);
							}

							// Hold blinking
							if (ts.ui.holdBlinking)
							{
								blinkTimer += Time.deltaTime;
								if (blinkTimer > blinkingHalfPeriod)
								{
									hold.gameObject.SetActive(!hold.gameObject.activeSelf);
									blinkTimer = 0;
								}
							}
							else if (!hold.gameObject.activeSelf)
							{
								hold.gameObject.SetActive(true);
							}
							break;
						// P2
						case 2:
							break;
						// P3
						case 3:
							// F/M blinking
							if (ts.ui.f_mBlinking)
							{
								blinkTimer += Time.deltaTime;
								if (blinkTimer > blinkingHalfPeriod)
								{
									f_m.gameObject.SetActive(!f_m.gameObject.activeSelf);
									blinkTimer = 0;
								}
							}
							else if (!f_m.gameObject.activeSelf)
							{
								f_m.gameObject.SetActive(true);
							}
							break;


					}
					break;
				case TotalStation.UI.ScreenState.Home:
					break;
				case TotalStation.UI.ScreenState.Measuring:
					ts.ui.measuringScreenTimer += Time.deltaTime;
					if (ts.ui.measuringScreenTimer > measuringTime)
					{
						ts.ui.measuringScreenTimer = 0;
						ts.ui.Screen = ts.ui.previousScreen;
					}
					break;
				case TotalStation.UI.ScreenState.EDM:
					break;
				case TotalStation.UI.ScreenState.Data:
					break;
				case TotalStation.UI.ScreenState.Menu:
					break;
				case TotalStation.UI.ScreenState.Coord:
					break;
				case TotalStation.UI.ScreenState.OccOrien:
					break;
				case TotalStation.UI.ScreenState.Backsight:
					backsightZA.text = AngleToDegMinSec(ts.VerticalAngle());
					backsightHAR.text = AngleToDegMinSec(ts.VerticalAngle());
					break;
				case TotalStation.UI.ScreenState.Observation:
					break;
			}
		}


		/// <summary>
		/// Calculates and shows the on-screen bubble level
		/// </summary>
		public void UpdateLevelingScreen()
		{
			float deltaSettlingSpeed = settlingSpeed * Time.deltaTime;
			Vector2 levelVal = ts.Level;
			if (levelReversed)
			{
				levelVal.x = -levelVal.x;
				levelVal.y = -levelVal.y;
			}
			levelingValue.x = (levelVal.x * Mathf.Rad2Deg) * deltaSettlingSpeed + (levelingValue.x * (1 - deltaSettlingSpeed));
			levelingValue.y = (levelVal.y * Mathf.Rad2Deg) * deltaSettlingSpeed + (levelingValue.y * (1 - deltaSettlingSpeed));
			if (Mathf.Abs(levelingValue.x) > levelAsteriskCutoff)
			{
				levelingX.text = "X " + " ****";
			}
			else
			{
				levelingX.text = "X " + AngleToDegMinSec(levelingValue.x, false);
			}

			if (Mathf.Abs(levelingValue.y) > levelAsteriskCutoff)
			{
				levelingY.text = "Y " + " ****";
			}
			else
			{
				levelingY.text = "Y " + AngleToDegMinSec(levelingValue.y, false);
			}

			Debug.DrawRay(ts.baseCylinder.position, ts.baseCylinder.forward);

			bubble.localPosition = new Vector3(
				Mathf.Clamp(levelingValue.x * bubbleMoveMultiplier, -bubbleMoveRadius, bubbleMoveRadius),
				-Mathf.Clamp(levelingValue.y * bubbleMoveMultiplier, -bubbleMoveRadius, bubbleMoveRadius),
				0);
		}

		/// <summary>
		/// Converts a float angle in degrees to "deg° min' sec"
		/// </summary>
		/// <param name="angle">The angle to convert</param>
		/// <returns></returns>
		private static string AngleToDegMinSec(float angle, bool showDegrees = true)
		{
			// degrees, minutes, seconds
			int d = (int)angle;
			int m = (int)((angle - d) * 60);
			int s = Math.Abs((int)((angle - d - (m / 60f)) * 3600));
			string str = "";
			str += angle < 0 ? "-" : " ";
			if (showDegrees)
				str += Math.Abs(d) + "°";
			str += Math.Abs(m).ToString("D2") + "\'" + s.ToString("D2") + "\"";
			return str;
		}
	}
}