using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public GameObject timerAndPoints;
    // Update is called once per frame
    void Update()
    {
        scoreText.text = timerAndPoints.GetComponent<Timer>().currentPoints.ToString();
    }
}
