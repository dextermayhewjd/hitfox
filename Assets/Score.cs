using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;
    public GameObject objectivesTracker;
    // Update is called once per frame
    void Update()
    {
        scoreText.text = objectivesTracker.GetComponent<ObjectivesScript>().currentPoints.ToString();
    }
}
