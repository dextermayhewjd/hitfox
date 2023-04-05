using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FireInteraction : MonoBehaviour
{
    public Slider progressBar;
    public float health = 1f;
    public float recoverSpeed = 0.025f;    

        // for the fire it encounter 
    private BucketFill BucketFillInteraction;
    
    void Update(){
        if(health <= 0)
        {
            BucketFillInteraction.isPouring = false;
            Destroy(this.gameObject);
        }
        else
        {
            if(health >=1f)
            {
                health = 1f;
            }
            health += recoverSpeed*Time.deltaTime;
        }
        progressBar.value = Mathf.Clamp(health,0f,1f);        
    }

    void OnTriggerEnter(Collider collision)
    {   
        if (collision.gameObject.CompareTag("Bucket"))
        {    
            // Get the FireInteraction component of the current fire
            BucketFillInteraction = collision.gameObject.GetComponent<BucketFill>();
        }
    }
}
