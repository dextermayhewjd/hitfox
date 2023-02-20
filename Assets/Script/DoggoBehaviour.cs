using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoBehaviour : MonoBehaviour{
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

    UnityEngine.AI.NavMeshAgent agent;

    public enum DoggoState { 
        WALK,
        RUNTOWARD,
        RUNAWAY,
        ATTACK,
        BARK,
        SEARCH

    }
    void Start(){
        if (interactingWith == null) {
            interactingWith = GameObject.FindGameObjectWithTag("Player");
        }
        reactivity = Random.Range(5, 10);
        fearfulness = Random.Range(0, 100); 
        responsiveness = Random.Range(0, 100);
        aggression = Random.Range(0, 100);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update(){
        float distance = Vector3.Distance(interactingWith.transform.position, transform.position);
        float angle = Vector3.Angle(interactingWith.transform.position - transform.position, transform.forward);
        switch (state) {
            
            case DoggoState.WALK:
                if (distance < reactivity && CanSee(interactingWith,distance)) { 
                    state = DoggoState.RUNTOWARD;
                }

                    break;
            case DoggoState.RUNTOWARD:
                if (interactingWith != null) {
                    if (distance > Mathf.Lerp(2, 5, fearfulness / 100))
                        agent.destination = interactingWith.transform.position;
                    else if(interactingWith.tag == "Player") { 
                        state = DoggoState.BARK;
                        agent.destination = transform.position;
                    }
                }
                break;
            case DoggoState.BARK:
                if (Random.Range(0, 100) <0.001) Instantiate(woof, transform);
                if (timer > 5) {

                    if (distance > 3) state = DoggoState.SEARCH;
                    /*if (Random.Range(0, 100) < aggression)
                        state = DoggoState.ATTACK;*/
                    else if (Random.Range(0, 100) < fearfulness) { 
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
            default:
                break;
        }
    }
    bool CanSee(GameObject o,float distance) { 
        float angle = Vector3.Angle(Vector3.Normalize(o.transform.position - transform.position), transform.forward);

        if (Mathf.Abs(angle) < fov) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.Normalize(o.transform.position - transform.position), out hit, distance)) {
                Debug.Log("Hit " + hit.collider.gameObject.name + "," + o.name, hit.collider.gameObject);
                if (hit.collider.transform.parent != null) {
                    
                    return hit.collider.transform.parent.gameObject.GetInstanceID() == o.GetInstanceID();
                }
                Debug.Log("Hit nothing");
                return false;
            }
        }
        return false;
    }
}
