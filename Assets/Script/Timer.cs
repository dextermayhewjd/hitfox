using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;
    private float maxtime = 300f;//5 minutes
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //countdown from 5 minutes
        float t = Time.time - startTime;//seconds since started
        float rem = maxtime - t;
        if (rem <= 0)
        {
            timerText.text = "Time's up!";
        }
        else
        {
            string minutes = ((int)rem / 60).ToString();
            string seconds = (rem % 60).ToString("f0");

            timerText.text = minutes + ":" + seconds;
        }
        //Note: change later so when reaches 1 min left turns red/blue

    }
}
