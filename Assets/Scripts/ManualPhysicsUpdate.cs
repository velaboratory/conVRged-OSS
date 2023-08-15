using UnityEngine;

public class ManualPhysicsUpdate : MonoBehaviour
{
	/// <summary>
	/// This prevents objects from falling through the floor and stuff like that.
	/// </summary>
	public float maxDeltaTime = .1f;

	private void Awake()
	{
		Physics.autoSimulation = false;
	}

	private void Update()
	{
		Physics.Simulate(Mathf.Clamp(Time.deltaTime, 0, maxDeltaTime));
	}
}