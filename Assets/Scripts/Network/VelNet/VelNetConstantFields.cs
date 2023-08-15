using unityutilities;
using VelNet;

public class VelNetConstantFields : LoggerConstantFields
{
	public override string[] GetConstantFields()
	{
		return new[]
		{
			VelNetManager.Room,
			VelNetManager.LocalPlayer?.userid.ToString() ?? "-1",
			VelNetMan.NickName,
			GameManager.instance.player.trackedHandsVisible ? "hands" : "controllers"
		};
	}
}