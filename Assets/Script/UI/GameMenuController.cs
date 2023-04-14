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

    [SerializeField] private GameObject gameMenuParent;

    [SerializeField] private KeyCode gameMenuKey = KeyCode.Escape;

    [SerializeField] private bool developerMode;

    [SerializeField] private GameObject developerMenuButton;
    [SerializeField] private GameObject settingsMenuButton;
    [SerializeField] private GameObject quitButton;

    [SerializeField] private GameObject developerMenuParent;
    [SerializeField] private GameObject settingsMenuParent;

    private bool menuOpen;

    // Start is called before the first frame update
    void Start()
    {
        uiControllerObj = transform.parent.gameObject;
        uiController = uiControllerObj.GetComponent<UIController>();

        menuOpen = false;

        if (gameMenuParent != null)
        {
            gameMenuParent.SetActive(false);
        }

        if (developerMenuParent != null)
        {
            developerMenuParent.SetActive(false);
        }

        if (settingsMenuParent != null)
        {
            settingsMenuParent.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(gameMenuKey))
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

        // Need to add an escape stack if we want to close UI elements in order.
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

        if (developerMenuParent != null)
        {
            developerMenuParent.SetActive(false);
        }

        if (settingsMenuParent != null)
        {
            settingsMenuParent.SetActive(false);
        }

        uiController.LockCursor();
        uiController.UnlockCharacterControls();
    }

    public void OpenDeveloperMenu()
    {
        if (developerMenuParent != null)
        {
            developerMenuParent.SetActive(true);
        }

    }

    public void OpenSettingsMenu()
    {
        if (settingsMenuParent != null)
        {
            settingsMenuParent.SetActive(true);
        }
    }

    public void ButtonQuit()
    {

    }
}
