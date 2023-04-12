using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CageScript : OnTrigger
{
    public int ownerId;

    [PunRPC]
    void RPC_Rescue(int playerID)
    {
        Debug.Log("Rescued");
        PhotonView.Find(playerID).GetComponent<PlayerMovement>().captured = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact")) 
        {
            Debug.Log("interacted");
            if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null &&
            !colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>().captured)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void OnDestroy() 
    {
        Debug.Log("destroy");
        base.photonView.RequestOwnership();
        this.photonView.RPC("RPC_Rescue", RpcTarget.AllBuffered, ownerId);
    }
}
