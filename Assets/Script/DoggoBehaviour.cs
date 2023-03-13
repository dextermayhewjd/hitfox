using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoggoBehaviour : MonoBehaviour {
    // Start is called before the first frame update
    public float speed = 3;
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

    private float timer = 0;

    public DoggoState state = DoggoState.WALK;
    public GameObject interactingWith;
    public GameObject woof;
    PhotonView view;

    UnityEngine.AI.NavMeshAgent agent;

    public enum DoggoState {
        WALK,
        RUNTOWARD,
        RUNAWAY,
        ATTACK,
        BARK,
        SEARCH,
        PLAY

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
                    foreach (GameObject dog in dogs) {
                        float ddistance = Vector3.Distance(dog.transform.position, transform.position);
                        if (ddistance < reactivity && CanSee(dog, ddistance)) {
                            interactingWith = dog;

                            state = DoggoState.RUNTOWARD;
                        }
                    }
                    foreach (GameObject player in players) {
                        float pdistance = Vector3.Distance(player.transform.position, transform.position);
                        if (pdistance < reactivity && CanSee(player, pdistance)) {
                            interactingWith = player;

                            state = DoggoState.RUNTOWARD;
                        }
                    }


                    break;
                case DoggoState.RUNTOWARD:
                    if (interactingWith != null) {
                        if (distance > Mathf.Lerp(2, 5, fearfulness / 100))
                            agent.destination = interactingWith.transform.position;
                        else if (interactingWith.tag == "Player") {
                            state = DoggoState.BARK;
                            agent.destination = transform.position;
                        } else if (interactingWith.tag == "Dog") {
                            state = DoggoState.PLAY;
                        }
                    }
                    break;
                case DoggoState.BARK:

                    transform.LookAt(interactingWith.transform, Vector3.up);
                    if (Random.Range(0, 100) < 0.001) Instantiate(woof, transform);
                    if (timer > 5) {

                        if (distance > 3) state = DoggoState.SEARCH;
                        /*if (Random.Range(0, 100) < aggression)
                            state = DoggoState.ATTACK;*/
                        else if (Random.Range(0, 100) < fearfulness && interactingWith.tag == "Player") {
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

                    break;
                case DoggoState.PLAY:
                    agent.destination = interactingWith.transform.GetChild(2).position;
                    break;

                default:
                    break;
            }
        }
    }
    bool CanSee(GameObject o, float distance) {
        float angle = Vector3.Angle(Vector3.Normalize(o.transform.position - transform.position), transform.forward);

        if (Mathf.Abs(angle) < fov) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.Normalize(o.transform.position - transform.position), out hit, distance)) {
                if (hit.collider.transform.parent != null) {

                    return hit.collider.transform.parent.gameObject.GetInstanceID() == o.GetInstanceID();
                }

                return false;
            }
        }
        return false;
    }
}
