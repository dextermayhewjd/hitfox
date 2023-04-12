using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookController : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    private float maxSpeedY;
    private float maxSpeedX;

    private bool rotationLocked;

    // Start is called before the first frame update
    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        freeLook.enabled = true;
        maxSpeedY = freeLook.m_YAxis.m_MaxSpeed;
        maxSpeedX = freeLook.m_XAxis.m_MaxSpeed;
        Unlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Lock()
    {
        freeLook.m_YAxis.m_MaxSpeed = 0;
        freeLook.m_XAxis.m_MaxSpeed = 0;
        rotationLocked = true;

    }

    public void Unlock()
    {
        rotationLocked = false;
        freeLook.m_YAxis.m_MaxSpeed = maxSpeedY;
        freeLook.m_XAxis.m_MaxSpeed = maxSpeedX;
    }
}
