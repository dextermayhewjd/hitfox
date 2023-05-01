using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPlayerInter : MonoBehaviour
{
    public Light spotlight;
    private bool once = false;
    public float playerHoldTime = 2f;  // How long to hold E to swap back to tree from treewithposter
    private float playerHoldTimer = 0f;
    private float gameTime = 0f;
    bool timeMid = false;

    // Start is called before the first frame update
    void Start()
    {
        spotlight.enabled = false;
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player collided with Lampost!");

        }
    }
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Lampost");
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("Pressing E");
                playerHoldTimer += Time.deltaTime;
                if ((playerHoldTimer >= playerHoldTime) && spotlight.enabled)
                {
                    Debug.Log("Before: " + spotlight.enabled);
                    spotlight.enabled = false;
                    Debug.Log("After: " + spotlight.enabled);
                    GameObject objectives = GameObject.Find("Timer+point");
                    Debug.Log("Lampost off get 5 points");
                    objectives.GetComponent<Timer>().IncreaseScore(5);
                }
            }

        }
    }
    void Update()
    {
        gameTime += Time.deltaTime;
        if(gameTime >= 10f && !once)//how many seconds till lamposts turn on(should turn on at halftime)
        {
            timeMid = true;
            spotlight.enabled = true;
            once = true;
        }
    }
}
