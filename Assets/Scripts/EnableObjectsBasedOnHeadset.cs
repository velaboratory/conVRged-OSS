using UnityEngine;
using unityutilities;

public class EnableObjectsBasedOnHeadset : MonoBehaviour
{
	public GameObject[] cv1Objects;
	public GameObject[] riftSQuestObjects;
	public GameObject[] quest2Objects;
	public GameObject[] other;

	private void Start()
	{
		// disable all objects first
		foreach (GameObject obj in cv1Objects)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in riftSQuestObjects)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in quest2Objects)
		{
			obj.SetActive(false);
		}

		foreach (GameObject obj in other)
		{
			obj.SetActive(false);
		}

		// then enable only the right section.
		// this arch is to allow the same object in multiple headset types
		switch (InputMan.controllerStyle)
		{
			case HeadsetControllerStyle.Rift:
			{
				foreach (GameObject obj in cv1Objects)
				{
					obj.SetActive(true);
				}

				break;
			}
			case HeadsetControllerStyle.RiftSQuest:
			{
				foreach (GameObject obj in riftSQuestObjects)
				{
					obj.SetActive(true);
				}

				break;
			}
			case HeadsetControllerStyle.Quest2:
			{
				foreach (GameObject obj in quest2Objects)
				{
					obj.SetActive(true);
				}

				break;
			}
			default:
			{
				foreach (GameObject obj in other)
				{
					obj.SetActive(true);
				}

				break;
			}
		}
	}
}