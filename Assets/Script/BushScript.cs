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
                this.photonView.RPC("RPC_UnhidePlayer", RpcTarget.All, playerInBush.ViewID);
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = playerInBush.transform;
                cam.Follow = playerInBush.transform;
                playerInBush = null;
            }

            else if (!inUse) 
            {
                Debug.Log("If not in use");
                playerInBush = colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>();
                if (playerInBush.IsMine) {
                    CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                    cam.LookAt = transform;
                    cam.Follow = transform;
                }
                this.photonView.RPC("RPC_HidePlayer", RpcTarget.All, playerInBush.ViewID);
            }
        }
    }

    [PunRPC]
    void RPC_UnhidePlayer(int disablePlayer)
    {
        inUse = !inUse;
        PhotonView dp = PhotonView.Find(disablePlayer);
        dp.transform.SetParent(null);
        dp.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_HidePlayer(int disablePlayer)
    {
        inUse = !inUse;
        PhotonView dp = PhotonView.Find(disablePlayer);
        dp.transform.SetParent(this.transform);
        dp.gameObject.SetActive(false);
    }
}
