using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBucketSlider : MonoBehaviour
{
    public BucketFill bucketFill;
    public Slider slider;
    

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
    }

    void Update()
    {
        slider.value = bucketFill.fillAmount;
    }
}
