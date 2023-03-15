using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUp : Interact
{
    public bool pickedUp = false;
    public PhotonView playerView = null;
    void Update()
    {
        if (colliders.Count != 0 && Input.GetButtonDown("PickUp"))
        {
            if (pickedUp && playerView.IsMine)
            {
                // this.photonView.RPC("Drop", RpcTarget.All, playerView.ViewID);
                Debug.Log("Object dropped");
        pickedUp = !pickedUp;
        PhotonView p = PhotonView.Find(playerView.ViewID);
        this.transform.SetParent(null);
                playerView = null;
            }

            else if (!pickedUp) 
            {
                playerView = colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>();
                // this.photonView.RPC("RPC_PickUp", RpcTarget.All, playerView.ViewID);
                Debug.Log("Picked up");
        pickedUp = !pickedUp;
        PhotonView p = PhotonView.Find(playerView.ViewID);
        this.transform.SetParent(p.transform);
            }
        }
    }

    [PunRPC]
    void RPC_Drop(int player)
    {
        
    }

    [PunRPC]
    void RPC_PickUp(int player)
    {
        
    }
}
