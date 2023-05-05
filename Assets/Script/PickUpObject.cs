using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PickUpObject : MonoBehaviourPun
{
    public bool pickedUp;
    public float throwForce; // the force added to the picked up things

    private Rigidbody rigidbody;
    private PhotonRigidbodyView rigidbodyView;
    public bool hasBeenDeleted = false;
    public GameObject hud;

    void Start()
    {
        pickedUp = false;
        throwForce = 500f;
        rigidbody = GetComponent<Rigidbody>();
        rigidbodyView = GetComponent<PhotonRigidbodyView>();
        hud = GameObject.Find("HUD");
    }

    public void Interact(PhotonView pv)
    {
        if (pv.IsMine)
        {
            if (!pickedUp)
            {
                // base.photonView.RequestOwnership();
                // hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
                // hud.transform.Find("ThrowButton").gameObject.SetActive(false); // hide button
                this.photonView.RPC("RPC_PickUpObject", RpcTarget.AllBuffered, pv.ViewID); // pick up object
            } else {
                
                this.photonView.RPC("RPC_DropObject", RpcTarget.AllBuffered, pv.ViewID); // drop object
            }
        }
    }

    public void Throw(PhotonView pv)
    {
        if (pv.IsMine)
        {
            // if we are holding something
            if(pickedUp)
            {
                // hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
                // hud.transform.Find("ThrowButton").gameObject.SetActive(false); // hide button
                this.photonView.RPC("RPC_DropObject", RpcTarget.AllBuffered, pv.ViewID); // drop object
                this.photonView.RPC("RPC_ThrowBucket", RpcTarget.AllBuffered, Camera.main.transform.forward); // throw object
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
    void RPC_ThrowBucket(Vector3 direction){
        rigidbody.AddForce(direction * throwForce);
    }
}
