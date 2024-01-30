using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFireSlider : MonoBehaviour
{
    public FireInteraction fireInteraction;
    public Slider slider;
    

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
    }

    void Update()
    {
        slider.value = fireInteraction.health;
    }
}
