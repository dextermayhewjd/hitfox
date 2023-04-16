using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUp : OnTrigger
{
    public bool pickedUp = false;
    public PhotonView playerView = null;

    private Rigidbody rigidbody = null;
    private PhotonRigidbodyView rigidbodyView = null;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbodyView = GetComponent<PhotonRigidbodyView>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact")) 
        {
            if (pickedUp) 
            {
                if (playerView.IsMine)
                {
                    this.photonView.RPC("RPC_Drop", RpcTarget.AllBuffered);
                }
            } 
            else 
            {
                if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null)
                {
                    Debug.Log(colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                    // base.photonView.RequestOwnership();
                    this.photonView.RPC("RPC_PickUp", RpcTarget.AllBuffered, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                }
            }
        }
    }

    [PunRPC]
    void RPC_Drop()
    {
        Debug.Log("Object dropped");
        pickedUp = false;
        this.transform.SetParent(null);
        this.transform.position = playerView.transform.GetChild(0).gameObject.transform.position;
        this.transform.rotation = playerView.transform.GetChild(0).gameObject.transform.rotation;
        rigidbody.isKinematic = false;
        rigidbody.detectCollisions = true;
        rigidbodyView.enabled = true;
        playerView = null;
    }

    [PunRPC]
    void RPC_PickUp(int player)
    {
        Debug.Log("Picked up");
        pickedUp = true;
        playerView = PhotonView.Find(player);
        this.transform.SetParent(playerView.transform.GetChild(0).gameObject.transform);
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
        rigidbodyView.enabled = false;
    }
}
