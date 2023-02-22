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
        this.inventory.AddItem(new Item{itemType=Item.ItemType.Chainsaw, amount=1});

        this.state = State.IDLE;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log("itemholder NPC started");
    }

    // Update is called once per frame
    void Update()
    {
        if(this.state == State.CHASE) {
            agent.destination = transformToFollow.position;
        }
        // Debug.Log(state);
    }

    public void Interact() {
        Debug.Log("Interacted");
        this.state = State.CHASE;
        Debug.Log("Interacted; chasing!");
        AngerThenCalmDown(5);
    }

    public void StealFrom(SC_CharacterController characterController, Item item) {
        this.inventory.RemoveItem(item);
        characterController.inventory.AddItem(item);
        Debug.Log("stolen:");
        Debug.Log(item.itemType);
        this.state = State.CHASE;
        AngerThenCalmDown(3);
    }

    public List<Item> CheckItems() {
        return this.inventory.GetItemList();    
    }

    private void AngerThenCalmDown(int secs) {
        this.state = State.CHASE;
        StartCoroutine(CalmDown(secs));
        this.state = State.IDLE;
    }


    private IEnumerator CalmDown(int secs) {
        yield return new WaitForSeconds(secs);
        this.state = State.IDLE;
        Debug.Log("calmed down");
    }
}
