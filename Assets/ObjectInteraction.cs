using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectInteraction : OnTriggerPickUp
{
    public GameObject objectInMouth;

    void Start()
    {
        objectInMouth = null;
    }

    // private void OnTriggerStay(Collider other) {
    //     if (other.gameObject.GetComponent<PickUpObject>() != null)
    //     {
    //         if (Input.GetButtonDown("Interact"))
    //             if (objectInMouth == null)
    //             {
    //                 Debug.Log("Can pick up");
    //                 // if (Input.GetButtonDown("Interact"))
    //                 // {
    //                     objectInMouth = other.gameObject;
    //                     other.gameObject.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
    //                 // }
    //             } else {
    //                 // if (Input.GetButtonDown("Interact"))
    //                 // {
    //                     Debug.Log("Drop");
    //                     objectInMouth.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
    //                     objectInMouth = null;
    //                 // }
    //             }
    //             {
    //             }
    //         // else 
    //         // {
    //         //     Debug.Log("Switch");
    //         //     if (Input.GetButtonDown("Interact"))
    //         //     {
    //         //         objectInMouth.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
    //         //         other.gameObject.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
    //         //         objectInMouth = null;
    //         //     }
    //         // }
    //     }
    // }

    private void Update() {
        if (Input.GetButtonDown("Interact"))
        {
            if (objectInMouth != null) 
            {
                Debug.Log("Dropped");
                objectInMouth.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
                objectInMouth = null;
                
            } else if (objectsToPickUp.Count != 0) {
                Debug.Log("Picked up");
                objectInMouth = objectsToPickUp[0].gameObject;
                objectInMouth.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
            }
        }
    }
}
