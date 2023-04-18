using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PickUp : OnTrigger
{
    public bool pickedUp = false;
    public PhotonView playerView = null;
    public float throwForce = 2500f; // the force added to the picked up things

    private Rigidbody rigidbody = null;
    private PhotonRigidbodyView rigidbodyView = null;
    // public Text pickupText = null; // reference to the text mesh component
    // private bool isPlayerColliding = false; // flag to track whether player is colliding with object


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
                if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null && colliders.Find(x => x.GetComponent<PhotonView>().IsMine).transform.Find("Mouth").childCount == 0)
                {
                    // base.photonView.RequestOwnership();
                    this.photonView.RPC("RPC_PickUp", RpcTarget.AllBuffered, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                }
            }
        }
        if (photonView.IsMine && pickedUp && Input.GetKeyDown(KeyCode.Q)) {
            Transform mouthTransform = playerView.gameObject.transform.Find("Mouth");
            if (Camera.main != null) {
            photonView.RPC("ThrowBucket", RpcTarget.AllViaServer, Camera.main.transform.forward);
        }
        }
    }
    //     void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         isPlayerColliding = true;
    //         pickupText.gameObject.SetActive(true); // show the text mesh when player enters the trigger zone
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         isPlayerColliding = false;
    //         pickupText.gameObject.SetActive(false); // hide the text mesh when player exits the trigger zone
    //     }
    // }
    



    [PunRPC]
    void RPC_Drop()
    {
        Debug.Log("Object dropped");
        pickedUp = false;
        this.transform.SetParent(null);
        this.transform.position = playerView.transform.Find("Mouth").position;
        this.transform.rotation = playerView.transform.Find("Mouth").rotation;
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
        this.transform.SetParent(playerView.transform.Find("Mouth"));
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
        rigidbodyView.enabled = false;
    }    
    
    [PunRPC]
    void ThrowBucket(Vector3 direction){
        pickedUp = false;
        rigidbody.transform.SetParent(null);
        rigidbody.isKinematic = false; // unfreeze the rigidbody
        rigidbody.detectCollisions = true;
        rigidbody.AddForce(direction * throwForce);
    }
}
