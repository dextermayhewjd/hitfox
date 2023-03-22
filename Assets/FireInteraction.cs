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

    void Update(){
        if(health <= 0)
        {
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
}
