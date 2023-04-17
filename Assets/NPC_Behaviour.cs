using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC_Behaviour : MonoBehaviour{

    public GameObject goTo;
    public State state;
    UnityEngine.AI.NavMeshAgent agent;
    PhotonView view;

    public enum State { 
        WALK
    }

    // Start is called before the first frame update
    void Start(){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            switch (state) {
                case State.WALK: agent.destination = goTo.transform.position;
                    break;
            }
        }
    }
}
