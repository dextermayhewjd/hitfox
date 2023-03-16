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
                this.photonView.RPC("RPC_Drop", RpcTarget.AllBuffered);
            }

            else if (!pickedUp) 
            {
                base.photonView.RequestOwnership();
                this.photonView.RPC("RPC_PickUp", RpcTarget.AllBuffered, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
            }
        }
    }

    [PunRPC]
    void RPC_Drop()
    {
        Debug.Log("Object dropped");
        this.transform.SetParent(null);
        playerView = null;
        pickedUp = false;
    }

    [PunRPC]
    void RPC_PickUp(int player)
    {
        Debug.Log("Picked up");
        pickedUp = true;
        playerView = PhotonView.Find(player);
        this.transform.SetParent(playerView.transform.GetChild(0).gameObject.transform); 
    }
}
