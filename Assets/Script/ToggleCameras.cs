using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCameras : MonoBehaviour
{
    public Camera cam1;
    public Camera cam2;
    
    public void switchcam(int x)
    {
        deactivateall();
        if(x == 1)
        {
            cam1.enabled = true;
        }
        else if(x == 2)
        {
            cam2.enabled = true;
        }
    }

    public void deactivateall()
    {
        cam1.enabled = false;
        cam2.enabled = true;
    }
}
