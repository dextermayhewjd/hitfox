using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpObject : MonoBehaviourPun
{
    public bool pickedUp;
    public float throwForce; // the force added to the picked up things

    private Rigidbody rigidbody;
    private PhotonRigidbodyView rigidbodyView;

    void Start()
    {
        pickedUp = false;
        throwForce = 2500f;
        rigidbody = GetComponent<Rigidbody>();
        rigidbodyView = GetComponent<PhotonRigidbodyView>();
    }

    public void Interact(PhotonView pv)
    {
        if (pv.IsMine)
        {
            if (!pickedUp)
            {
                // base.photonView.RequestOwnership();
                this.photonView.RPC("RPC_PickUpObject", RpcTarget.AllBuffered, pv.ViewID);
            } else {
                this.photonView.RPC("RPC_DropObject", RpcTarget.AllBuffered, pv.ViewID);
            }
        }
    }

    [PunRPC]
    void RPC_DropObject(int playerID)
    {
        Debug.Log("Object dropped");
        pickedUp = false;
        PhotonView player = PhotonView.Find(playerID);
        this.transform.SetParent(null);
        this.transform.position = player.transform.Find("Mouth").position;
        this.transform.rotation = player.transform.Find("Mouth").rotation;
        rigidbody.isKinematic = false;
        rigidbody.detectCollisions = true;
        rigidbodyView.enabled = true;
    }
    [PunRPC]
    void RPC_PickUpObject(int playerID)
    {
        Debug.Log("Picked up");
        pickedUp = true;
        PhotonView player = PhotonView.Find(playerID);
        this.transform.SetParent(player.transform.Find("Mouth"));
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
        rigidbodyView.enabled = false;
    }    
    
    [PunRPC]
    void ThrowBucket1(Vector3 direction){
        pickedUp = false;
        rigidbody.transform.SetParent(null);
        rigidbody.isKinematic = false; // unfreeze the rigidbody
        rigidbody.detectCollisions = true;
        rigidbody.AddForce(direction * throwForce);
    }
}
