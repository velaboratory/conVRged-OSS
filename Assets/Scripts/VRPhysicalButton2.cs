using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.UI.Button;

/// <summary>
/// If the rb is beyond the activate pos, the button is pressed,
/// If the rb is beyond the neutral pos, the button becomes unpressed,
/// If the rigidbody is between the neutral and activation positions, then the button maintains it's pressed state
/// Button physics is up to the user of the script
/// </summary>
public class VRPhysicalButton2 : MonoBehaviour
{
	// The distance between these two positions is the hysteresis buffer
	public Transform releaseBeyond;
	public Transform pressBeyond;

	[Space] public Rigidbody rb;
	private bool pressed;

	[Header("Color and sound")] public Renderer rend;
	private Color normalColor;
	public Color clickedColor;
	public AudioSource sound;

	[Space] public ButtonClickedEvent onClick;

	private enum PressState
	{
		BeyondPressed,
		Between,
		BeyondReleased
	}

	private void Start()
	{
		if (rb == null) rb = GetComponentInChildren<Rigidbody>();
		if (rend == null) rend = rb.GetComponentInChildren<MeshRenderer>();

		if (rend)
		{
			normalColor = rend.sharedMaterial.color;
		}
	}

	private void FixedUpdate()
	{
		Vector3 currentPos = rb.transform.position;
		float distanceToRelease = Vector3.Distance(currentPos, releaseBeyond.position);
		float distanceToPress = Vector3.Distance(currentPos, pressBeyond.position);
		float distanceBetween = Vector3.Distance(pressBeyond.position, releaseBeyond.position);
		// could law of cosines here instead
		PressState currentState;
		if (distanceToRelease > distanceBetween && distanceToRelease > distanceToPress)
		{
			currentState = PressState.BeyondPressed;
		}
		else if (distanceToPress > distanceBetween && distanceToPress > distanceToRelease)
		{
			currentState = PressState.BeyondReleased;
		}
		else
		{
			currentState = PressState.Between;
		}

		switch (currentState)
		{
			case PressState.BeyondPressed when !pressed:
				Press();
				break;
			case PressState.BeyondReleased when pressed:
				Release();
				break;
		}
	}

	public void Press()
	{
		Debug.Log("Press");
		onClick.Invoke();
		if (sound)
		{
			sound.Play();
		}

		pressed = true;
		if (rend)
		{
			rend.material.color = clickedColor;
		}
	}

	public void Release()
	{
		Debug.Log("Release");
		pressed = false;
		if (rend)
		{
			rend.material.color = normalColor;
		}
	}
}


#if UNITY_EDITOR

[CustomEditor(typeof(VRPhysicalButton2))]
public class VRPhysicalButton2Editor : Editor
{
	public override void OnInspectorGUI()
	{
		VRPhysicalButton2 button = target as VRPhysicalButton2;

		DrawDefaultInspector();

		if (GUILayout.Button("Click Button") && button != null)
		{
			button.Press();
		}
	}
}
#endif