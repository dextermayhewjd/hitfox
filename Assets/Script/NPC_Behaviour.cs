using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor.Animations;
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
    Animator anim;
    PhotonView view;
    float recallTimer = 0;
    float emoteTimer = 0;
    float talkTimer = 0;
    float invincTimer;
    float angry;
    float fov = 180;
    float loseDistance = 20;
    public float sitTimer;
    public List<GameObject> npcsTalkingWith = new List<GameObject>();
    public GameObject playerChasing;


    

    public enum State {
        WALK,
        SIT,
        CONVERSATION,
        CHASE
    }
    public enum Emotion {
        HAPPY,
        ANGRY,
        QUESTION
    }
    Dictionary<GameObject, float> relationships = new Dictionary<GameObject, float>();

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        view = GetComponent<PhotonView>();
        emotions = new Canvas[] { happySign, angrySign, questionSign };
        happySign.enabled = false;
        angrySign.enabled = false;
        questionSign.enabled = false;
        anim = GetComponentInChildren<Animator>();
        angry = Random.Range(0, 25);
  
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            emoteTimer -= Time.deltaTime;
            invincTimer -= Time.deltaTime;
            if (emoteTimer < 0) this.view.RPC("RPC_HideSign", RpcTarget.All);
            switch (state) {
                case State.WALK:
                    
                    agent.destination = goTo.transform.position;
                    if (Vector3.Distance(transform.position, goTo.transform.position) < 2) {
                        if (goTo.tag == "NPCFactory") {
                            if (dog != null) {
                                if (Vector3.Distance(transform.position, dog.transform.position) < 2) {
                                    PhotonNetwork.Destroy(dog);
                                    PhotonNetwork.Destroy(gameObject);
                                }

                                if (recallTimer <= 0) {
                                    dog.GetComponent<PhotonView>().RPC("recall",RpcTarget.All);
                                    recallTimer = 5;
                                }
                                recallTimer -= Time.deltaTime;
                            } else PhotonNetwork.Destroy(gameObject);
                        } else {
                            state = State.SIT;
                            sitTimer = Random.Range(60, 180);
                            anim.SetBool("Walking",false);
                            anim.SetBool("Sitting",true);
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
                        anim.SetBool("Sitting",false);
                        anim.SetBool("Walking",true);
                    }
                    break;
                case State.CONVERSATION:
                    if (talkTimer < 0 || npcsTalkingWith.Count == 0) {
                        npcsTalkingWith.ForEach((x)=>x.GetComponent<PhotonView>().RPC("endConversation", RpcTarget.All,view.ViewID));
                        npcsTalkingWith.Clear();
                        anim.SetBool("Sitting", false);
                        anim.SetBool("Standing", false);
                        anim.SetBool("Walking", true);

                        state = State.WALK;
                    }

                    if (dog != null) {
                        if (Vector3.Distance(dog.transform.position, transform.position) > 8) {
                            dog.GetComponent<PhotonView>().RPC("recall", RpcTarget.All);
                        }
                    }

                    agent.destination = transform.position;
                    talkTimer -= Time.deltaTime;
                    Vector3 pos = Vector3.zero;
                    npcsTalkingWith.ForEach((x)=>pos+=x.transform.position);
                    pos /= npcsTalkingWith.Count;
                    transform.LookAt(pos);
                    
                    if (activeSign.enabled == false) {
                        int i = Random.Range(0, emotions.Length);
                        this.view.RPC("RPC_ShowSign", RpcTarget.All, (Emotion)i);
                    }
                    break;
                case State.CHASE:
                    agent.destination = playerChasing.transform.position;
                    if (!CanSee(playerChasing, loseDistance)) {
                        Debug.Log("Player Lost");
                        state = State.WALK;
                    }
                    break;
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        GameObject o = other.gameObject;
        if (view.IsMine) {
            if (o.tag == "NPC" && state != State.CONVERSATION) {
                if (relationships.ContainsKey(o)) {
                    int i = Random.Range(25, 100);
                    if (i < relationships[o]) {
                        o.GetComponent<PhotonView>().RPC("initiateConversation", RpcTarget.All, view.ViewID);
                        state = State.CONVERSATION;
                        npcsTalkingWith.Add(o);
                        talkTimer = relationships[o];
                        anim.SetBool("Sitting",false);
                        anim.SetBool("Walking",false);
                        anim.SetBool("Standing",true);
                    }
                } else {
                    int i = Random.Range(0, 100);
                    relationships.Add(o, i);
                    int j = Random.Range(26, 100);
                    if (j < i) {
                        o.GetComponent<PhotonView>().RPC("initiateConversation", RpcTarget.All, view.ViewID);
                        state = State.CONVERSATION;
                        npcsTalkingWith.Add(o);
                        talkTimer = i;
                        anim.SetBool("Sitting",false);
                        anim.SetBool("Walking",false);
                        anim.SetBool("Standing",true);
                    }
                }
            } else if (o.tag == "Player") {
                if (state != State.CHASE) {
                    if (angry > 20) {
                        this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, Emotion.ANGRY);
                        if (state != State.CONVERSATION) {
                            playerChasing = o;
                            invincTimer = 2;
                            state = State.CHASE;
                            anim.SetBool("Sitting",false);
                            anim.SetBool("Walking", true);
                        }
                    } else {
                        this.view.RPC("RPC_ShowSign", RpcTarget.AllBuffered, Emotion.QUESTION);
                        angry += Random.Range(0, 5);
                    }
                } 

                //o.GetComponent<PlayerMovement>().Catch();
            }
        }
    }

    public void OnTriggerStay(Collider other) {
        if(view.IsMine && other.tag == "Player" && state == State.CHASE && invincTimer <0) {
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if (!movement.captured) { 
                movement.Catch();
                state = State.WALK;
            }
        }
    }

    public void OnCollisionEnter(Collision collision) {
        GameObject o = collision.gameObject;
        Debug.Log("Player Touching");
        if (o.tag == "Player") this.view.RPC("RPC_ShowSign", RpcTarget.All, Emotion.ANGRY);
    }

    [PunRPC]
    void RPC_ShowSign(Emotion e) {
        activeSign.enabled = false;
        Canvas c = emotions[(int)e];
        c.enabled = true;
        activeSign = c;
        emoteTimer = 2;
    }

    [PunRPC]
    void RPC_HideSign() {
        activeSign.enabled = false;
    }

    [PunRPC]
    void initiateConversation(int npcID) {
       
        if (view.IsMine) {
            anim.SetBool("Sitting",false);
            anim.SetBool("Walking",false);
            anim.SetBool("Standing",true);
            state = State.CONVERSATION;
            GameObject npc = PhotonView.Find(npcID).gameObject;
            transform.LookAt(npc.transform);
            if(relationships.ContainsKey(npc))talkTimer = relationships[npc];
            npcsTalkingWith.Add(npc);
        }
    }

    [PunRPC]
    void endConversation(int npcID) {
        
        if (view.IsMine) {
            GameObject npc = PhotonView.Find(npcID).gameObject;
            npcsTalkingWith.Remove(npc);
            if (npcsTalkingWith.Count == 0) {
                state = State.WALK;
                anim.SetBool("Sitting",false);
                anim.SetBool("Standing",false);
                anim.SetBool("Walking",true);
            }
            
        }
        
        
    }
    bool CanSee(GameObject o, float distance) {
        float angle = Vector3.Angle(Vector3.Normalize(o.transform.position - transform.position), transform.forward);

        if (Mathf.Abs(angle) < fov) {
            int layerMask = ~LayerMask.GetMask("NPC");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.Normalize(o.transform.position - transform.position), out hit, distance, layerMask)) {
                if (hit.collider.transform.parent != null) {

                    return hit.collider.transform.parent.gameObject.GetInstanceID() == o.GetInstanceID();
                }
                return hit.collider.gameObject.GetInstanceID() == o.GetInstanceID();

            }
        }
        return false;
    }
}