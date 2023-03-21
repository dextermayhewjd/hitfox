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
    private bool isPouring = false;
    private float fillAmount = 0f;

    // for the fire it encounter 
    private FireInteraction currentFireInteraction;

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
        if (isPouring)
        {   
            if (fillAmount <= 0f)
            {
                Debug.Log("bucket is  empty");
                isPouring = false;
            }
            Debug.Log("bucket is pouring water");
            fillAmount -= fillSpeed * Time.deltaTime;
            progressBar.value = Mathf.Clamp(fillAmount, 0f, 1f);

            if(currentFireInteraction!=null)
            {
                currentFireInteraction.health -=0.2f * Time.deltaTime;
                currentFireInteraction.health = Mathf.Clamp(currentFireInteraction.health, 0f, 1f);
                currentFireInteraction.progressBar.value = Mathf.Clamp(currentFireInteraction.health, 0f, 1f);
            }
            
        }
    }

    void OnTriggerEnter(Collider collision)
    {   

        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("bump into water");
            isFilling = true;
        }
        if (collision.gameObject.CompareTag("Fire"))
        {
            Debug.Log("bump into fire");
            isPouring = true;
            
            // Get the FireInteraction component of the current fire
            currentFireInteraction = collision.gameObject.GetComponent<FireInteraction>();
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            isFilling = false;
        }
        if (collision.gameObject.CompareTag("Fire"))
        {
            isPouring = false;
            // Reset the current fire reference when the bucket exits the fire's collider
            currentFireInteraction = null;
        }
    }
}

