using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    // Todo
    // - Improve code logic.
    // - Add functions for the different menu buttons.
    // - Animations?

    // Main UI Controller.
    private GameObject uiControllerObj;
    private UIController uiController;

    private InputController inputController;

    [SerializeField] private GameObject gameMenuParent;

    [SerializeField] private bool developerMode;

    [SerializeField] private GameObject developerMenuButton;

    private GameObject developerMenuParent;
    private GameObject settingsMenuParent;

    private bool menuOpen;
    private bool developerModeActive;

    // Start is called before the first frame update
    void Start()
    {
        uiControllerObj = GameObject.Find("UIController");
        uiController = uiControllerObj.GetComponent<UIController>();

        inputController = GameObject.Find("InputController").GetComponent<InputController>();

        developerMenuParent = GameObject.Find("DeveloperModeMenu");
        settingsMenuParent = GameObject.Find("SettingsMenu");

        menuOpen = false;
        developerModeActive = false;

        if (gameMenuParent != null)
        {
            gameMenuParent.SetActive(false);
        }

        CloseSettingsMenu();
        CloseDeveloperModeMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputController.GetInputDown(KeyCode.Escape))
        {
            menuOpen = !menuOpen;
            if (menuOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }

    private void Open()
    {
        if (gameMenuParent != null)
        {
            gameMenuParent.SetActive(true);

            if (developerMenuButton != null)
            {
                if (developerMode)
                {
                    developerMenuButton.SetActive(true);
                }
                else
                {
                    developerMenuButton.SetActive(false);
                }
            }

            if (developerModeActive)
            {
                OpenDeveloperModeMenu();
            }
        }

        uiController.LockCharacterControls();
        uiController.UnlockCursor();
    }

    private void Close()
    {
        if (gameMenuParent != null)
        {
            gameMenuParent.SetActive(false);
        }

        if (developerModeActive)
        {
            CloseDeveloperModeMenu();
        }

        CloseSettingsMenu();

        uiController.LockCursor();
        uiController.UnlockCharacterControls();
    }

    public void ToggleDeveloperMode()
    {
        if (developerModeActive)
        {
            CloseDeveloperModeMenu();
            developerModeActive = false;
        }
        else
        {
            OpenDeveloperModeMenu();
            developerModeActive = true;
        }
    }

    private void OpenDeveloperModeMenu()
    {
        if (developerMenuParent != null)
        {
            developerMenuParent.SetActive(true);
        }
    }

    private void CloseDeveloperModeMenu()
    {
        if (developerMenuParent != null)
        {
            developerMenuParent.SetActive(false);
        }
    }

    public void OpenSettingsMenu()
    {
        if (settingsMenuParent != null)
        {
            settingsMenuParent.SetActive(true);
        }
    }

    private void CloseSettingsMenu()
    {
        if (settingsMenuParent != null)
        {
            settingsMenuParent.SetActive(false);
        }
    }

    public void QuitToHomeMenu()
    {
        Debug.Log("Quit");
    }
}
