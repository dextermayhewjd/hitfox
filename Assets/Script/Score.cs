using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Timer timerAndPoints;
    // Update is called once per frame
    void Update()
    {
        scoreText.text = timerAndPoints.currentPoints.ToString();
        scoreText.text += "/" + timerAndPoints.requiredPoints.ToString();
    }
}
