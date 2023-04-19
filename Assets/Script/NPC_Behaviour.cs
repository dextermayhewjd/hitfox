using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC_Behaviour : MonoBehaviour{

    public GameObject goTo;
    public GameObject dog;
    public State state;
    UnityEngine.AI.NavMeshAgent agent;
    PhotonView view;
    float recallTimer = 0;

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
                    if (Vector3.Distance(transform.position, goTo.transform.position) < 1) {
                        if (dog != null) {
                            if (Vector3.Distance(transform.position, dog.transform.position) < 2)) {
                                Destroy(dog);
                                Destroy(gameObject);
                            }

                            if (recallTimer <= 0) {
                                dog.BroadcastMessage("recall");
                                recallTimer = 5;
                            }
                            recallTimer -= Time.deltaTime;
                        }
                        
                        else Destroy(gameObject);
                    }
                    break;
            }
        }
    }

   
}
