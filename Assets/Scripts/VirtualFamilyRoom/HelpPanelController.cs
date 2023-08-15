using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPanelController : MonoBehaviour
{

    private float goalRot = -45;
    [SerializeField]
    private MeshRenderer backPanel;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.BackQuote))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                raisePanel();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                lowerPanel();
            }
        }

        float currentAngle = transform.localEulerAngles.x;
        if (currentAngle > 0)
        {
            currentAngle -= 360f;
        }
        currentAngle -= 45;

        if (currentAngle < -50f)
            backPanel.material.color = new Color(.1f, .1f, .1f, -(currentAngle + 45) / 90 + .5f);

        if (currentAngle - goalRot > 1)
        {
            transform.Rotate(-100 * Time.deltaTime, 0, 0);
        }
        else if (currentAngle - goalRot < -1)
        {
            transform.Rotate(100 * Time.deltaTime, 0, 0);
        }
    }

    public void raisePanel()
    {
        goalRot = -135;
    }

    public void lowerPanel()
    {
        goalRot = -45;
    }
}
