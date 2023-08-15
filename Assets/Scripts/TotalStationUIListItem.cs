using UnityEngine;
using UnityEngine.UI;

public class TotalStationUIListItem : MonoBehaviour
{
	public Color normalTextColor = Color.black;
	public Color highlightedColor = new Color(195,195,195);
	public Text text;
	public Transform background;

	public void HoverOn() {
		text.color = highlightedColor;
		background.gameObject.SetActive(true);
	}

	public void HoverOff() {
		text.color = normalTextColor;
		background.gameObject.SetActive(false);
	}
}
