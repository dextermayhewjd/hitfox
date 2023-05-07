using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightPlayerInter : MonoBehaviour
{
    public Light spotlight;
    private bool once = false;
    public float playerHoldTime = 2f;  // How long to hold E to swap back to tree from treewithposter
    public float playerHoldTimer = 0f;
    private float gameTime = 0f;
    bool timeMid = false;
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        spotlight.enabled = false;
        photonView = GetComponent<PhotonView>();
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
        if (col.gameObject.tag == "Player" && spotlight.enabled == true && col.gameObject.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("Lampost");
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("Pressing E");
                playerHoldTimer += Time.deltaTime;
                if ((playerHoldTimer >= playerHoldTime))
                {
                    Debug.Log("Before: " + spotlight.enabled);
                    photonView.RPC("RPC_TurnLightOff", RpcTarget.AllBuffered);
                    Debug.Log("After: " + spotlight.enabled);
                    GameObject objectives = GameObject.Find("Timer+point");
                    Debug.Log("Lampost off get 5 points");
                    objectives.GetComponent<Timer>().IncreaseScore(5);
                }
            } else {
                playerHoldTimer = 0;
            }

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerHoldTimer = 0;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && !once) {
            gameTime += Time.deltaTime;
            if(gameTime >= 10f)//how many seconds till lamposts turn on(should turn on at halftime)
            {
                timeMid = true;
                once = true;
                photonView.RPC("RPC_TurnLightOn", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void RPC_TurnLightOn() {
        spotlight.enabled = true;
    }

    [PunRPC]
    void RPC_TurnLightOff() {
        spotlight.enabled = false;
    }
}
