using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;
    // private float maxtime = 300f;//5 minutes
    private float maxtime = 3f;// for testing 
    public Text victoryText;
    public Text failedText;
    public int totalPoints = 0;

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
        
        if (totalPoints >= 500) {
        victoryText.gameObject.SetActive(true);
        return;
        }

        if (rem <= 0)
        {
            timerText.text = "Time's up!";
            failedText.gameObject.SetActive(true);
        return;
        }
        else
        {
            string minutes = ((int)rem / 60).ToString();
            string seconds = (rem % 60).ToString("f0");
            if(rem%60 < 9.5)
            {
                seconds = "0" + seconds;
            }

            timerText.text = minutes + ":" + seconds;
        }
        //Note: change later so when reaches 1 min left turns red/blue

    }
}
