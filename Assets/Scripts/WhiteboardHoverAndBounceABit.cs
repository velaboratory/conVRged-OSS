using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardHoverAndBounceABit : MonoBehaviour
{
	public float height = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit)) {
			transform.Translate(0, height - hit.distance, 0);
		}
    }
}
