using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using unityutilities;
using unityutilities.Interaction.WorldMouse;

namespace MenuTablet
{
#if UNITY_EDITOR
	[CustomEditor(typeof(MenuTabletMover))]
	[CanEditMultipleObjects]
	public class MenuTabletMoverEditor : Editor
	{
		MenuTabletMover menuTabletMover;

		private void OnEnable()
		{
			menuTabletMover = target as MenuTabletMover;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (!menuTabletMover.GetComponent<MenuTabletToggler>())
			{
				if (GUILayout.Button("Add MenuTabletToggler"))
				{
					MenuTabletToggler toggler = menuTabletMover.gameObject.AddComponent<MenuTabletToggler>();
					toggler.tablet = menuTabletMover;
				}
			}
		}
	}
#endif


	/// <summary>
	/// 📑
	/// </summary>
	public class MenuTabletMover : MonoBehaviour
	{
		private bool visible;
		public bool startVisible = false;
		public static bool isVisible => instance.visible;

		public static MenuTabletMover instance;
		public GameObject tabletGraphics;

		public Transform head;
		public Transform hand;
		public bool detached;

		/// <summary>
		/// This is not used to move the tablet locally, but sent over the network
		/// </summary>
		public Side side;

		public static Action<MenuTabletMover> OnShow;
		public static Action<MenuTabletMover> OnHide;

		[Tooltip(
			"Set this to false to only be able to toggle the tablet with a manual reference. Static references use the global tablet.")]
		public bool isGlobalInstance = true;

		public bool findWorldMouseCanvasesOnToggle;
		public bool dontDestroyOnLoad = true;

		private void Awake()
		{
			if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

			if (isGlobalInstance) instance = this;
		}

		private void Start()
		{
			if (head == null)
			{
				head = FindObjectOfType<Rig>().head;
			}

			ShowTabletInstance(startVisible);
		}

		private void Update()
		{
			if (!visible) return;

			// don't move tablet if detached
			if (detached) return;

			if (head == null)
			{
				Debug.LogError("Head not set for visible tablet.");
				return;
			}

			if (hand != null)
			{
				MoveTablet(hand, 10f);
			}
			else if (head != null)
			{
				MoveTablet(head.position + head.forward * .5f + -head.up * .3f, 10f);
			}
		}

		/// <summary>
		/// Moves the tablet to a hand obj
		/// </summary>
		/// <param name="targetPosition">The position of the hand</param>
		/// <param name="time">The time it takes to get there. Will have to be called every frame to actually get there. 0 to move instantly.</param>
		private void MoveTablet(Transform targetPosition, float time = 0)
		{
			MoveTablet(targetPosition.position, time);
		}

		/// <summary>
		/// Moves the tablet to a global position
		/// </summary>
		/// <param name="targetPosition">The position to move to</param>
		/// <param name="time">The time it takes to get there. Will have to be called every frame to actually get there. 0 to move instantly.</param>
		private void MoveTablet(Vector3 targetPosition, float time = 0)
		{
			Vector3 diff = head.forward;
			diff.y = 0;

			Vector3 goalPos = targetPosition + (Vector3.up * .25f) + (diff.normalized * .10f);

			transform.position = time != 0 ? Vector3.Lerp(transform.position, goalPos, time * Time.deltaTime) : goalPos;

			transform.LookAt(head.position);
		}

		/// <summary>
		/// Toggles this tablet
		/// </summary>
		/// <param name="show">True to show, false to hide.</param>
		public void ShowTabletInstance(bool show)
		{
			tabletGraphics.gameObject.SetActive(show);

			switch (show)
			{
				case true when !visible:
					OnShow?.Invoke(this);
					if (findWorldMouseCanvasesOnToggle) WorldMouseInputModule.FindCanvases();
					
					// immediately teleport tablet
					if (hand != null)
					{
						MoveTablet(hand, 0);
					}
					else if (head != null)
					{
						MoveTablet(head.position + head.forward * .5f + -head.up * .3f, 0);
					}
					break;
				case false when visible:
					OnHide?.Invoke(this);
					break;
			}

			detached = false;
			visible = show;
		}

		/// <summary>
		/// Toggles the global tablet instance
		/// </summary>
		/// <param name="show">True to show, false to hide.</param>
		public static void ShowTablet(bool show)
		{
			instance.ShowTabletInstance(show);
		}

		/// <summary>
		/// Toggles the local tablet instance
		/// </summary>
		public void ToggleTabletInstance()
		{
			ShowTabletInstance(!visible);
		}
		
		/// <summary>
		/// Detaches the tablet from a hand so it stays in space
		/// </summary>
		public void DetachTabletInstance()
		{
			detached = true;
		}
		
		/// <summary>
		/// Detaches the tablet from a hand so it stays in space
		/// </summary>
		public static void DetachTablet()
		{
			instance.detached = true;
		}

		public static void ToggleTablet()
		{
			ShowTablet(!instance.visible);
		}

		public static void ToggleTablet(Transform hand, Side side)
		{
			instance.hand = hand;
			instance.side = side;
			ShowTablet(!instance.visible);

			// bool showOrHide;
			// if (instance.visible)
			// {
			//     lastHandTransform = null;
			//     instance.ShowTablet(false, side);
			//     showOrHide = false;
			// }
			// else
			// {
			//     lastHandTransform = watchHand;
			//     instance.ShowTablet(true, side);
			//     instance.MoveTablet(watchHand);
			//     showOrHide = true;
			// }
		}
	}
}