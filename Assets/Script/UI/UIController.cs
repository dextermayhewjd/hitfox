using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

// Main UI controller to easily communicate with other UI components.
public class UIController : MonoBehaviour
{
    private Canvas canvas;
    private CanvasScaler canvasScaler;
    [SerializeField] private float planeDistance;

    // The communications wheel and ping system.
    private GameObject commsWheelControllerObj;
    private CommunicationsWheelController commsWheelController;

    // The in-game menu.
    private GameObject gameMenuControllerObj;
    private GameMenuController gameMenuController;

    // Cinemachine Free Look. Camera controls and looking around.
    private CinemachineFreeLook freeLook;
    private FreeLookController freeLookController;

    // Input controller.
    private InputController inputController;

    // Controls the cursor behaviour.
    private CursorController cursorController;

    private bool characterControlsLocked;
    public bool developerModeEnabled;

    // Start is called before the first frame update
    void Start()
    {
        canvas = transform.parent.gameObject.GetComponent<Canvas>();
        canvasScaler = transform.parent.gameObject.GetComponent<CanvasScaler>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = planeDistance;
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        
        inputController = GameObject.Find("InputController").GetComponent<InputController>();

        freeLook = FindObjectOfType<CinemachineFreeLook>();
        freeLookController = freeLook.GetComponent<FreeLookController>();

        cursorController = GetComponent<CursorController>();

        characterControlsLocked = false;        
        developerModeEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // void OnApplicationFocus(bool focusStatus)
    // {
    //     if (focusStatus) {
    //         Cursor.lockState = CursorLockMode.Locked;
    //     } else {
    //         Cursor.lockState = CursorLockMode.None;
    //     }
    // }

    public void LockCharacterControls()
    {
        characterControlsLocked = true;
        inputController.LockMovementKeys();
        inputController.LockInteractKeys();
        LockFreeLook();
    }

    public void UnlockCharacterControls()
    {
        characterControlsLocked = false;
        inputController.UnlockMovementKeys();
        inputController.UnlockInteractKeys();
        UnlockFreeLook();
    }

    public void LockFreeLook()
    {
        freeLookController.Lock();
    }

    public void UnlockFreeLook()
    {
        freeLookController.Unlock();
    }

    public void LockCursor()
    {
        cursorController.Lock();
    }

    public void UnlockCursor()
    {
        cursorController.Unlock();
    }

    public bool CharacterControlsLocked()
    {
        return characterControlsLocked;
    }

    public void EnableDeveloperMode()
    {
        developerModeEnabled = true;
    }

    public void DisableDeveloperMode()
    {
        developerModeEnabled = false;
    }
}
