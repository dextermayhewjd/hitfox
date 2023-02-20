using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_ItemHolder : MonoBehaviour, IInteractable, IStealable
{
    public enum State {IDLE, CHASE}

    public Inventory inventory;
    State state;

    //Transform that NPC has to follow
    public Transform transformToFollow;
    //NavMesh Agent variable
    UnityEngine.AI.NavMeshAgent agent;


    // Start is called before the first frame update
    void Start()
    {
        Inventory inventory = new Inventory();
        this.inventory = inventory;

        state = State.IDLE;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log("itemholder NPC started");
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.CHASE) {
            agent.destination = transformToFollow.position;
        }
        // Debug.Log(state);
    }

    public void Interact() {
        Debug.Log("Interacted");
        state = State.CHASE;
        Debug.Log("Interacted; chasing!");
        CalmDown(5);
    }

    public void StealFrom(SC_CharacterController characterController, Item item) {
        this.inventory.RemoveItem(item);
        characterController.inventory.AddItem(item);
        Debug.Log("stolen");
        state = State.CHASE;
        CalmDown(5);
    }

    public List<Item> CheckItems() {
        return this.inventory.GetItemList();    
    }

    private IEnumerator CalmDown(int secs) {
        yield return new WaitForSeconds(secs);
        state = State.IDLE;
    }
}
