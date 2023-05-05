using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdatePointsSlider : MonoBehaviour
{
    public Timer timerAndPoints;
    public Slider slider;
    

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = timerAndPoints.requiredPoints;
    }

    void Update()
    {
        Debug.Log("lol");
        slider.maxValue = timerAndPoints.requiredPoints;
        slider.value = timerAndPoints.currentPoints;
    }
}
