using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AnimateScissorLift : MonoBehaviour
{
    public ScissorJack scissorJack;
    private bool goingUp;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            scissorJack.angle = 15;
            yield return new WaitForSeconds(5);
            scissorJack.angle = 80;
            yield return new WaitForSeconds(5);
        } 
    }

    private void Update()
    {
        scissorJack.angle += Time.deltaTime;
    }
}
