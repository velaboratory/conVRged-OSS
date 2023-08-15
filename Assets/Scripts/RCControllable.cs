using UnityEngine;
using unityutilities;

public class RCControllable : MonoBehaviour
{
	[ReadOnly]
	public float horizontalInput;
	[ReadOnly]
	public float verticalInput;
	[ReadOnly]
	public float yawInput;
	[ReadOnly]
	public float altitudeInput;
	[ReadOnly]
	public bool activelyControlled;
}
