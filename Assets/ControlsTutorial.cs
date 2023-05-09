using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTutorial : MonoBehaviour
{
    public UIController uiController;

    // Start is called before the first frame update
    void Update() {
        uiController.UnlockCursor();
        uiController.LockCharacterControls();
    }

    public void CloseTutorial() {
        uiController.LockCursor();
        uiController.UnlockCharacterControls();
        this.gameObject.SetActive(false);
    }
}
