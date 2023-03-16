using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

[RequireComponent(typeof (NewCarController))]
public class NewCarUserControl : Interact
{
    private NewCarController m_Car; // the car controller we want to use
    bool driving = false;
    public PhotonView acceleratePlayerView = null;
    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<NewCarController>();
    }


    private void FixedUpdate()
    {
        float h = 0, v = 0;

        // pass the input to the car!
        if (driving) {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        #if !MOBILE_INPUT
        float handbrake = Input.GetAxis("Jump");
        m_Car.Move(h, v, v, handbrake);
        
        #else
        m_Car.Move(h, v, v, 0f);
        #endif
    }

    private void Update()
    {
        if (colliders.Count != 0 && Input.GetButtonDown("Interact"))
        {
            if (driving && acceleratePlayerView.IsMine)
            {
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = acceleratePlayerView.transform;
                cam.Follow = acceleratePlayerView.transform;
                this.photonView.RPC("RPC_ExitAccPlayer", RpcTarget.All);
                
            }

            else if (!driving) 
            {
                base.photonView.RequestOwnership();
                this.photonView.RPC("RPC_EnterAccPlayer", RpcTarget.All, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                if (acceleratePlayerView.IsMine) 
                {
                    CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                    cam.LookAt = transform;
                    cam.Follow = transform;
                }
            }
        }
    }

    [PunRPC]
    void RPC_ExitAccPlayer()
    {
        Debug.Log("Player left vehicle");
        acceleratePlayerView.transform.SetParent(null);
        acceleratePlayerView.gameObject.SetActive(true);
        acceleratePlayerView = null;
        driving = false;
    }

    [PunRPC]
    void RPC_EnterAccPlayer(int player)
    {
        Debug.Log("Player entered vehicle");
        driving = true;
        acceleratePlayerView = PhotonView.Find(player);
        acceleratePlayerView.transform.SetParent(this.transform.GetChild(0).gameObject.transform);
        acceleratePlayerView.gameObject.SetActive(false);
    }
}

