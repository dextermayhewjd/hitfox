using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menus;
    
    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName) // using string as a key to access the menu
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                OpenMenu(menus[i]);


            }
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                CloseMenu(menus[i]);

            }
        }

        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
