using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    // Todo
    // - Add functions for the different menu buttons.
    // - Animations?

    // Main UI Controller.
    private GameObject uiControllerObj;
    private UIController uiController;

    [SerializeField] private GameObject gameMenuParent;

    [SerializeField] private KeyCode gameMenuKey = KeyCode.Escape;

    // Start is called before the first frame update
    void Start()
    {
        uiControllerObj = transform.parent.gameObject;
        uiController = uiControllerObj.GetComponent<UIController>();

        Close(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(gameMenuKey))
        {
            Open();
        }
        else if (Input.GetKeyUp(gameMenuKey))
        {
            Close();
        }
    }

    private void Open()
    {
        if (gameMenuParent != null)
        {
            gameMenuParent.SetActive(true);
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

        uiController.LockCursor();
        uiController.UnlockCharacterControls();
    }
}
