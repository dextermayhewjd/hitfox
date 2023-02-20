using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class PlayerInteraction : MonoBehaviour {
 
    public Transform player;
    public float interactionDistance;
    bool active = false;

    private SC_CharacterController characterController;
 
    private void Start() {
        characterController = GetComponent<SC_CharacterController>();
    }

    private void Update() {
        InteractionRay();
    }
 
    void InteractionRay() {
        RaycastHit hit;
        active = Physics.Raycast(player.position, player.TransformDirection(Vector3.forward), out hit, interactionDistance);
 
        if (active) {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            IStealable stealable = hit.collider.GetComponent<IStealable>();
 
            if (interactable != null) { 
                if (Input.GetKeyDown(KeyCode.E)) {
                    interactable.Interact();
                    Debug.Log("Interacting with:");
                    Debug.Log(interactable);
                }
            }
            if (stealable != null) {
                if(Input.GetKeyDown(KeyCode.R)) {
                    foreach(Item item in stealable.CheckItems()) {
                        stealable.StealFrom(characterController, item);
                    }
                }
            }
        }
    }
}