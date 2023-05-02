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
    public Canvas questionSign;

    Canvas[] emotions;

    public State state;
    UnityEngine.AI.NavMeshAgent agent;
    PhotonView view;
    float recallTimer = 0;
    float emoteTimer = 0;
    float talkTimer= 0;
    public float sitTimer;
    public GameObject npcTalkingWith;
    struct ConvoParams {
        public GameObject npc;
        public bool rpc;
    }

    public enum State { 
        WALK,
        SIT,
        CONVERSATION
    }
    public enum Emotion {
        HAPPY,
        ANGRY,
        QUESTION
    }
    Dictionary<GameObject, float> relationships = new Dictionary<GameObject, float>();

    // Start is called before the first frame update
    void Start(){
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        view = GetComponent<PhotonView>();
        emotions = new Canvas[]{ happySign, angrySign, questionSign };
        happySign.enabled = false;
        angrySign.enabled = false;
        questionSign.enabled = false;
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            emoteTimer -= Time.deltaTime;
            if (emoteTimer < 0) this.view.RPC("RPC_HideSign", RpcTarget.AllBuffered);
            switch (state) {
                case State.WALK: 
                    agent.destination = goTo.transform.position;
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
                case State.CONVERSATION:
                    agent.destination = transform.position;
                    talkTimer -= Time.deltaTime;
                    if (talkTimer < 0) {
                        npcTalkingWith.BroadcastMessage("endConversation", false);
                        state = State.WALK;
                    }
                    if (activeSign.enabled == false) {
                        int i = Random.Range(0, emotions.Length);
                        this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, (Emotion)i);
                    }
                    break;
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        GameObject o = other.gameObject;
        if (view.IsMine) {
            if (o.tag == "NPC"&&state != State.CONVERSATION) {
                if (relationships.ContainsKey(o)) {
                    int i = Random.Range(0, 100);
                    if (i < relationships[o]) {
                        ConvoParams param;
                        param.npc = gameObject;
                        param.rpc = false;
                        o.BroadcastMessage("initiateConversation");
                        transform.LookAt(o.transform);
                    }
                }
                else {
                    int i = Random.Range(0, 100);
                    relationships.Add(o, i);
                    int j = Random.Range(0, 100);
                    if (j < i) {
                        ConvoParams param;
                        param.npc = gameObject;
                        param.rpc = false;
                        o.BroadcastMessage("initiateConversation");
                        transform.LookAt(o.transform);
                    }
                }
            }
            else if(o.tag == "Player") this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, Emotion.QUESTION);
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

    void initiateConversation(ConvoParams p) {
        if (!p.rpc) {
            p.rpc = true;
            view.RPC("initiateConversation", RpcTarget.AllBuffered, p);
        }
        if (view.IsMine && state != State.CONVERSATION) {
            state = State.CONVERSATION;
            transform.LookAt(p.npc.transform);
            talkTimer = relationships[p.npc];
            npcTalkingWith = p.npc;
        }
    }

    void endConversation(bool rpc) {
        if (!rpc) {
            view.RPC("endConversation", RpcTarget.AllBuffered, true);
        }
        if (view.IsMine) {
            state = State.WALK;
        }
    }
}
