using VelNet;
using UnityEngine;
using TMPro;

public class ShowOwnerAndIsMine : MonoBehaviour
{
	public NetworkObject netObj;
	public TMP_Text text;

	[Space] 
	
	public bool isMine;
	public bool ownerActorNumber;
	public bool ownerName = true;

	public string ownerPrepend = "Owner: ";

	private void Update()
	{
		text.text = (isMine ? $"IsMine: {netObj.IsMine}\n" : "") +
		            (ownerActorNumber ? $"Owner Actor Number: {netObj.owner?.userid}" : "") +
		            (ownerName ? $"{ownerPrepend}{VelNetMan.GetPlayerPrefab(netObj.owner)?.nickName ?? "NONE"}" : "");
	}
}