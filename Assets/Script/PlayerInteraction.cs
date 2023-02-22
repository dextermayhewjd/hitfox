using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
 
public class PlayerInteraction : MonoBehaviour, IInteractable {
 
    public Transform player;
    public float interactionDistance;
    bool active = false;
 
    private void Update() 
    {
        InteractionRay();
    }

    public void Interact() 
    {    
        PlayerMovement PM = GetComponent<PlayerMovement>();
        Debug.Log(PM.gameObject.name);
        PM.view.RPC("RPC_Revive", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Revive()
    {
        PlayerMovement PM = GetComponent<PlayerMovement>();
        if (!PM.view.IsMine) {
            Debug.Log(PM.view);
            Debug.Log("Returned");
            return;
        }
        Debug.Log(PM.captured);
        if (PM.captured)
        {
            Debug.Log("Revived");
            PM.captured = false;
        }
    }
 
    void InteractionRay() {
        RaycastHit hit;
        active = Physics.Raycast(player.position, player.TransformDirection(Vector3.forward), out hit, interactionDistance);
 
        if (active) {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
 
            if (interactable != null) { 
                if (Input.GetButton("Interact")) {
                    interactable.Interact();
                }
            }
        }
    }
}