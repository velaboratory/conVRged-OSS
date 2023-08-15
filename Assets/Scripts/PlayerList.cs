using System.Collections;
using System.Linq;
using conVRged;
using TMPro;
using UnityEngine;
using VelNet;

public class PlayerList : MonoBehaviour
{
	public GameObject playerPrefab;
	public Transform parent;

	private IEnumerator Start()
	{
		int lastLength = 0;
		while (true)
		{
			yield return new WaitForSeconds(2);
			if (lastLength != VelNetManager.PlayerCount)
			{
				yield return new WaitForSeconds(1);
				RefreshList();
			}

			lastLength = VelNetManager.PlayerCount;
		}
	}

	private void RefreshList()
	{
		// clear the old entries
		foreach (Transform child in parent)
		{
			Destroy(child.gameObject);
		}

		foreach (VelNetWolf3DAvatarController p in VelNetMan.playerPrefabs.Values.ToList())
		{
			GameObject buttonObj = Instantiate(playerPrefab, parent);
			TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
			text.text = p.nickName;
		}
	}
}