using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls cursor behaviours.
public class CursorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Unlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
