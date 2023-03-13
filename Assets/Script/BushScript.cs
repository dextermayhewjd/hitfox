using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class BushScript : Interact
{
    public bool inUse = false;
    public Collider playerInBush = null;

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count != 0 && Input.GetButtonDown("Interact"))
        {
            Debug.Log("Player interacted");

            if (inUse && playerInBush.GetComponent<PhotonView>().IsMine)
            {
                this.photonView.RPC("RPC_UnhidePlayer", RpcTarget.All);
                // if (playerInBush.GetComponent<PhotonView>.IsMine) 
                // {
                    
                // }
            }

            else if (!inUse) 
            {
                Debug.Log("If not in use");
                this.photonView.RPC("RPC_HidePlayer", RpcTarget.All);
                
                // if (playerInBush.GetComponent<PhotonView>.IsMine) 
                // {
                    
                // }
            }
        }
    }

    [PunRPC]
    void RPC_UnhidePlayer()
    {
        inUse = !inUse;
        playerInBush.transform.SetParent(null);
        playerInBush.gameObject.SetActive(true);
        CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
        cam.LookAt = playerInBush.transform;
        cam.Follow = playerInBush.transform;
        playerInBush = null;
    }

    [PunRPC]
    void RPC_HidePlayer()
    {
        inUse = !inUse;
        playerInBush = colliders.Find(x => x.GetComponent<PhotonView>().IsMine);
        playerInBush.transform.SetParent(this.transform);
        playerInBush.gameObject.SetActive(false);
        CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
        cam.LookAt = transform;
        cam.Follow = transform;
    }
}
