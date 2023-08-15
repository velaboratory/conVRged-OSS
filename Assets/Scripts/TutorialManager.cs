using System.Collections;
using MenuTablet;
using UnityEngine;
using UnityEngine.SceneManagement;
using unityutilities;
using unityutilities.VRInteraction;

namespace ENGREDUVR {
	public class TutorialManager : MonoBehaviour {
		public DoorOpener[] doorOpeners;
		public AudioSource correctSound;

		[Header("Teleport Area")]
		public SendActionOnTriggerEnter movedIntoTeleportArea;

		[Header("Color Picker")]
		// public PlayerColorPicker colorPicker;
		public SendActionOnTriggerEnter movedIntoColorArea;

		[Header("Name Choosing")]
		public NickNameField nickNameField;

		[Header("Grabbing")]
		public SendActionOnTriggerEnter movedIntoObjArea;
		public SendActionOnTriggerEnter moveObjIntoBin;
		public VRDial lever;
		public AvatarPlayback grabObjTut;
		public GameObject blueCubeSpawnLoc;
		public VRMoveable grabObj;
		public SendActionOnTriggerEnter grabFloorTrigger;
		public GameObject pickUpBlueCubeCanvas;
		public GameObject pullLeverCanvas;
		private bool objInBin;

		[Header("Whiteboard")]
		public bool whiteboardTutEnabled = true;
		public SendActionOnTriggerEnter movedIntoWhiteboardArea;
		public SendActionOnTriggerEnter touchedWhiteboard;
		public AvatarPlayback whiteboardTut;

		[Header("Tablet")]
		public SendActionOnTriggerEnter movedIntoTabletArea;
		public AvatarPlayback tabletTut;



		private Vector3 watchOffset = new Vector3(-.05f, -.02f, -.1f);
		private Vector3 watchDir = new Vector3(-.15f, -.01f, -.1f);




		// Start is called before the first frame update
		IEnumerator Start() {
			// colorPicker.colorChanged += OpenDoor1;
			nickNameField.nameFinished += OpenDoor2;
			moveObjIntoBin.triggerEntered += ObjIntoBin;
			movedIntoColorArea.triggerEntered += MovedIntoColorArea;
			movedIntoTeleportArea.triggerEntered += MovedIntoTeleportArea;
			movedIntoObjArea.triggerEntered += MovedIntoObjArea;
			grabObj.Grabbed += GrabbedObj;
			grabFloorTrigger.triggerEntered += ResetBlueCube;
			movedIntoWhiteboardArea.triggerEntered += MovedIntoWhiteboardArea;
			touchedWhiteboard.triggerEntered += TouchedWhiteBoard;
			movedIntoTabletArea.triggerEntered += MovedIntoTabletArea;
			MenuTabletMover.OnShow += ShowTablet;

			yield return null;
			yield return null;
			yield return null;
			yield return null;

			ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Thumbstick, "Push forward on the thumbstick to teleport");
		}

		// Update is called once per frame
		void Update() {
			if (doorOpeners[1].open) {
				if (!doorOpeners[2].open && lever.currentAngle < -35) {
					OpenDoor3();
				}
			}
		}

		private void ResetBlueCube() {
			grabObj.transform.position = blueCubeSpawnLoc.transform.position;
			grabObj.transform.rotation = blueCubeSpawnLoc.transform.rotation;
		}

		private void MovedIntoTabletArea() {
			ControllerHelp.HideAllHints();
			// ControllerHelp.ControllerLabel[] labels = ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.OtherObject, "Tap on your watch");
			// labels[0].offset[0] = watchOffset;
			// labels[0].offset[1] = watchDir;
			// labels[1].offset[0] = watchOffset;
			// labels[1].offset[1] = watchDir;
			//
			// // flip the right sideMe
			// labels[1].offset[0].x *= -1;
			// labels[1].offset[1].x *= -1;
			
			ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Button1, "Press B/Y");
			

			tabletTut.Play();
		}

		private void MovedIntoWhiteboardArea() {
			ControllerHelp.HideAllHints();

			// disable the whiteboard tutorial
			if (whiteboardTutEnabled)
			{
				whiteboardTut.Play();
			} else
			{
				if (!doorOpeners[2].open) return;

				ControllerHelp.HideAllHints();

				if (!doorOpeners[3].OpenDoor())
				{
					correctSound.Play();
				}
			}

		}

		private void TouchedWhiteBoard() {
			if (!doorOpeners[2].open) return;

			ControllerHelp.HideAllHints();

			if (!doorOpeners[3].OpenDoor()) {
				correctSound.Play();
			}
		}

		private void GrabbedObj()
		{
			pickUpBlueCubeCanvas.SetActive(false);
			pullLeverCanvas.SetActive(true);
			ControllerHelp.HideAllHints();
		}

		private void MovedIntoObjArea() {
			if (!doorOpeners[2].open) {
				ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Trigger, "Use the trigger to grab");
				grabObjTut.Play();
			}
		}

		private void MovedIntoTeleportArea() {
			ControllerHelp.HideAllHints();
			correctSound.Play();
		}

		private void MovedIntoColorArea() {
			ControllerHelp.HideAllHints();
			if (!doorOpeners[0].open) {
				ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Trigger, "Point at a color and press the trigger to select");
			}
		}

		private void OpenDoor1() {
			if (!doorOpeners[0].OpenDoor()) {
				correctSound.Play();
			}
			ControllerHelp.HideAllHints();
		}

		private void OpenDoor2() {
			if (!doorOpeners[1].OpenDoor()) {
				correctSound.Play();
			}
		}

		private void ObjIntoBin() {
			objInBin = true;
			correctSound.Play();
		}

		private void OpenDoor3() {
			if (objInBin) {
				doorOpeners[2].OpenDoor();
				correctSound.Play();
			}
		}

		private void ShowTablet(MenuTabletMover tablet) {
			if (doorOpeners[2].open) {
				ControllerHelp.HideAllHints();
			}
		}

	}
}
