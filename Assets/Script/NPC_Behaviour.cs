using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC_Behaviour : MonoBehaviour{

    public GameObject goTo;
    public GameObject dog;
    public Canvas activeSign;
    public Canvas angrySign;
    public Canvas happySign;

    Canvas[] emotions;

    public State state;
    UnityEngine.AI.NavMeshAgent agent;
    PhotonView view;
    float recallTimer = 0;
    float emoteTimer = 0;
    public float sitTimer;
    

    public enum State { 
        WALK,
        SIT
    }
    public enum Emotion {
        HAPPY,
        ANGRY
    }
    Dictionary<GameObject, Emotion> relationships = new Dictionary<GameObject, Emotion>();

    // Start is called before the first frame update
    void Start(){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        view = GetComponent<PhotonView>();
        emotions = new Canvas[]{ happySign, angrySign };
        happySign.enabled = false;
        angrySign.enabled = false;
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            emoteTimer -= Time.deltaTime;
            if (emoteTimer < 0) this.view.RPC("RPC_HideSign", RpcTarget.AllBuffered);
            switch (state) {
                case State.WALK: agent.destination = goTo.transform.position;
                    if (Vector3.Distance(transform.position, goTo.transform.position) < 1) {
                        if (goTo.tag == "NPCFactory") {
                            if (dog != null) {
                                if (Vector3.Distance(transform.position, dog.transform.position) < 2) {
                                    Destroy(dog);
                                    Destroy(gameObject);
                                }

                                if (recallTimer <= 0) {
                                    dog.BroadcastMessage("recall");
                                    recallTimer = 5;
                                }
                                recallTimer -= Time.deltaTime;
                            } else Destroy(gameObject);
                        } else {
                            state = State.SIT;
                            sitTimer = Random.Range(60, 180);
                        }
                    }
                    break;
                case State.SIT:
                    sitTimer -= Time.deltaTime;
                    if (sitTimer < 0) {
                        PicnicNPCHolder picnic = goTo.GetComponent<PicnicNPCHolder>();
                        picnic.isOccupied = false;
                        GameObject[] factories = GameObject.FindGameObjectsWithTag("NPCFactory");
                        int i = Random.Range(0, factories.Length);
                        goTo = factories[i];
                        if (dog != null) {
                            DoggoBehaviour dogBehaviour = dog.GetComponent<DoggoBehaviour>();
                            dogBehaviour.destination = factories[i];
                        }
                        state = State.WALK;
                    }
                    break;
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        GameObject o = other.gameObject;
        if (view.IsMine) {
            if (o.tag == "NPC") {
                if (relationships.ContainsKey(o)) this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, relationships[o]);
                else {
                    int i = Random.Range(0, emotions.Length);
                    this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, (Emotion)i);
                    relationships.Add(o, (Emotion)i);
                }
            }
            else if(o.tag == "Player") this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, Emotion.ANGRY);
        }
    }

    [PunRPC]
    void RPC_ShowSign(Emotion e) {
        activeSign.enabled = false;
        Canvas c = emotions[(int)e];
        c.enabled = true;
        activeSign = c;
        emoteTimer = 5;
    }
    [PunRPC]
    void RPC_HideSign() {
        activeSign.enabled = false;
    }


}
