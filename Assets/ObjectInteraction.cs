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

    private void Update() {
        if (objectsToPickUp.Count != 0)
        {
            if (objectsToPickUp[0].GetComponent<PickUpObject>() == null)
            {
                objectsToPickUp.RemoveAt(0);
            }
        }

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
