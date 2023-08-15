using MenuTablet;
using UnityEngine;
using UnityEngine.UI;
using unityutilities;

public class WatchController : MonoBehaviour
{
	public Side side;
	public Transform hudMenu;
	private Transform cam;
	private Transform t;
	public float angleCutoff = 20;
	public Toggle muteToggle;
	public Transform palmMenu;
	public bool showMuteButton;

	private void Start()
	{
		if (Camera.main != null) cam = Camera.main.transform;
		t = transform;

		if (side == Side.Right)
		{
			Vector3 p = palmMenu.localPosition;
			p.x *= -1;
			palmMenu.localPosition = p;
		}
	}

	private void Update()
	{
		if (cam == null) return;
		float watchAngle = Vector3.Angle(t.up, cam.transform.position - t.position);
		float headAngle = Vector3.Angle(-cam.forward, cam.transform.position - t.position);
		if (showMuteButton)
		{
			hudMenu.gameObject.SetActive(watchAngle < angleCutoff && headAngle < angleCutoff);
			muteToggle.SetIsOnWithoutNotify(VelNetMan.microphoneSetting == VelNetMan.MicrophoneSetting.Off);
		}

		palmMenu.gameObject.SetActive(watchAngle > 150 && headAngle < 50);

	}

	public void ToggleTablet()
	{
		MenuTabletMover.ToggleTablet(transform, side);
		MenuTabletMover.DetachTablet();
	}

	public void SetMuted(bool muted)
	{
		VelNetMan.microphoneSetting = muted ? VelNetMan.MicrophoneSetting.Off : VelNetMan.MicrophoneSetting.On;
	}

	public void SetLaser(bool on)
	{
		if (side == Side.Left)
		{
			GameManager.instance.player.leftHand.sharedLaserShowing = on;
		}
		else
		{
			GameManager.instance.player.rightHand.sharedLaserShowing = on;
		}
	}
}