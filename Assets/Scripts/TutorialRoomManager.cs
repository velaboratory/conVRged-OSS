using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using unityutilities;

public class TutorialRoomManager : MonoBehaviour
{
	public ElevatorDoorOpener elevatorDoorOpener;
	private bool finished;

	public Transform testSpeakerIcon;

	// Start is called before the first frame update
	private IEnumerator Start()
	{
		if (GameManager.instance.lastLaunchedVersion != Application.version)
		{
			yield return new WaitForSeconds(1);
			ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Trigger, "Point at a button on the wall and press the trigger to select");
			yield return new WaitForSeconds(10);
			ControllerHelp.HideAllHints();
			ControllerHelp.ShowHint(Side.Both, ControllerHelp.ButtonHintType.Thumbstick, "Push forward on the thumbstick to teleport");
			yield return new WaitForSeconds(10);
			ControllerHelp.HideAllHints();
		}
	}

	public void TestAudio()
	{
		StopAllCoroutines();
		StartCoroutine(TestAudioCo());
	}

	private IEnumerator TestAudioCo()
	{
		// FindObjectOfType<DissonanceComms>(). = true;
		testSpeakerIcon.gameObject.SetActive(true);
		yield return new WaitForSeconds(5);
		// FindObjectOfType<Recorder>().DebugEchoMode = false;
		testSpeakerIcon.gameObject.SetActive(false);
	}

	public void ReloadMic()
	{
		// FindObjectOfType<Recorder>().RestartRecording(true);
	}

	public void ResetAvatar()
	{
		VelNetMan.SetAvatarURL("", true);
		SceneMan.LoadScene(SceneManager.GetActiveScene().name);
	}
}