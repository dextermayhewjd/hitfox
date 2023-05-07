using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    Dictionary<KeyCode, bool> keys;
    Dictionary<string, bool> buttons;

    private bool movementKeysLocked;

    public GameObject maincam;
    public GameObject cam1;
    public GameObject cam2;
    public GameObject cam3;


    void Awake()
    {
        movementKeysLocked = false;

        keys = new Dictionary<KeyCode, bool>(); 
        buttons = new Dictionary<string, bool>();

        buttons["Jump"] = true;
        buttons["Escape"] = true;
        buttons["Sprint"] = true;

        SetKeyCodes();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetKeyCodes()
    {
        foreach (KeyCode keycode in System.Enum.GetValues(typeof(KeyCode)))
        {
            keys[keycode] = true;
        }
    }

    public void LockMovementKeys()
    {
        movementKeysLocked = true;
        buttons["Sprint"] = false;
        buttons["Jump"] = false;
    }

    public void UnlockMovementKeys()
    {
        movementKeysLocked = false;
        buttons["Sprint"] = true;
        buttons["Jump"] = true;
    }

    public void LockInteractKeys()
    {
        keys[KeyCode.Tab] = false;

        GameObject cameraSwitcherObj = GameObject.Find("CameraSwitcher");

        if (cameraSwitcherObj != null)
        {
            CameraSwitcher cameraSwitcher = cameraSwitcherObj.GetComponent<CameraSwitcher>();

            foreach (KeyCode keycode in cameraSwitcher.keybinds)
            {
                keys[keycode] = false;
            }
        }
    }

    public void UnlockInteractKeys()
    {
        keys[KeyCode.Tab] = true;

        GameObject cameraSwitcherObj = GameObject.Find("CameraSwitcher");

        if (cameraSwitcherObj != null)
        {
            CameraSwitcher cameraSwitcher = cameraSwitcherObj.GetComponent<CameraSwitcher>();

            foreach (KeyCode keycode in cameraSwitcher.keybinds)
            {
                keys[keycode] = true;
            }
        }
    }

    // Get Button Input.
    public bool GetInput(string buttonName)
    {
        return (buttons[buttonName]) ? Input.GetButton(buttonName) : false;
    }

    // Get Key Input.
    public bool GetInput(KeyCode keycode)
    {
        return (keys[keycode]) ? Input.GetKey(keycode) : false;
    }

    // Get Button Downn Input. 
    public bool GetInputDown(string buttonName)
    {
        return (buttons[buttonName]) ? Input.GetButtonDown(buttonName) : false;
    }

    // Get Key Down Input.
    public bool GetInputDown(KeyCode keycode)
    {
        return (keys[keycode]) ? Input.GetKeyDown(keycode) : false;
    }

    // Get Button Up Input.
    public bool GetInputUp(string buttonName)
    {
        return (buttons[buttonName]) ? Input.GetButtonUp(buttonName) : false;
    }

    // Get Key Up Input.
    public bool GetInputUp(KeyCode keycode)
    {
        return (keys[keycode]) ? Input.GetKeyUp(keycode) : false;
    }

    // Get Axis input.
    public float GetInputAxis(string axis) 
    {
        return (movementKeysLocked) ? 0 : Input.GetAxis(axis);
    }
}
