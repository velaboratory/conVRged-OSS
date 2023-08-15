using UnityEngine;

namespace ENGREDUVR
{
	public class TabsView : MonoBehaviour
	{
		public RectTransform[] tabs;
		public Transform[] tabsContent;

		public Vector2 normalTabSize = new Vector2(280, 55);
		public Vector2 selectedTabSize = new Vector2(280, 65);

		public void SelectTab(int tabId)
		{
			if (tabId > tabs.Length) return;

			for (int i = 0; i < tabs.Length; i++)
			{
				if (i == tabId)
				{
					tabs[i].sizeDelta = selectedTabSize;
					tabsContent[i].gameObject.SetActive(true);
				}
				else
				{
					tabs[i].sizeDelta = normalTabSize;
					tabsContent[i].gameObject.SetActive(false);
				}
			}
		}
	}
}