using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorJack : MonoBehaviour
{

    public float angle = 0;
    public float objSize = 1;
    public List<Transform> leftObjects;
    public List<Transform> rightObjects;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //angles go from 0 to 90
        //as the angles rotate, the lift and the center points of each object move up by the sum of the sin of the angles

        float inc = objSize * Mathf.Sin(angle*Mathf.Deg2Rad);
        float currentInc = inc / 2.0f;
        for(int i = 0; i < leftObjects.Count; i++)
		{
            leftObjects[i].localPosition = new Vector3(0, objSize *  currentInc, 0);
            rightObjects[i].localPosition = new Vector3(0, objSize *  currentInc, 0);

            currentInc += inc;
            leftObjects[i].localRotation = Quaternion.Euler(0, 0, angle);
            rightObjects[i].localRotation = Quaternion.Euler(0, 0, -angle);
        }
        
    }
}
