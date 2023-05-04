using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ObjectInteraction : OnTriggerPickUp
{
    public GameObject objectInMouth;

    void Start()
    {
        objectInMouth = null;
        hud = GameObject.Find("HUD");
    }

    private void Update() {
        int numRemoved = objectsToPickUp.RemoveAll(item => item == null);
        if (numRemoved > 0) {
            hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
        }
        if (objectInMouth != null) {
            hud.transform.Find("InteractButton").gameObject.SetActive(true); // show button
            hud.transform.Find("InteractButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Drop"; // change text of action
            hud.transform.Find("ThrowButton").gameObject.SetActive(true); // show button
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (objectInMouth != null) 
            {
                hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
                hud.transform.Find("ThrowButton").gameObject.SetActive(false); // hide button
                objectInMouth.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
                objectInMouth = null;
                
            } else if (objectsToPickUp.Count != 0) {
                Debug.Log("Picked up");
                objectInMouth = objectsToPickUp[0].gameObject;
                objectInMouth.GetComponent<PickUpObject>().Interact(this.GetComponent<PhotonView>());
            }
        }
        if (objectInMouth != null && Input.GetKeyDown(KeyCode.Q)) {
               
               if (Camera.main != null) 
               {
                hud.transform.Find("ThrowButton").gameObject.SetActive(false); // hide button
                objectInMouth.GetComponent<PickUpObject>().Throw(this.GetComponent<PhotonView>());
                objectInMouth = null;
               }
        }
    }
}
