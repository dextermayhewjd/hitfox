using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCamera : MonoBehaviour
{
    public Camera cam;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        cam.enabled = true;
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Exit");
        cam.enabled = false;
    }
}
