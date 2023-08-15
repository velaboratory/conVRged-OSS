using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unityutilities.OVR
{
	/// <summary>
	/// Makes input from VR devices accessible from a unified set of methods. Can treat axes as button down.
	/// </summary>
	[DefaultExecutionOrder(-90)]
	[AddComponentMenu("unityutilities/InputMan OVR Hands")]
	public class InputManOVRHands : MonoBehaviour
	{
		public OVRHand leftHand;
		public OVRHand rightHand;
		private OVRSkeleton leftHandSkeleton;
		private OVRSkeleton rightHandSkeleton;

		/// <summary>
		/// Contains a pair of bools for each axis input that can act as a button.
		/// The first is true only for the first frame the axis is active
		/// The second is true only for the first frame the axis is inactive
		/// The third remains true when the button is held.
		/// 	it represents whether the button was already down last frame
		/// </summary>
		protected static Dictionary<InputOVRHands, bool[,]> firstPressed = new Dictionary<InputOVRHands, bool[,]>();

		/// <summary>
		/// Same as above but at a lower threshold for hysteresis. A pinch will be detected sooner and for longer
		/// </summary>
		protected static Dictionary<InputOVRHands, bool[,]> firstPressedShallow = new Dictionary<InputOVRHands, bool[,]>();

		protected enum InputOVRHands
		{
			Pinch,
			MiddleFingerPinch,
		}

		private static InputManOVRHands instance;
		protected static bool init;

		protected static void Init()
		{
			if (!init)
			{
				instance = new GameObject("InputMan OVR Hands").AddComponent<InputManOVRHands>();
				DontDestroyOnLoad(instance.gameObject);
				init = true;
			}
		}

		private void Awake()
		{
			firstPressed = new Dictionary<InputOVRHands, bool[,]>
			{
				{ InputOVRHands.Pinch, new bool[2, 3] },
				{ InputOVRHands.MiddleFingerPinch, new bool[2, 3] },
			};
			firstPressedShallow = new Dictionary<InputOVRHands, bool[,]>
			{
				{ InputOVRHands.Pinch, new bool[2, 3] },
				{ InputOVRHands.MiddleFingerPinch, new bool[2, 3] },
			};
			leftHandSkeleton = leftHand.GetComponent<OVRSkeleton>();
			rightHandSkeleton = rightHand.GetComponent<OVRSkeleton>();

			instance = this;
			init = true;
		}

		public static OVRHand GetHand(Side side)
		{
			return side switch
			{
				Side.Left => instance.leftHand,
				Side.Right => instance.rightHand,
				_ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
			};
		}

		public static OVRSkeleton GetSkeleton(Side side)
		{
			return side switch
			{
				Side.Left => instance.leftHandSkeleton,
				Side.Right => instance.rightHandSkeleton,
				_ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
			};
		}


		public static Vector3 GetPinchPos(Side side)
		{
			OVRSkeleton skele = GetSkeleton(side);
			if (skele.Bones.Count > 0)
			{
				return (skele.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position +
				        skele.Bones[(int)OVRSkeleton.BoneId.Hand_ThumbTip].Transform.position) / 2f;
			}

			return Vector3.zero;
		}

		#region Pinch

		public static float PinchValue(Side side = Side.Either)
		{
			float left, right;
			switch (side)
			{
				case Side.Both:
					left = GetHand(Side.Left).IsDataHighConfidence ? GetHand(Side.Left).GetFingerPinchStrength(OVRHand.HandFinger.Index) : 0;
					right = GetHand(Side.Right).IsDataHighConfidence ? GetHand(Side.Right).GetFingerPinchStrength(OVRHand.HandFinger.Index) : 0;
					return Mathf.Min(left, right);
				case Side.Either:
					left = GetHand(Side.Left).IsDataHighConfidence ? GetHand(Side.Left).GetFingerPinchStrength(OVRHand.HandFinger.Index) : 0;
					right = GetHand(Side.Right).IsDataHighConfidence ? GetHand(Side.Right).GetFingerPinchStrength(OVRHand.HandFinger.Index) : 0;
					return Mathf.Max(left, right);
				case Side.None:
					return 0;
				default:
					return GetHand(side).IsDataHighConfidence ? GetHand(side).GetFingerPinchStrength(OVRHand.HandFinger.Index) : 0;
			}
		}

		public static bool Pinch(Side side = Side.Either)
		{
			bool left, right;
			switch (side)
			{
				case Side.Both:
					left = GetHand(Side.Left).GetFingerIsPinching(OVRHand.HandFinger.Index) && GetHand(Side.Left).IsDataHighConfidence;
					right = GetHand(Side.Right).GetFingerIsPinching(OVRHand.HandFinger.Index) && GetHand(Side.Right).IsDataHighConfidence;
					return left && right;
				case Side.Either:
					left = GetHand(Side.Left).GetFingerIsPinching(OVRHand.HandFinger.Index) && GetHand(Side.Left).IsDataHighConfidence;
					right = GetHand(Side.Right).GetFingerIsPinching(OVRHand.HandFinger.Index) && GetHand(Side.Right).IsDataHighConfidence;
					return left || right;
				case Side.None:
					return false;
				default:
					return GetHand(side).GetFingerIsPinching(OVRHand.HandFinger.Index) && GetHand(side).IsDataHighConfidence;
			}
		}
		
		public static bool PinchThreshold(Side side = Side.Either, float threshold = .5f)
		{
			bool left, right;
			switch (side)
			{
				case Side.Both:
					left = GetHand(Side.Left).GetFingerPinchStrength(OVRHand.HandFinger.Index) > threshold && GetHand(Side.Left).IsDataHighConfidence;
					right = GetHand(Side.Right).GetFingerPinchStrength(OVRHand.HandFinger.Index) > threshold && GetHand(Side.Right).IsDataHighConfidence;
					return left && right;
				case Side.Either:
					left = GetHand(Side.Left).GetFingerPinchStrength(OVRHand.HandFinger.Index) > threshold && GetHand(Side.Left).IsDataHighConfidence;
					right = GetHand(Side.Right).GetFingerPinchStrength(OVRHand.HandFinger.Index) > threshold && GetHand(Side.Right).IsDataHighConfidence;
					return left || right;
				case Side.None:
					return false;
				default:
					return GetHand(side).GetFingerPinchStrength(OVRHand.HandFinger.Index) > threshold && GetHand(side).IsDataHighConfidence;
			}
		}

		public static bool PinchDown(Side side = Side.Either)
		{
			return instance.GetRawDown(InputOVRHands.Pinch, side);
		}

		public static bool PinchUp(Side side = Side.Either)
		{
			return instance.GetRawUp(InputOVRHands.Pinch, side);
		}

		#endregion

		#region Middle Finger Pinch

		public static float MiddleFingerPinchValue(Side side = Side.Either)
		{
			float left, right;
			switch (side)
			{
				case Side.Both:
					left = GetHand(Side.Left).IsDataHighConfidence ? GetHand(Side.Left).GetFingerPinchStrength(OVRHand.HandFinger.Middle) : 0;
					right = GetHand(Side.Right).IsDataHighConfidence ? GetHand(Side.Right).GetFingerPinchStrength(OVRHand.HandFinger.Middle) : 0;
					return Mathf.Min(left, right);
				case Side.Either:
					left = GetHand(Side.Left).IsDataHighConfidence ? GetHand(Side.Left).GetFingerPinchStrength(OVRHand.HandFinger.Middle) : 0;
					right = GetHand(Side.Right).IsDataHighConfidence ? GetHand(Side.Right).GetFingerPinchStrength(OVRHand.HandFinger.Middle) : 0;
					return Mathf.Max(left, right);
				case Side.None:
					return 0;
				default:
					return GetHand(side).IsDataHighConfidence ? GetHand(side).GetFingerPinchStrength(OVRHand.HandFinger.Middle) : 0;
			}
		}

		public static bool MiddleFingerPinch(Side side = Side.Either)
		{
			bool left, right;
			switch (side)
			{
				case Side.Both:
					left = GetHand(Side.Left).GetFingerIsPinching(OVRHand.HandFinger.Middle) && GetHand(Side.Left).IsDataHighConfidence;
					right = GetHand(Side.Right).GetFingerIsPinching(OVRHand.HandFinger.Index) && GetHand(Side.Right).IsDataHighConfidence;
					return left && right;
				case Side.Either:
					left = GetHand(Side.Left).GetFingerIsPinching(OVRHand.HandFinger.Middle) && GetHand(Side.Left).IsDataHighConfidence;
					right = GetHand(Side.Right).GetFingerIsPinching(OVRHand.HandFinger.Middle) && GetHand(Side.Right).IsDataHighConfidence;
					return left || right;
				case Side.None:
					return false;
				default:
					return GetHand(side).GetFingerIsPinching(OVRHand.HandFinger.Middle) && GetHand(side).IsDataHighConfidence;
			}
		}

		public static bool MiddleFingerPinchDown(Side side = Side.Either)
		{
			return instance.GetRawDown(InputOVRHands.MiddleFingerPinch, side);
		}

		public static bool MiddleFingerPinchUp(Side side = Side.Either)
		{
			return instance.GetRawUp(InputOVRHands.MiddleFingerPinch, side);
		}

		#endregion


		protected bool GetRawDown(InputOVRHands key, Side side)
		{
			switch (side)
			{
				case Side.Both:
					return firstPressed[key][0, 0] &&
					       firstPressed[key][1, 0];
				case Side.Either:
					return firstPressed[key][0, 0] ||
					       firstPressed[key][1, 0];
				case Side.None:
					return false;
				default:
					return firstPressed[key][(int)side, 0];
			}
		}

		protected bool GetRawUp(InputOVRHands key, Side side)
		{
			switch (side)
			{
				case Side.Both:
					return firstPressedShallow[key][0, 1] &&
					       firstPressedShallow[key][1, 1];
				case Side.Either:
					return firstPressedShallow[key][0, 1] ||
					       firstPressedShallow[key][1, 1];
				case Side.None:
					return false;
				default:
					return firstPressedShallow[key][(int)side, 1];
			}
		}


		private void UpdateDictionary(bool currentVal, int side, InputOVRHands key, Dictionary<InputOVRHands, bool[,]> dict)
		{
			// if down right now
			if (currentVal)
			{
				// if it wasn't down last frame
				if (!dict[key][side, 2] && !dict[key][side, 0])
				{
					// activate the down event 
					dict[key][side, 0] = true;
				}
				else
				{
					// deactive the down event
					dict[key][side, 0] = false;
				}

				// save that the input was down for next frame
				dict[key][side, 2] = true;
			}
			// if up right now
			else
			{
				// if it wasn't up last frame
				if (dict[key][side, 2] && !dict[key][side, 1])
				{
					// activate the up event
					dict[key][side, 1] = true;
				}
				else
				{
					// deactivate the up event
					dict[key][side, 1] = false;
				}

				// save that the input was up for next frame
				dict[key][side, 2] = false;

				dict[key][side, 0] = false;
			}
		}

		private void Update()
		{
			for (int i = 0; i < 2; i++)
			{
				UpdateDictionary(Pinch((Side)i), i, InputOVRHands.Pinch, firstPressed);
				UpdateDictionary(PinchThreshold((Side)i, .1f), i, InputOVRHands.Pinch, firstPressedShallow);
				UpdateDictionary(MiddleFingerPinch((Side)i), i, InputOVRHands.MiddleFingerPinch, firstPressed);
			}
		}
	}
}