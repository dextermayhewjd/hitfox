using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void doExitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
