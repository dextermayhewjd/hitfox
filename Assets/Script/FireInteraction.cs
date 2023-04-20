using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FireInteraction : MonoBehaviourPun
{
    public Slider progressBar;
    public float health = 1f;
    public float recoverSpeed = 0.025f;    
    // public GameObject QuestSystem = GameObject.Find("QuestManager");
    // for the fire it encounter 
    private BucketFill BucketFillInteraction;
    
    private QuestManager questManager;
    
    private void Start()
    {
        // Get the QuestManager instance
        questManager = FindObjectOfType<QuestManager>();
    }

    void Update(){
        // if the fire is put out
        if(health <= 0)
        {
            BucketFillInteraction.isPouring = false;
            PhotonNetwork.Destroy(this.gameObject);
            if (PhotonNetwork.IsMasterClient) {
                GameObject objectives = GameObject.Find("Timer+point");
                Debug.Log("20 points for putting out fire");
                objectives.GetComponent<Timer>().IncreaseScore(20);
                // questManager.CompleteQuest(0);
                // QuestSystem.GetComponent<Quest>().missionComplete("Fire");
            }
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
        // this.photonView.RPC("RPC_UpdateBucket", RpcTarget.AllBuffered);
    }

    void OnTriggerEnter(Collider collision)
    {   
        if (collision.gameObject.CompareTag("Bucket"))
        {    
            // Get the FireInteraction component of the current fire
            BucketFillInteraction = collision.gameObject.GetComponent<BucketFill>();
        }
    }

    void OnTriggerExit(Collider collision)
    {   
        if (collision.gameObject.CompareTag("Bucket"))
        {    
            BucketFillInteraction = null;
        }
    }

    [PunRPC]
    void RPC_UpdateBucket()
    {
        
    }
}
