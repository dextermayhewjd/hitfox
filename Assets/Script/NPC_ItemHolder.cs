using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_ItemHolder : MonoBehaviour, IInteractable
{
    public enum State {IDLE, CHASE}
    State state;

    //Transform that NPC has to follow
    public Transform transformToFollow;
    //NavMesh Agent variable
    UnityEngine.AI.NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        Inventory inventory = new Inventory();
        state = State.IDLE;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log("itemholder NPC started");
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.CHASE) {
            agent.destination = transformToFollow.position;
            Debug.Log("itemholder NPC chasing");
        }
        // Debug.Log(state);
    }

    public void Interact() {
        Debug.Log("Interacted");
        state = State.CHASE;
        Debug.Log("Interacted; chasing!");
    }
}
