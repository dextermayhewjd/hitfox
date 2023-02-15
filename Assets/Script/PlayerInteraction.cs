using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class PlayerInteraction : MonoBehaviour {
 
    public Transform player;
    public float interactionDistance;
    bool active = false;
 
    private void Update() {
        InteractionRay();
    }
 
    void InteractionRay() {
        RaycastHit hit;
        active = Physics.Raycast(player.position, player.TransformDirection(Vector3.forward), out hit, interactionDistance);
 
        if (active) {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
 
            if (interactable != null) { 
                if (Input.GetKeyDown(KeyCode.E)) {
                    interactable.Interact();
                    Debug.Log("Interacting with:");
                    Debug.Log(interactable);
                }
            }
        }
    }
}