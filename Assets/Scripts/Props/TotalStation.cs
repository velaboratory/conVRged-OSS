using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ENGREDUVR;
using UnityEngine;
using UnityEngine.Serialization;
using unityutilities;
using unityutilities.VRInteraction;
using VelNet;
using Logger = unityutilities.Logger;

// namespace ENGREDUVR
// {
	public class TotalStation : SyncState
	{
		[Header("Moving parts")] public Transform baseCylinder;

		[FormerlySerializedAs("baseAdjust_distFront")] [Header("Tuning Parameters")]
		// To adjust the speed of the tilt dials, change the max and min limits on the dials themselves.
		public float baseAdjustDistFront = .02f;

		public float baseAdjustDistLeft = .02f;
		public float baseAdjustDistRight = .02f;

		private const float minBaseAdjust = .015f;
		private const float maxBaseAdjust = .025f; //.5cm

		[Header("Dials")] public VRDial baseFrontDial;
		public VRDial baseLeftDial;
		public VRDial baseRightDial;
		public VRDial lensDial;
		public VRDial stationDial;
		public VRLocalConstrainedMoveable shifter;

		[HideInInspector] public float lastHorizontal;
		private Vector3 lastVertical;

		/// <summary>
		/// PTName, x, y, z (in unity coords (would be e, z, n)
		/// </summary>
		public Action<string, double, double, double> measureAction;


		//public float LevelX => baseCylinder.worldToLocalMatrix.MultiplyVector(Vector3.up).x;
		//public float LevelY => baseCylinder.worldToLocalMatrix.MultiplyVector(Vector3.up).z;

		/// <summary>
		/// Returns the X and Y values of the total station on-screen level in radians
		/// </summary>
		public Vector2 Level
		{
			get
			{
				// should point up
				var stationDialTransform = stationDial.transform;
				Vector3 level = stationDialTransform.up;
				level.y = 0;

				// should point to the side
				Vector3 right = stationDialTransform.right;
				right.y = 0;
				right.Normalize();

				// should point forward
				Vector3 forward = -stationDial.transform.forward;
				forward.y = 0;
				forward.Normalize();

				float x = -Vector3.Dot(level, right);
				float y = -Vector3.Dot(level, forward);
				return new Vector2(x, y);
			}
		}

		[Space] public TotalStationUI[] UIs;

		[Space] public bool drawLaserOnMeasure;

		private readonly List<byte[]> localStateHistory = new List<byte[]>();

		private AudioSource audioSourceBeep;


		private readonly List<TotalStationMeasurement> measuredPoints = new List<TotalStationMeasurement>();

		private TotalStationMeasurement MeasuredData
		{
			get { return measuredPoints.Last(); }
		}

		public class StringEntereableFloat
		{
			public StringEntereableFloat(string value)
			{
				this.strVal = value;
			}

			public int maxStringLength = 8;
			string strVal;

			public string StringValue
			{
				get => strVal;
				set
				{
					strVal = value;
					OnChange?.Invoke(strVal);
				}
			}

			public float FloatValue
			{
				get
				{
					float.TryParse(strVal, out float val);
					return val;
				}
				set
				{
					strVal = value.ToString("N3");
					OnChange?.Invoke(strVal);
				}
			}

			/// <summary>
			/// Easily networked form. 
			/// Creates a constant-length byte array defined with length defined in maxStringLength.
			/// </summary>
			public byte[] BytesValue
			{
				get
				{
					byte[] arr = new byte[maxStringLength];
					char[] charArr = StringValue.ToCharArray();
					for (int i = 0; i < maxStringLength; i++)
					{
						if (i >= charArr.Length)
						{
							arr[i] = 0;
						}
						else
						{
							arr[i] = (byte)charArr[i];
						}
					}

					return arr;
				}
				set
				{
					StringBuilder strBuild = new StringBuilder();
					foreach (var c in value)
					{
						strBuild.Append((char)c);
					}

					StringValue = strBuild.ToString();
				}
			}


			public Action<string> OnChange;
		}

		/// <summary>
		/// Height of the instrument
		/// </summary>
		public StringEntereableFloat HI = new StringEntereableFloat("1.500");

		/// <summary>
		/// Height of the Reflector
		/// </summary>
		public StringEntereableFloat HR = new StringEntereableFloat("1.500");

		// TODO not yet used
		public StringEntereableFloat InstrumentN = new StringEntereableFloat("0");
		public StringEntereableFloat InstrumentE = new StringEntereableFloat("0");
		public StringEntereableFloat InstrumentZ = new StringEntereableFloat("0");

		// TODO not yet set or used
		public StringEntereableFloat BacksightN = new StringEntereableFloat("0");
		public StringEntereableFloat BacksightE = new StringEntereableFloat("0");
		public StringEntereableFloat BacksightZ = new StringEntereableFloat("0");


		/// <summary>
		/// Name of the measured point
		/// </summary>
		/// 
		public int PTName
		{
			get => pTNum;
			set
			{
				pTNum = value;
				foreach (var ui in UIs)
				{
					ui.observationPT.text = pTName + pTNum.ToString();
				}
			}
		}

		[NonSerialized] public string pTName = "PT";
		[NonSerialized] public int pTNum = 1;

		/// <summary>
		/// Name of the total station point
		/// </summary>
		/// 
		public int StationPTName
		{
			get => stationPTNum;
			set
			{
				stationPTNum = value;
				foreach (var ui in UIs)
				{
					ui.occOrienPT.text = stationPTName + stationPTNum.ToString();
				}
			}
		}

		[NonSerialized] public string stationPTName = "ST";
		[NonSerialized] public int stationPTNum = 1;

		public class UI : INetworkPack
		{
			public TotalStation ts;

			public enum ScreenState
			{
				Off,
				Splash,
				Leveling,
				OBS,
				Home,
				Measuring,
				EDM,
				Data,
				Menu,
				Coord,
				OccOrien,
				Backsight,
				Observation
			}

			private ScreenState screen;
			private bool powerOn;
			public Action<ScreenState> screenStateTransition;


			/// <summary>
			/// Shows angles when true, distances when false
			/// </summary>
			public bool ShowAnglesInsteadOfDistances
			{
				get => showAnglesInsteadOfDistances;
				set
				{
					showAnglesInsteadOfDistances = value;
					foreach (var ui in ts.UIs)
					{
						ui.anglesScreen.gameObject.SetActive(value);
						ui.distancesScreen.gameObject.SetActive(!value);
					}
				}
			}

			private bool showAnglesInsteadOfDistances = true;

			public ScreenState Screen
			{
				get => screen;
				set
				{
					previousScreen = screen;
					screen = value;
					screenStateTransition?.Invoke(value);
				}
			}

			public bool PowerOn
			{
				get => powerOn;
				set
				{
					powerOn = value;
					Screen = value ? ScreenState.Splash : ScreenState.Off;
					ShowAnglesInsteadOfDistances = true;
					OBSPage = 1;
					MenuScreenListIndex = 0;
					ObservationScreenListIndex = 0;
					OccOrienScreenListIndex = 0;
					CoordScreenListIndex = 0;
				}
			}

			/// <summary>
			/// OBS Page: [P1, P2, P3]
			/// </summary>
			public int OBSPage
			{
				get { return currentPage; }
				set
				{
					currentPage = value;
					foreach (TotalStationUI ui in ts.UIs)
					{
						ui.pageIndicator.text = "P" + currentPage;
						ui.P1BottomButtons.gameObject.SetActive(currentPage == 1);
						ui.P2BottomButtons.gameObject.SetActive(currentPage == 2);
						ui.P3BottomButtons.gameObject.SetActive(currentPage == 3);
					}
				}
			}

			private int currentPage = 1;

			public int edmPage;

			#region Data Properties (SD, HD, etc.)

			private float SD_val;

			public float SD_Val
			{
				get { return SD_val; }
				set
				{
					SD_val = value;
					foreach (var ui in ts.UIs)
					{
						ui.SD.text = SD_val.ToString("N3") + "";
					}
				}
			}

			private float HD_val;

			public float HD_Val
			{
				get { return HD_val; }
				set
				{
					HD_val = value;
					foreach (var ui in ts.UIs)
					{
						ui.HD[0].text = HD_val.ToString("N3") + "m";
						ui.HD[1].text = HD_val.ToString("N3") + "m";
					}
				}
			}

			private float VD_val;

			public float VD_Val
			{
				get { return VD_val; }
				set
				{
					VD_val = value;
					foreach (var ui in ts.UIs)
					{
						ui.VD.text = VD_val.ToString("N3") + "m";
					}
				}
			}


			public double N_Val
			{
				get => n_val;
				set
				{
					n_val = value;
					foreach (var ui in ts.UIs)
					{
						ui.N_text.text = n_val.ToString("N3");
					}
				}
			}

			private double n_val;

			public double E_Val
			{
				get => e_val;
				set
				{
					e_val = value;
					foreach (var ui in ts.UIs)
					{
						ui.E_text.text = e_val.ToString("N3");
					}
				}
			}

			private double e_val;

			public double Z_Val
			{
				get => z_val;
				set
				{
					z_val = value;
					foreach (var ui in ts.UIs)
					{
						ui.Z_text.text = z_val.ToString("N3");
					}
				}
			}

			private double z_val;

			#endregion

			#region List Indices

			public byte MenuScreenListIndex
			{
				get { return ts.UIs[0].menuScreenList.CurrentIndex; }
				set
				{
					foreach (var ui in ts.UIs)
					{
						ui.menuScreenList.CurrentIndex = value;
					}
				}
			}

			public byte ObservationScreenListIndex
			{
				get { return ts.UIs[0].observationScreenList.CurrentIndex; }
				set
				{
					foreach (var ui in ts.UIs)
					{
						ui.observationScreenList.CurrentIndex = value;
					}
				}
			}

			public byte OccOrienScreenListIndex
			{
				get { return ts.UIs[0].occOrienScreenList.CurrentIndex; }
				set
				{
					foreach (var ui in ts.UIs)
					{
						ui.occOrienScreenList.CurrentIndex = value;
					}
				}
			}

			public byte CoordScreenListIndex
			{
				get { return ts.UIs[0].coordScreenList.CurrentIndex; }
				set
				{
					foreach (var ui in ts.UIs)
					{
						ui.coordScreenList.CurrentIndex = value;
					}
				}
			}

			#endregion


			public ScreenState previousScreen; // The screen to return to after measuring

			public bool osetBlinking;
			public bool f_mBlinking;
			public bool holdBlinking;

			public float measuringScreenTimer;

			/// <summary>
			/// HA-L or HA-R
			/// </summary>
			public bool useHAL = false;

			public bool haHold;


			#region Networking

			public byte[] PackData()
			{
				using MemoryStream outputStream = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(outputStream);

				writer.Write(powerOn);
				writer.Write((int)Screen);
				writer.Write(OBSPage);
				writer.Write(ShowAnglesInsteadOfDistances);
				writer.Write(SD_Val);
				writer.Write(HD_Val);
				writer.Write(VD_Val);
				writer.Write(ts.lastHorizontal);
				writer.Write(ts.HI.BytesValue);
				writer.Write(ts.HR.BytesValue);
				writer.Write(ts.InstrumentN.BytesValue);
				writer.Write(ts.InstrumentE.BytesValue);
				writer.Write(ts.InstrumentZ.BytesValue);
				writer.Write(ts.BacksightN.BytesValue);
				writer.Write(ts.BacksightE.BytesValue);
				writer.Write(ts.BacksightZ.BytesValue);
				writer.Write(MenuScreenListIndex);
				writer.Write(ObservationScreenListIndex);
				writer.Write(OccOrienScreenListIndex);
				writer.Write(CoordScreenListIndex);
				writer.Write((int)previousScreen);
				writer.Write(N_Val);
				writer.Write(E_Val);
				writer.Write(Z_Val);
				writer.Write(ts.PTName);
				writer.Write(ts.StationPTName);
				writer.Write(ts.LaserPlummetEnabled);

				return outputStream.ToArray();
			}

			public void UnpackData(byte[] data)
			{
				using MemoryStream inputStream = new MemoryStream(data);
				BinaryReader reader = new BinaryReader(inputStream);

				powerOn = reader.ReadBoolean();

				Screen = (ScreenState)reader.ReadInt32();

				OBSPage = reader.ReadInt32();
				ShowAnglesInsteadOfDistances = reader.ReadBoolean();
				SD_Val = reader.ReadSingle();
				HD_Val = reader.ReadSingle();
				VD_Val = reader.ReadSingle();
				ts.lastHorizontal = reader.ReadSingle();

				ts.HI.BytesValue = reader.ReadBytes(ts.HI.maxStringLength);
				ts.HR.BytesValue = reader.ReadBytes(ts.HR.maxStringLength);

				ts.InstrumentN.BytesValue = reader.ReadBytes(ts.InstrumentN.maxStringLength);
				ts.InstrumentE.BytesValue = reader.ReadBytes(ts.InstrumentE.maxStringLength);
				ts.InstrumentZ.BytesValue = reader.ReadBytes(ts.InstrumentZ.maxStringLength);

				ts.BacksightN.BytesValue = reader.ReadBytes(ts.BacksightN.maxStringLength);
				ts.BacksightE.BytesValue = reader.ReadBytes(ts.BacksightE.maxStringLength);
				ts.BacksightZ.BytesValue = reader.ReadBytes(ts.BacksightZ.maxStringLength);

				MenuScreenListIndex = reader.ReadByte();
				ObservationScreenListIndex = reader.ReadByte();
				OccOrienScreenListIndex = reader.ReadByte();
				CoordScreenListIndex = reader.ReadByte();

				previousScreen = (ScreenState)reader.ReadInt32();

				N_Val = reader.ReadDouble();
				E_Val = reader.ReadDouble();
				Z_Val = reader.ReadDouble();

				ts.PTName = reader.ReadInt32();
				ts.StationPTName = reader.ReadInt32();

				ts.LaserPlummetEnabled = reader.ReadBoolean();
			}

			#endregion

			/// <summary>
			/// Handle button presses in the UI
			/// </summary>
			/// <param name="buttonId">Which button was pressed</param>
			public void ButtonPress(int buttonId)
			{
				switch (Screen)
				{
					#region Splash

					case ScreenState.Splash:
						break;

					#endregion

					#region Leveling

					case ScreenState.Leveling:
						// OK
						if (buttonId == 1)
						{
							if (ts.Level.magnitude < .1f * Mathf.Deg2Rad)
							{
								if (previousScreen != ScreenState.Splash)
								{
									Screen = previousScreen;
								}
								else
								{
									Screen = ScreenState.OBS;
								}

								//study data
								if (Physics.Raycast(ts.baseCylinder.position, -ts.baseCylinder.up, out RaycastHit hit,
									    10, LayerMask.GetMask("Ground")))
								{
									//keep track off distance to the surveying points for logging
									StudyLogMan.updateDistanceToSurveyPoints(hit.point);
								}

								StudyLogMan.levelScreenLeft(ts.Level);
							}
						}
						// ESC
						else if (buttonId == 5)
						{
							if (previousScreen != ScreenState.Splash)
							{
								Screen = previousScreen;
							}
							else
							{
								Screen = ScreenState.OBS;
							}
						}
						// Laser Toggle
						else if (buttonId == 2)
						{
							ts.LaserPlummetEnabled = !ts.LaserPlummetEnabled;
						}

						break;

					#endregion

					#region OBS

					case ScreenState.OBS:

						#region P1

						if (OBSPage == 1)
						{
							// MEAS
							if (buttonId == 1)
							{
								if (ts.Level.magnitude < .1f * Mathf.Deg2Rad)
								{
									previousScreen = Screen;
									Screen = ScreenState.Measuring;
									ts.Measure();
								}
								else
								{
									Screen = ScreenState.Leveling;
								}
							}
							// SHV
							else if (buttonId == 2)
							{
								ShowAnglesInsteadOfDistances = !ShowAnglesInsteadOfDistances;
							}
							// 0SET
							else if (buttonId == 3)
							{
								if (!osetBlinking)
								{
									osetBlinking = true;
								}
								else
								{
									osetBlinking = false;
									ts.ZeroHorizontal();
								}
							}
							// HOLD
							else if (false && buttonId == 4)
							{
								if (haHold)
								{
									// update last angle and stuff
								}
								else
								{
									if (!holdBlinking)
									{
										holdBlinking = true;
									}
									else
									{
										holdBlinking = false;
										haHold = true;
									}
								}
							}
							// FUNC
							else if (buttonId == 8)
							{
								OBSPage = 2;
							}
						}

						#endregion

						#region P2

						else if (OBSPage == 2)
						{
							// MENU
							if (buttonId == 1)
							{
								// switch to menu screen
								Screen = ScreenState.Menu;
							}
							// TILT
							else if (buttonId == 2)
							{
								Screen = ScreenState.Leveling;
							}
							// H-SET
							else if (buttonId == 3)
							{
							}
							// EDM
							else if (buttonId == 4)
							{
								Screen = ScreenState.EDM;
							}
							// FUNC
							else if (buttonId == 8)
							{
								OBSPage = 3;
							}
						}

						#endregion

						#region P3

						else if (OBSPage == 3)
						{
							// R/L
							if (buttonId == 1)
							{
								useHAL = !useHAL;
							}
							// F/M
							// temporarily shows HD in meters rather than feet
							else if (false && buttonId == 2)
							{
								f_mBlinking = true;
							}
							// COORD
							else if (buttonId == 3)
							{
								Screen = ScreenState.Coord;
							}
							// S - O
							else if (buttonId == 4)
							{
							}
							// FUNC
							else if (buttonId == 8)
							{
								OBSPage = 1;
							}
						}

						#endregion

						if (buttonId == 5)
						{
							Screen = ScreenState.Home;
						}

						break;

					#endregion

					#region Home

					case ScreenState.Home:
						// OBS
						if (buttonId == 1)
						{
							Screen = ScreenState.OBS;
						}

						// DATA
						if (buttonId == 3)
						{
							Screen = ScreenState.Data;
						}
						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.OBS;
						}

						break;

					#endregion

					#region Measuring

					case ScreenState.Measuring:
						// STOP
						if (buttonId == 4)
						{
							Screen = previousScreen;
							measuringScreenTimer = 0;
						}
						// ESC
						else if (buttonId == 5)
						{
							Screen = previousScreen;
							measuringScreenTimer = 0;
						}

						break;

					#endregion

					#region EDM

					case ScreenState.EDM:
						// Up
						if (buttonId == 25)
						{
							if (edmPage > 0)
							{
								edmPage--;
							}
						}
						// Down
						else if (buttonId == 26)
						{
							if (edmPage < ts.UIs[0].edmPages.Length - 1)
							{
								edmPage++;
							}
						}
						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.Menu;
						}

						break;

					#endregion

					#region Data

					case ScreenState.Data:
						// ESC
						if (buttonId == 5)
						{
							Screen = ScreenState.OBS;
						}

						break;

					#endregion

					#region Menu

					case ScreenState.Menu:
						// Up
						if (buttonId == 25)
						{
							foreach (var ui in ts.UIs)
							{
								ui.menuScreenList.Up();
							}
						}
						// Down
						else if (buttonId == 26)
						{
							foreach (var ui in ts.UIs)
							{
								ui.menuScreenList.Down();
							}
						}
						// ENTER
						else if (buttonId == 24)
						{
							switch (MenuScreenListIndex)
							{
								case 0:
									Screen = ScreenState.Coord;
									break;
							}
						}
						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.OBS;
						}

						break;

					#endregion

					#region Coord

					case ScreenState.Coord:
						// Up
						if (buttonId == 25)
						{
							foreach (var ui in ts.UIs)
							{
								ui.coordScreenList.Up();
							}
						}
						// Down
						else if (buttonId == 26)
						{
							foreach (var ui in ts.UIs)
							{
								ui.coordScreenList.Down();
							}
						}
						// ENTER
						else if (buttonId == 24)
						{
							switch (CoordScreenListIndex)
							{
								// Occ.Orien.
								case 0:
									Screen = ScreenState.OccOrien;
									break;
								// Observation
								case 1:
									Screen = ScreenState.Observation;
									break;
								// EDM
								case 2:
									Screen = ScreenState.EDM;
									break;
							}
						}
						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.Menu;
						}

						break;

					#endregion

					#region OccOrien

					case ScreenState.OccOrien:
						// Up
						if (buttonId == 25)
						{
							foreach (var ui in ts.UIs)
							{
								ui.occOrienScreenList.Up();
							}
						}
						// Down
						else if (buttonId == 26)
						{
							foreach (var ui in ts.UIs)
							{
								ui.occOrienScreenList.Down();
							}
						}

						// BS AZ
						if (buttonId == 2)
						{
							Screen = ScreenState.Backsight;
						}
						// BS NEZ
						else if (buttonId == 3)
						{
						}
						// RESEC
						else if (buttonId == 4)
						{
						}

						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.Coord;
						}

						// Enter text
						switch (OccOrienScreenListIndex)
						{
							// N0
							case 0:
								ts.InstrumentN.StringValue = GetKeyPress(buttonId, ts.InstrumentN.StringValue);
								break;
							// E0
							case 1:
								ts.InstrumentE.StringValue = GetKeyPress(buttonId, ts.InstrumentE.StringValue);
								break;
							// Z0
							case 2:
								ts.InstrumentZ.StringValue = GetKeyPress(buttonId, ts.InstrumentZ.StringValue);
								break;
							// PT
							case 3:
								// LEFT
								if (buttonId == 27)
								{
									if (ts.StationPTName > 0)
									{
										ts.StationPTName--;
									}
								}

								// RIGHT
								else if (buttonId == 28)
								{
									ts.StationPTName++;
								}

								break;
							// HI
							case 4:
								ts.HI.StringValue = GetKeyPress(buttonId, ts.HI.StringValue);
								break;
						}

						break;

					#endregion

					#region Backsight

					case ScreenState.Backsight:
						// REC
						if (buttonId == 1)
						{
						}
						// OK
						else if (buttonId == 4)
						{
							Screen = ScreenState.OccOrien;
						}

						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.OccOrien;
						}

						break;

					#endregion

					#region Observation

					case ScreenState.Observation:
						// Up
						if (buttonId == 25)
						{
							foreach (var ui in ts.UIs)
							{
								ui.observationScreenList.Up();
							}
						}
						// Down
						else if (buttonId == 26)
						{
							foreach (var ui in ts.UIs)
							{
								ui.observationScreenList.Down();
							}
						}

						// REC
						if (buttonId == 1)
						{
						}
						// OFFSET
						else if (buttonId == 2)
						{
						}
						// AUTO
						else if (buttonId == 3)
						{
						}
						// MEAS
						else if (buttonId == 4)
						{
							if (ts.Level.magnitude > .1f * Mathf.Deg2Rad)
							{
								Screen = ScreenState.Leveling;
							}
							else
							{
								previousScreen = Screen;
								Screen = ScreenState.Measuring;
								ts.Measure();
							}
						}

						// ESC
						else if (buttonId == 5)
						{
							Screen = ScreenState.Coord;
						}

						switch (ObservationScreenListIndex)
						{
							// HR
							case 0:
								ts.HR.StringValue = GetKeyPress(buttonId, ts.HR.StringValue);
								break;
							// PT
							case 1:
								// LEFT
								if (buttonId == 27)
								{
									if (ts.PTName > 0)
									{
										ts.PTName--;
									}
								}

								// RIGHT
								else if (buttonId == 28)
								{
									ts.PTName++;
								}

								break;
						}

						break;

					#endregion
				}

				#region Any state

				if (buttonId == 22)
				{
					PowerOn = !PowerOn;
				}

				#endregion
			}


			private string GetKeyPress(int buttonId, string text)
			{
				if (buttonId == 10)
				{
					text += "0";
				}
				else if (buttonId == 11)
				{
					text += "1";
				}
				else if (buttonId == 12)
				{
					text += "2";
				}
				else if (buttonId == 13)
				{
					text += "3";
				}
				else if (buttonId == 14)
				{
					text += "4";
				}
				else if (buttonId == 15)
				{
					text += "5";
				}
				else if (buttonId == 16)
				{
					text += "6";
				}
				else if (buttonId == 17)
				{
					text += "7";
				}
				else if (buttonId == 18)
				{
					text += "8";
				}
				else if (buttonId == 19)
				{
					text += "9";
				}
				else if (buttonId == 9)
				{
					if (!text.Contains("."))
					{
						text += ".";
					}
				}

				// LEFT
				else if (buttonId == 27)
				{
					if (text.Length > 0)
					{
						text = text.Substring(0, text.Length - 1);
					}
				}

				// RIGHT
				else if (buttonId == 28)
				{
					text = "";
				}

				// ENTER
				else if (buttonId == 24)
				{
					//text = "";
				}

				return text;
			}
		}

		public UI ui = new UI();

		// data saved about a point when MEAS is pressed
		public class TotalStationMeasurement
		{
			public Vector3 origin;
			public Vector3 point;
			public float HA; // HA-R
			public float VA;

			public TotalStationMeasurement(Vector3 origin, Vector3 point, float ha, float va)
			{
				this.origin = origin;
				this.point = point;
				this.HA = ha;
				this.VA = va;
			}

			/// <summary>
			/// Should only be called when the tilt is within 6" limit
			/// </summary>
			/// <returns>The horizontal distance from the total station to the target as measured by the total station</returns>
			public float HD()
			{
				// method based on VA
				//float distance = Distance();
				//bool backsighting = VA > 180f;
				//return Mathf.Cos((90 - VA) * Mathf.Deg2Rad) * distance * (backsighting ? -1 : 1);


				// Old incorrect (correct?) calculation
				Vector3 flatOrigin = origin;
				Vector3 flatPoint = point;
				flatOrigin.y = 0;
				flatPoint.y = 0;
				return Vector3.Distance(flatOrigin, flatPoint);
			}

			/// <summary>
			/// Should only be called when the tilt is within 6" limit
			/// </summary>
			/// <returns>The horizontal distance from the total station to the target as measured by the total station</returns>
			public float VD()
			{
				// method based on VA
				//float distance = Distance();
				//return Mathf.Sin((90 - VA) * Mathf.Deg2Rad) * distance;

				// Old incorrect (correct?) calculation
				return point.y - origin.y;
			}

			/// <summary>
			/// The straight-line distance from ts to hit point
			/// Should only be called when the tilt is within 6" limit
			/// </summary>
			/// <returns>The straight-line distance from ts to hit point</returns>
			public float Distance()
			{
				return Vector3.Distance(point, origin);
			}
		}


		protected override void Awake()
		{
			base.Awake();
			
			foreach (TotalStationUI ui in UIs)
			{
				ui.ts = this;
			}

			ui.ts = this;

			HI.OnChange += delegate(string str)
			{
				foreach (var ui in UIs)
				{
					ui.occOrienHI.text = str;
				}
			};

			HR.OnChange += delegate(string str)
			{
				foreach (var ui in UIs)
				{
					ui.observationHR.text = str;
				}
			};
		}

		// Use this for initialization
		void Start()
		{
			audioSourceBeep = GetComponent<AudioSource>();
			localStateHistory.Add(ui.PackData());
			ZeroHorizontal();
			SetDialListeners();

			Adjust();
		}

		public void Measure()
		{
			if (!Physics.Raycast(lensDial.transform.position, -lensDial.transform.up, out RaycastHit hit)) return;

			if (drawLaserOnMeasure)
			{
				GameObject g = new GameObject("TotalStationShot");
				var position = lensDial.transform.position;
				g.transform.position = position;
				LineRenderer line = g.AddComponent<LineRenderer>();
				line.SetPositions(new[] { position, hit.point });
				line.widthMultiplier = .003f;
				Material material;
				(material = line.material).shader = Shader.Find("Unlit/Color");
				material.color = Color.red;
			}

			// Check the special case that we hit a prism. Make the collision point more accurate in this case.
			if (hit.collider.CompareTag("prism"))
			{
				// not required right now due to assymetrical collider arrangement on the prism.
				// may be used in the future if better grab volumes are needed
			}

			// the only things the ts REALLY knows are the distance, HA, and VA.
			float distance = hit.distance;
			float va = VerticalAngle();
			float ha = HorizontalAngle(Side.Right);

			bool backsighting = va > 180f;
			float prismHeight = HR.FloatValue;
			float tsHeight = HI.FloatValue;

			float x = Mathf.Sin(ha * Mathf.Deg2Rad) * distance * (backsighting ? -1 : 1);
			float y = Mathf.Sin((90f - va) * Mathf.Deg2Rad) * distance + (tsHeight - prismHeight);
			float z = Mathf.Cos(ha * Mathf.Deg2Rad) * distance * (backsighting ? -1 : 1);

			Vector3
				measuredPoint =
					new Vector3(x, y, z); // The point the total station thinks it measured in the total station space.
			Debug.Log(measuredPoint);


			measuredPoints.Add(new TotalStationMeasurement(lensDial.transform.position, hit.point,
				HorizontalAngle(Side.Right), VerticalAngle()));

			LogButtonPress(-1);

			// Update OBS screen
			ui.SD_Val = Distance();
			ui.HD_Val = HorizontalDistance();
			ui.VD_Val = VerticalDistance();

			// update Observation Screen
			ui.N_Val = z;
			ui.E_Val = x;
			ui.Z_Val = y;

			measureAction?.Invoke(UIs[0].observationPT.text, x, y, z);
		}

		public float Distance()
		{
			return MeasuredData.Distance();
		}

		public float HorizontalDistance()
		{
			return MeasuredData.HD();
		}

		public float VerticalDistance()
		{
			return MeasuredData.VD();
		}

		/// <summary>
		/// Gets the HA value of the total station
		/// </summary>
		/// <param name="side">Which direction to measure in. L or R.</param>
		/// <returns>the HA-R or HA-L value as shown by the total station</returns>
		public float HorizontalAngle(Side side = Side.Right)
		{
			float currentHorizontal = stationDial.currentAngle;
			float angle = currentHorizontal - lastHorizontal;
			if (side == Side.Left)
			{
				angle = -angle;
			}
			else if (side != Side.Right)
			{
				Debug.LogError("Neither left nor right. Something is wrong!", this);
			}

			while (angle < 0)
			{
				angle += 360;
			}

			while (angle > 360)
			{
				angle -= 360;
			}

			return angle;
		}


		/// <summary>
		/// Gets the VA value as shown by the total station
		/// </summary>
		/// <returns>The VA value as shown by the total station</returns>
		public float VerticalAngle()
		{
			// not corrected using dual-axis compensator
			//Vector3 currentVertical = stationDial.transform.worldToLocalMatrix.MultiplyVector(-lensDial.transform.forward);
			//float angle = Vector3.SignedAngle(lastVertical, currentVertical, Vector3.right);

			// corrected using dual-axis compensator
			Vector3 currentVertical = -lensDial.transform.forward;
			float angle = Vector3.SignedAngle(Vector3.up, currentVertical, lensDial.transform.right);
			angle -= 90;
			// TODO add check for within 6'


			while (angle < 0)
			{
				angle += 360;
			}

			while (angle > 360)
			{
				angle -= 360;
			}

			return angle;
		}

		private void SetDialListeners()
		{
			baseFrontDial.DialTurned += (currentAngle, amount, localInput) =>
			{
				baseAdjustDistFront = RemapAngleToHeight(currentAngle, baseFrontDial.minAngle, baseFrontDial.maxAngle);
				Adjust();
			};
			baseAdjustDistFront =
				RemapAngleToHeight(baseFrontDial.currentAngle, baseFrontDial.minAngle, baseFrontDial.maxAngle);

			baseLeftDial.DialTurned += (currentAngle, amount, localInput) =>
			{
				baseAdjustDistLeft = RemapAngleToHeight(currentAngle, baseLeftDial.minAngle, baseLeftDial.maxAngle);
				Adjust();
			};
			baseAdjustDistLeft =
				RemapAngleToHeight(baseLeftDial.currentAngle, baseLeftDial.minAngle, baseLeftDial.maxAngle);

			baseRightDial.DialTurned += (currentAngle, amount, localInput) =>
			{
				baseAdjustDistRight = RemapAngleToHeight(currentAngle, baseRightDial.minAngle, baseRightDial.maxAngle);
				Adjust();
			};
			baseAdjustDistRight =
				RemapAngleToHeight(baseRightDial.currentAngle, baseRightDial.minAngle, baseRightDial.maxAngle);

			lensDial.DialTurned += (currentAngle, amount, localInput) => { };
			stationDial.DialTurned += (currentAngle, amount, localInput) => { };
		}

		/// <summary>
		/// The max and min angles are used to calculate the factor for the three tilting dials
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="origMin"></param>
		/// <param name="origMax"></param>
		/// <returns></returns>
		private float RemapAngleToHeight(float angle, float origMin = 0, float origMax = 360)
		{
			float range = origMax - origMin;

			while (angle > origMax)
			{
				angle -= range;
			}

			while (angle < origMin)
			{
				angle += range;
			}

			return ((angle - origMin) / range) * (maxBaseAdjust - minBaseAdjust) + minBaseAdjust;
		}

		/// <summary>
		/// Adjusts the tilt of the Total Station from the three base knobs
		/// </summary>
		private void Adjust()
		{
			baseAdjustDistFront = Mathf.Clamp(baseAdjustDistFront, minBaseAdjust, maxBaseAdjust);
			baseAdjustDistLeft = Mathf.Clamp(baseAdjustDistLeft, minBaseAdjust, maxBaseAdjust);
			baseAdjustDistRight = Mathf.Clamp(baseAdjustDistRight, minBaseAdjust, maxBaseAdjust);
			var transform1 = baseFrontDial.transform;
			Vector3 p1 = transform1.position + transform1.up * baseAdjustDistFront;
			var transform2 = baseLeftDial.transform;
			Vector3 p2 = transform2.position + transform2.up * baseAdjustDistLeft;
			var transform3 = baseRightDial.transform;
			Vector3 p3 = transform3.position + transform3.up * baseAdjustDistRight;

			Vector3 centroid = (p1 + p2 + p3) / 3.0f;
			Vector3 forward = (centroid - p1).normalized;
			Vector3 right = (p3 - p2).normalized;
			Vector3 up = Vector3.Cross(forward, right).normalized;
			baseCylinder.rotation = Quaternion.LookRotation(forward, up);
			centroid.y += 0.002f;
			baseCylinder.position = centroid;
		}

		public void ZeroHorizontal()
		{
			if (!baseCylinder)
			{
				Debug.Log("no baseCylinder");
				return;
			}

			lastHorizontal = stationDial.currentAngle;
		}

		#region Laser Plummet

		private bool laserPlummetEnabled;
		private LineRenderer laserPlummet;

		public bool LaserPlummetEnabled
		{
			get => laserPlummetEnabled;
			set
			{
				if (value && laserPlummet == null)
				{
					laserPlummet = baseCylinder.gameObject.AddComponent<LineRenderer>();
					laserPlummet.useWorldSpace = false;
					laserPlummet.SetPositions(new[] { Vector3.zero, Vector3.up * -10 });
					//laserPlummet.SetPositions(new Vector3[] { baseCylinder.position, baseCylinder.position - Vector3.up * 10 });
					laserPlummet.widthMultiplier = .001f;
					Material material;
					(material = laserPlummet.material).shader = Shader.Find("Unlit/Color");
					Color color = Color.red;
					color.a = .1f;
					material.color = color;
				}

				if (laserPlummet != null)
				{
					laserPlummet.enabled = value;
				}

				foreach (var ui in UIs)
				{
					ui.laserOnBrightness.gameObject.SetActive(value);
				}

				laserPlummetEnabled = value;
			}
		}

		#endregion

		public void ButtonPress(int buttonId)
		{
			networkObject.TakeOwnership();
			ui.ButtonPress(buttonId);

			foreach (TotalStationUI totalStationUI in UIs)
			{
				totalStationUI.UpdateLevelingScreen();
			}

			if (audioSourceBeep)
			{
				audioSourceBeep.Play();
			}

			LogButtonPress(buttonId);
		}

		private void LogButtonPress(int buttonId)
		{
			StringList l = new StringList(new List<dynamic>()
			{
				buttonId.ToString(),
				baseCylinder.position,
				baseCylinder.rotation,
				lensDial.currentAngle,
				stationDial.currentAngle,
				stationDial.transform.rotation,
				UIs[0].levelingX.text,
				UIs[0].levelingY.text
			});

			Logger.LogRow("total_station_button_event", l.List);
		}

		[VelNetRPC]
		public void TotalStationButtonPress(int viewId, byte[] lastState, byte[] currentState)
		{
			//if the new old state doesnt match our current state, then there was a networking conflict.
			//Iterate backwards through our history until we hit the last state that matches the old state, then revert to it.
			if (!lastState.SequenceEqual(localStateHistory.Last()))
			{
				bool found = false;
				for (int i = localStateHistory.Count - 2; i >= 0 && !found; i--)
				{
					if (lastState.SequenceEqual(localStateHistory[i]))
					{
						Debug.Log("State reverted because of collision");
						currentState = localStateHistory[i];
						localStateHistory.RemoveRange(0, i);
						found = true;
					}
				}

				if (!found) Debug.LogError("No state match in history. This is a bug.");
			}

			//add changed state to history
			localStateHistory.Add(currentState);

			//update ui's
			ui.UnpackData(currentState);

			if (audioSourceBeep)
			{
				audioSourceBeep.Play();
			}
		}

		protected override void SendState(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(baseFrontDial.currentAngle);
			binaryWriter.Write(baseLeftDial.currentAngle);
			binaryWriter.Write(baseRightDial.currentAngle);
			binaryWriter.Write(lensDial.currentAngle);
			binaryWriter.Write(stationDial.currentAngle);
			binaryWriter.Write(shifter.transform.localPosition.x);
			binaryWriter.Write(shifter.transform.localPosition.z);
			binaryWriter.Write(ui.PackData());
		}

		protected override void ReceiveState(BinaryReader binaryReader)
		{
			baseFrontDial.SetData(binaryReader.ReadSingle(), false);
			baseLeftDial.SetData(binaryReader.ReadSingle(), false);
			baseRightDial.SetData(binaryReader.ReadSingle(), false);
			lensDial.SetData(binaryReader.ReadSingle(), false);
			stationDial.SetData(binaryReader.ReadSingle(), false);
			shifter.transform.localPosition = new Vector3(
				binaryReader.ReadSingle(),
				shifter.transform.localPosition.y,
				binaryReader.ReadSingle()
			);

			// reads the remaining bytes and sends them to the UI
			long length = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
			ui.UnpackData(binaryReader.ReadBytes((int)length));
		}
	}
// }