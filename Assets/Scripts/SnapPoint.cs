using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{

	public enum Type
	{
		TotalStation,
		TrainPiece
	}

	public Type pointType;
	public GameObject ghost;
	public bool snapped;
	public Transform currentObject;
	
}
