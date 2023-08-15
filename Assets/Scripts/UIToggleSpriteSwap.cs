using UnityEngine;
using UnityEngine.UI;

public class UIToggleSpriteSwap : MonoBehaviour
{
	public Toggle toggle;
	public Transform onSprite;
	public Transform offSprite;

	private void Start()
	{
		toggle.onValueChanged.AddListener(ValueChanged);

		ValueChanged(toggle.isOn);
	}

	void ValueChanged(bool on)
	{
		if (on)
		{
			onSprite.gameObject.SetActive(true);
			offSprite.gameObject.SetActive(false);
		}
		else
		{
			onSprite.gameObject.SetActive(false);
			offSprite.gameObject.SetActive(true);
		}
	}
}
