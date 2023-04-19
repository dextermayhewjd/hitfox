using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemStats : MonoBehaviour
{
    // Main UI Controller.
    private GameObject uiControllerObj;
    private UIController uiController;

    // [0] FPS Counter
    // [1] Ping
    private TMP_Text[] textFields;

    private float pollingTime = 1f;
    private float time;
    private int frameCount;

    // Start is called before the first frame update
    void Start()
    {
        uiControllerObj = GameObject.Find("UIController");
        uiController = uiControllerObj.GetComponent<UIController>();

        textFields = GetComponentsInChildren<TMP_Text>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (uiController.developerModeEnabled)
        {
            time += Time.deltaTime;

            frameCount++;

            if (time >= pollingTime)
            {
                int frameRate = Mathf.RoundToInt(frameCount / time);
                textFields[0].text = frameRate.ToString();

                time -= pollingTime;
                frameCount = 0;
            }
        }
        else
        {
            textFields[0].text = "";
        }
    }
}
