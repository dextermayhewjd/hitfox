using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC_ItemHolder : MonoBehaviour, IInteractable, IStealable
{
    public enum State {IDLE, CHASE}

    public SC_CharacterController aggro;

    public Inventory inventory;
    State state;

    //Transform that NPC has to follow
    public Transform transformToFollow;
    //NavMesh Agent variable
    UnityEngine.AI.NavMeshAgent agent;
    PhotonView view;


    // Start is called before the first frame update
    void Start()
    {
        Inventory inventory = new Inventory();
        this.inventory = inventory;
        this.inventory.AddItem(new Item{itemType=Item.ItemType.Chainsaw, amount=1});

        this.state = State.IDLE;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        PhotonView view = new PhotonView();
        view = GetComponent<PhotonView>();
        Debug.Log("itemholder NPC started");
    }

    // Update is called once per frame
    void Update()
    {
        view = GetComponent<PhotonView>();
        if(view.IsMine) {    
            if(this.state == State.CHASE) {
                agent.destination = transformToFollow.position;
            }
            // Debug.Log(state);
        }
    }

    public void Interact(SC_CharacterController interactor) {
        Debug.Log("Interacted");
        this.state = State.CHASE;
        Debug.Log("Interacted; chasing!");
        StartCoroutine(npc.GetComponent<NPC_ItemHolder>().CalmDown(5));
        // transformToFollow = interactor.transform;
        // Debug.Log("aggro'd onto:");
        // Debug.Log(interactor);

        // foreach(GameObject npc in GameObject.FindGameObjectsWithTag("BlueNPC")) {
        //     npc.GetComponent<NPC_ItemHolder>().state = State.CHASE;
        //     StartCoroutine(npc.GetComponent<NPC_ItemHolder>().CalmDown(5));
        // }
    }

    public void StealFrom(SC_CharacterController characterController, Item item) {
        this.inventory.RemoveItem(item);
        characterController.inventory.AddItem(item);
        Debug.Log("stolen:");
        Debug.Log(item.itemType);
        this.state = State.CHASE;
        StartCoroutine(CalmDown(5));
    }

    public List<Item> CheckItems() {
        return this.inventory.GetItemList();    
    }


    private IEnumerator CalmDown(int secs) {
        yield return new WaitForSeconds(secs);
        this.state = State.IDLE;
        Debug.Log("calmed down");
    }
}
