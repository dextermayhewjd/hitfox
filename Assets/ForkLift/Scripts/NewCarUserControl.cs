using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

[RequireComponent(typeof (NewCarController))]
public class NewCarUserControl : OnTrigger
{
    private NewCarController m_Car; // the car controller we want to use
    public bool driving = false;
    public PhotonView acceleratePlayerView = null;
    public float h = 0f, v = 0f, handbrake = 1;
    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<NewCarController>();
    }


    private void FixedUpdate()
    {
        // pass the input to the car!
        if (driving) {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            handbrake = Input.GetAxis("Jump");
            
        } else {
            if (handbrake >= 1f) {
                handbrake = 1f;
            } else {
                handbrake += 0.01f;
            }
        }
        m_Car.Move(h, v, v, handbrake);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (driving)
            {
                if (acceleratePlayerView.IsMine)
                {
                    CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                    cam.LookAt = acceleratePlayerView.transform;
                    cam.Follow = acceleratePlayerView.transform;
                    this.photonView.RPC("RPC_ExitAccPlayer", RpcTarget.AllBuffered);
                    
                }
            }
            else
            {
                if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null) 
                {
                    // if (acceleratePlayerView.IsMine)
                    // {
                        CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                        cam.LookAt = transform;
                        cam.Follow = transform;
                    // }
                    base.photonView.RequestOwnership();
                    this.photonView.RPC("RPC_EnterAccPlayer", RpcTarget.AllBuffered, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                }
            }
        }   
    }

    [PunRPC]
    void RPC_ExitAccPlayer()
    {
        Debug.Log("Player left vehicle");
        acceleratePlayerView.transform.SetParent(null);
        acceleratePlayerView.GetComponent<CharacterController>().enabled = true;
        acceleratePlayerView.GetComponent<PlayerInteraction>().enabled = true;
        acceleratePlayerView.GetComponent<PlayerMovement>().enabled = true;
        // acceleratePlayerView.gameObject.SetActive(true);
        acceleratePlayerView = null;
        driving = false;
    }

    [PunRPC]
    void RPC_EnterAccPlayer(int player)
    {
        Debug.Log("Player entered vehicle");
        driving = true;
        acceleratePlayerView = PhotonView.Find(player);
        acceleratePlayerView.transform.SetParent(this.transform.Find("AccelerateSeat"));
        // acceleratePlayerView.gameObject.SetActive(false);
        acceleratePlayerView.transform.localPosition = Vector3.zero;
        acceleratePlayerView.transform.localRotation = Quaternion.identity;
        acceleratePlayerView.GetComponent<CharacterController>().enabled = false;
        acceleratePlayerView.GetComponent<PlayerInteraction>().enabled = false;
        acceleratePlayerView.GetComponent<PlayerMovement>().enabled = false;
    }
}

