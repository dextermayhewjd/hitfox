using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;

    public void Open()
    {
        open = true;
        //activates main menu 
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        //disables main menu 
        gameObject.SetActive(false);
    }
}
