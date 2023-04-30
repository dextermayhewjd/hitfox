using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPlayerInter : MonoBehaviour
{
    public Light spotlight;
    public float playerHoldTime = 2f;  // How long to hold E to swap back to tree from treewithposter
    private float playerHoldTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        spotlight.enabled = true;
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player collided with Lampost!");

        }
    }
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player stayed");
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("Pressing E");
                playerHoldTimer += Time.deltaTime;
                if (playerHoldTimer >= playerHoldTime)
                {
                    spotlight.enabled = false;
                }
            }

        }
    }
    void Update()
    {
        
    }
}
