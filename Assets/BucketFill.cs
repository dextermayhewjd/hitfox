using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class BucketFill : MonoBehaviour
{
    public Slider progressBar;
    public float fillSpeed = 0.1f;
    private bool isFilling = false;
    private float fillAmount = 0f;

    void Update()
    {
        if (isFilling)
        {   
            if (fillAmount >= 1f)
            {
                Debug.Log("bucket is  filled");
                isFilling = false;
            }
            Debug.Log("bucket is being filled");
            fillAmount += fillSpeed * Time.deltaTime;
            progressBar.value = Mathf.Clamp(fillAmount, 0f, 1f);

            
        }
    }

    void OnTriggerEnter(Collider collision)
    {   

        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("bump into water");
            isFilling = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            isFilling = false;
        }
    }
}

