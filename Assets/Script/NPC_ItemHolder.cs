using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_ItemHolder : MonoBehaviour, IInteractable
{
    public enum State {IDLE, CHASE}

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory();
        State state = State.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Interact() {
        
    }
}
