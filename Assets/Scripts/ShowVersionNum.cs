using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowVersionNum : MonoBehaviour
{
	private void Awake() {
		GetComponent<Text>().text = "v " + Application.version;
	}
}
