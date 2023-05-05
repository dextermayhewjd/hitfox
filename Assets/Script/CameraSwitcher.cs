using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    private InputController inputController;

    [SerializeField] private GameObject[] cameras;
    [SerializeField] public KeyCode[] keybinds;

    // Start is called before the first frame update
    void Start()
    {
        inputController = GameObject.Find("InputController").GetComponent<InputController>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < keybinds.Length; i++)
        {
            if (inputController.GetInputDown(keybinds[i]))
            {
                SetCamera(i);
            }
        }
    }

    public void SetCamera(int camIndex)
    {
        if (camIndex >= cameras.Length)
        {
            Debug.Log("Not Enough Cameras Set");
            return;
        }

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
            {
                if (camIndex == i)
                {
                    cameras[i].SetActive(true);
                }
                else
                {
                    cameras[i].SetActive(false);
                }
            }
        }
    }
}
