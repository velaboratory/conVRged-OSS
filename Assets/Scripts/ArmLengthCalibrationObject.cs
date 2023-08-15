using UnityEngine;
using unityutilities.VRInteraction;

public class ArmLengthCalibrationObject : MonoBehaviour
{
	public VRGrabbable obj;
	public MenuTabletController menuTabletController;
	public Renderer edgeRend;
	public Color grabbingColor = Color.red;
	private Color originalColor;

	// Start is called before the first frame update
	private void Start()
	{
		originalColor = edgeRend.material.color;
		obj.Released += () =>
		{
			menuTabletController.StopArmLengthCalibration();
			edgeRend.material.color = originalColor;
		};
		obj.Grabbed += () => { edgeRend.material.color = grabbingColor; };
	}
}