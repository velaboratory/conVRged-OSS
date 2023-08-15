using UnityEngine;

public class TotalStationUIList : MonoBehaviour {
	public TotalStationUIListItem[] listItems;
	private byte currentIndex;

	public byte CurrentIndex {
		get => currentIndex;
		set {
			currentIndex = value;
			Refresh();
		}
	}

	public void Up() {
		if (CurrentIndex > 0) {
			CurrentIndex--;
		}

		Refresh();
	}

	public void Down() {
		if (CurrentIndex < listItems.Length - 1) {
			CurrentIndex++;
		}

		Refresh();
	}

	public void Refresh() {
		for (int i = 0; i < listItems.Length; i++) {
			if (i == CurrentIndex) {
				listItems[i].HoverOn();
			}
			else {
				listItems[i].HoverOff();
			}
		}
	}

}
