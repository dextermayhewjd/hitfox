using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoggoBehaviour : MonoBehaviour {
    // Start is called before the first frame update
    public float speed = 10;
    public float walkSpeed = 5f;
    public float stopDist = 2;
    //Distance from player that it will run towards player
    public float reactivity;
    //Chance to run away when notices player
    public float fearfulness;
    //Chance to respond to owner's call
    public float responsiveness;
    //Chance to attack
    public float aggression;
    //Dog's field of view
    public float fov = 120;

    public float wanderRadius = 10;

    private float timer = 0;
    private float timer2 = 0;

    public DoggoState state = DoggoState.WALK;
    public GameObject interactingWith;
    public GameObject woof;
    public GameObject owner;
    public GameObject destination;
    PhotonView view;

    UnityEngine.AI.NavMeshAgent agent;
    public AudioSource dogbark;

    public enum DoggoState {
        WALK,
        RUNTOWARD,
        RUNAWAY,
        ATTACK,
        BARK,
        SEARCH,
        PLAY,
        RECALL

    }



    void Start() {
        /*if (interactingWith == null) {
            interactingWith = GameObject.FindGameObjectWithTag("Player");
        }*/
        reactivity = Random.Range(5, 10);
        fearfulness = Random.Range(0, 100);
        responsiveness = Random.Range(0, 100);
        aggression = Random.Range(0, 100);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        view = GetComponent<PhotonView>();
        transform.localScale *= Random.Range(0.3f, 0.8f);
    }

    // Update is called once per frame
    void Update() {
        if (view.IsMine) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] dogs = GameObject.FindGameObjectsWithTag("Dog");
            float distance = 0;
            if (interactingWith != null) {
                distance = Vector3.Distance(interactingWith.transform.position, transform.position);

            }
            switch (state) {

                case DoggoState.WALK:
                    timer -= Time.deltaTime;
                    agent.speed = walkSpeed;
                    agent.destination = destination.transform.position;
                    foreach (GameObject dog in dogs) {
                        if (CanSee(dog, reactivity)) {
                            interactingWith = dog;

                            state = DoggoState.RUNTOWARD;
                        }
                    }
                    foreach (GameObject player in players) {
                        if (CanSee(player, reactivity)) {
                            interactingWith = player;

                            state = DoggoState.RUNTOWARD;
                        }
                    }

                    /*Old Wander code
                     * if (timer <= 0) {
                        Vector3 newPos = Random.onUnitSphere;
                        newPos.y = 0;
                        newPos = Vector3.RotateTowards(newPos, transform.forward, Mathf.Deg2Rad * 0.5f * Vector3.Angle(newPos, transform.forward), 0);
                        newPos = Vector3.Normalize(newPos) * wanderRadius;
                        agent.SetDestination(transform.position + newPos);
                        timer = 1;
                        //Debug.Log("Changing direction: "+ newPos + ", " + transform.position);
                    }*/


                    //Debug.DrawRay(transform.position, agent.destination-transform.position, Color.black, 0, false);
                    break;
                case DoggoState.RUNTOWARD:
                    agent.speed = speed;
                    if (interactingWith != null) {
                        if (!CanSee(interactingWith, distance)) state = DoggoState.RECALL;
                        if (distance > Mathf.Lerp(2, 5, fearfulness / 100))
                            agent.destination = interactingWith.transform.position;
                        else if (interactingWith.tag == "Player") {
                            dogbark.enabled = true;
                            state = DoggoState.BARK;
                            agent.destination = transform.position;
                            timer = 0;
                        } else if (interactingWith.tag == "Dog") {
                            state = DoggoState.PLAY;
                        }
                    }
                    break;
                case DoggoState.BARK:

                    transform.LookAt(interactingWith.transform, Vector3.up);
                    if (Random.Range(0, 100) <= 2) {
                        PhotonNetwork.Instantiate(woof.name, transform.position, transform.rotation);
                        Debug.Log("Woof");
                    }
                    if (timer > 5) {

                        if (distance > 3) {
                            dogbark.enabled = false;
                            state = DoggoState.SEARCH;
                        } 
                        /*if (Random.Range(0, 100) < aggression)
                            state = DoggoState.ATTACK;*/
                        else if (Random.Range(0, 100) < fearfulness && interactingWith.tag == "Player") {
                            dogbark.enabled = false;
                            state = DoggoState.RUNAWAY;
                            agent.destination = interactingWith.transform.position + Random.Range(5, 10) * Vector3.Normalize(transform.position - interactingWith.transform.position);
                        }
                        timer = 0;
                    }
                    timer += Time.deltaTime;
                    GameObject[] chasables = GameObject.FindGameObjectsWithTag("DogChase");
                    foreach (GameObject i in chasables) {
                        if (CanSee(i, distance)) {
                            interactingWith = i;
                            dogbark.enabled = false;
                        }
                    }

                    break;
                case DoggoState.ATTACK:

                    break;

                case DoggoState.SEARCH:
                    //Temp
                    state = DoggoState.RUNTOWARD;
                    break;
                case DoggoState.RUNAWAY:
                    if (interactingWith == null) state = DoggoState.RECALL;
                    if (interactingWith.tag == "Dog") {
                        timer -= Time.deltaTime;
                        timer2 -= Time.deltaTime;
                        if (timer <= 0) {
                            Vector3 newPos = Random.onUnitSphere;
                            newPos.y = 0;
                            //newPos = Vector3.RotateTowards(newPos, transform.forward, Mathf.Deg2Rad * 0.5f * Vector3.Angle(newPos, transform.forward), 0);
                            newPos = Vector3.Normalize(newPos) * wanderRadius;
                            agent.SetDestination(transform.position + newPos);
                            timer = 1;
                            //Debug.Log("Chosen new position");
                        }
                        if (timer2 <= 0) {

                            float time = Random.Range(5, 10);
                            object[] param = { view.ViewID, time };
                            view.RPC("becomeChased", RpcTarget.AllBuffered, param);
                            state = DoggoState.RUNTOWARD;
                        }
                        Debug.DrawRay(transform.position, agent.destination - transform.position, Color.black, 0, false);



                    } else {
                        if (distance > reactivity) state = DoggoState.WALK;
                    }
                    break;
                case DoggoState.PLAY:
                    agent.speed = speed;
                    agent.destination = interactingWith.transform.GetChild(2).position;
                    if (Vector3.Distance(transform.position, interactingWith.transform.position) < 2) {
                        float time = Random.Range(10, 20);
                        object[] param = { view.ViewID, time };
                        view.RPC("becomeChased", RpcTarget.AllBuffered, param);
                        state = DoggoState.RUNTOWARD;
                    }
                    foreach (GameObject player in players) {
                        if (CanSee(player, reactivity)) {
                            interactingWith = player;

                            state = DoggoState.RUNTOWARD;
                        }
                    }
                    break;

                case DoggoState.RECALL:
                    agent.speed = speed;
                    agent.destination = owner.transform.position;
                    if (Vector3.Distance(owner.transform.position, transform.position) < 2) state = DoggoState.WALK;
                    break;

                default:
                    break;
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

    [PunRPC]
    void becomeChased(int dogID, float time) {

        if (view.IsMine && interactingWith.tag != "Player" && (state == DoggoState.PLAY || state == DoggoState.RUNTOWARD)) {
            interactingWith = PhotonView.Find(dogID).gameObject;
            state = DoggoState.RUNAWAY;
            timer2 = time;
        }
    }

    [PunRPC]
    void recall() {
        
        /*if(responsiveness > Random.Range(0,100))*/state = DoggoState.RECALL;
    }
}
