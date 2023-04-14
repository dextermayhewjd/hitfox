using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{
    private Camera playerCam;

    // Use this instead.
    // [SerializeField] private float sensitivity;

    private bool cameraRotationLocked;

    // Start is called before the first frame update
    void Start()
    {
        Camera playerCam = GetComponent<Camera>();
        cameraRotationLocked = false;
        LookAtCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (!cameraRotationLocked)
        {
            LookAtCursor();
        }
    }

    private void LookAtCursor()
    {
        float sensitivity = 0.05f;
        Vector3 vp = playerCam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerCam.nearClipPlane));
        vp.x -= 0.5f;
        vp.y -= 0.5f;
        vp.x *= sensitivity;
        vp.y *= sensitivity;
        vp.x += 0.5f;
        vp.y += 0.5f;
        Vector3 sp = playerCam.ViewportToScreenPoint(vp);
        
        Vector3 v = playerCam.ScreenToWorldPoint(sp);
        transform.LookAt(v, Vector3.up);
    }

    public void lockCameraRotation()
    {

    }
}
