using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Main UI controller to easily communicate with other UI components.
public class UIController : MonoBehaviour
{
    // The communications wheel and ping system.
    private GameObject commsWheelControllerObj;
    private CommunicationsWheelController commsWheelController;

    // The in-game menu.
    private GameObject gameMenuControllerObj;
    private GameMenuController gameMenuController;

    // Cinemachine Free Look. Camera controls and looking around.
    private CinemachineFreeLook freeLook;
    private FreeLookController freeLookController;

    // Controls the cursor behaviour.
    private CursorController cursorController;

    private bool characterControlsLocked;

    // Start is called before the first frame update
    void Start()
    {
        gameMenuControllerObj = transform.GetChild(0).gameObject;
        commsWheelControllerObj = transform.GetChild(1).gameObject;

        gameMenuController = gameMenuControllerObj.GetComponent<GameMenuController>();
        commsWheelController = commsWheelControllerObj.GetComponent<CommunicationsWheelController>();

        freeLook = FindObjectOfType<CinemachineFreeLook>();
        freeLookController = freeLook.GetComponent<FreeLookController>();

        cursorController = GetComponent<CursorController>();

        characterControlsLocked = false;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LockCharacterControls()
    {
        characterControlsLocked = true;
        LockFreeLook();
    }

    public void UnlockCharacterControls()
    {
        characterControlsLocked = false;
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
}
