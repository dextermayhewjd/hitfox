using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class BucketFill : MonoBehaviour
{
    public Slider progressBar;
    public float fillSpeed = 0.2f;
    public bool isFilling = false;
    public bool isPouring = false;
    public float fillAmount = 0f;
    public Transform water;
    public float waterLevelHeightTop;
    public float waterLevelHeightBottom = 0.1f;
    public float waterLevelHeightCurrent;
    public float waterLevelRiseAmount;

    // for the fire it encounter 
    private FireInteraction currentFireInteraction;
    private NPC_Woodcutter currentNPC_Woodcutter;

    void Start()
    {
        water = this.transform.GetChild(1);
        water.gameObject.SetActive(false);
        waterLevelHeightTop = water.transform.localPosition.y;
        waterLevelRiseAmount = (waterLevelHeightTop - waterLevelHeightBottom) / (1f / fillSpeed);
        water.gameObject.transform.localPosition = new Vector3(0f, waterLevelHeightBottom, 0f);
        waterLevelHeightCurrent = waterLevelHeightBottom;
    }

    void Update()
    {
        progressBar.value = Mathf.Clamp(fillAmount, 0f, 1f);  
        if (isFilling)
        {   
            water.gameObject.SetActive(true);

            if (fillAmount >= 1)
            {
                Debug.Log("bucket is  filled");
                fillAmount = 1;
                isFilling = false;
                // water.transform.localPosition = new Vector3(0f, waterLevelHeightTop, 0f);
            }
            else
            {
                Debug.Log("bucket is being filled");
                fillAmount += fillSpeed * Time.deltaTime;
                waterLevelHeightCurrent += waterLevelRiseAmount * Time.deltaTime;
                progressBar.value = Mathf.Clamp(fillAmount, 0f, 1f);
                water.transform.localPosition = new Vector3(0f, Mathf.Clamp(waterLevelHeightCurrent, waterLevelHeightBottom, waterLevelHeightCurrent), 0f);

            }
        }

        if (isPouring)
        {   
            if (fillAmount <= 0f)
            {
                Debug.Log("bucket is  empty");
                isPouring = false;
                fillAmount = 0f;
                water.gameObject.SetActive(false);
                // water.transform.localPosition = new Vector3(0f, waterLevelHeightBottom, 0f);
            }
            else
            {
                Debug.Log("bucket is pouring water");
                fillAmount -= fillSpeed * Time.deltaTime;
                waterLevelHeightCurrent -= waterLevelRiseAmount * Time.deltaTime;
                progressBar.value = Mathf.Clamp(fillAmount, 0f, 1f);
                water.transform.localPosition = new Vector3(0f, Mathf.Clamp(waterLevelHeightCurrent, waterLevelHeightBottom, waterLevelHeightCurrent), 0f);
            }

            if (currentFireInteraction != null)
            {
                currentFireInteraction.health -= 0.2f * Time.deltaTime;
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
        if (collision.gameObject.CompareTag("Lumberjack"))
        {
            currentNPC_Woodcutter = collision.gameObject.GetComponent<NPC_Woodcutter>();  
            float timeForLumberjackToStop = fillAmount * 4;
            Debug.Log("Lumberjack need to stop for" + timeForLumberjackToStop +"secs");
            // Get the FireInteraction component of the current fire
      
            currentNPC_Woodcutter.pauseTime = timeForLumberjackToStop;
            fillAmount = 0f;  
            Debug.Log("WaterEmpty");

        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            isFilling = false;
            Debug.Log("Left water");
        }
        if (collision.gameObject.CompareTag("Fire"))
        {
            isPouring = false;
            Debug.Log("Left fire");
            // Reset the current fire reference when the bucket exits the fire's collider
            currentFireInteraction = null;
        }
    }
}

