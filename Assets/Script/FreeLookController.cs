using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookController : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    [SerializeField] private float maxSpeedY;
    [SerializeField] private float maxSpeedX;

    private bool rotationLocked;

    // Start is called before the first frame update
    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        freeLook.enabled = true;
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
