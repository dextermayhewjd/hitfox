using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class BushScript : Interact
{
    public bool inUse = false;
    public PhotonView playerInBush = null;

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count != 0 && Input.GetButtonDown("Interact"))
        {
            if (inUse && playerInBush.IsMine)
            {
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = playerInBush.transform;
                cam.Follow = playerInBush.transform;
                this.photonView.RPC("RPC_UnhidePlayer", RpcTarget.All);
                
            }

            else if (!inUse) 
            {
                this.photonView.RPC("RPC_HidePlayer", RpcTarget.All, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                if (playerInBush.IsMine) 
                {
                    CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                    cam.LookAt = transform;
                    cam.Follow = transform;
                }
            }
        }
    }

    [PunRPC]
    void RPC_UnhidePlayer()
    {
        Debug.Log("Player left hiding spot");
        playerInBush.transform.SetParent(null);
        playerInBush.gameObject.SetActive(true);
        playerInBush = null;
        inUse = false;
    }

    [PunRPC]
    void RPC_HidePlayer(int player)
    {
        Debug.Log("Player hidden");
        inUse = true;
        playerInBush = PhotonView.Find(player);
        playerInBush.transform.SetParent(this.transform);
        playerInBush.gameObject.SetActive(false);
    }
}
