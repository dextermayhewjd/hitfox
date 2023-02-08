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

    private float timer = 0;

    public DoggoState state = DoggoState.RUNTOWARD;
    public GameObject player;
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
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        reactivity = Random.Range(5, 10);
        fearfulness = Random.Range(0, 100); 
        responsiveness = Random.Range(0, 100);
        aggression = Random.Range(0, 100);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update(){
        float distance = Vector3.Distance(player.transform.position, transform.position);
        switch (state) {
            
            case DoggoState.WALK:
                if (distance < reactivity) state = DoggoState.RUNTOWARD;
                break;
            case DoggoState.RUNTOWARD:
                if (player != null) {
                    if (distance > Mathf.Lerp(2, 5, fearfulness / 100))
                        agent.destination = player.transform.position;
                    else { 
                        state = DoggoState.BARK;
                        agent.destination = transform.position;
                    }
                }
                break;
            case DoggoState.BARK:
                if (Random.Range(0, 100) <0.001) Instantiate(woof, transform);
                if (timer > 10) {
                    
                    if (distance > 3) state = DoggoState.SEARCH;
                    /*if (Random.Range(0, 100) < aggression)
                        state = DoggoState.ATTACK;*/
                    else if (Random.Range(0, 100) < fearfulness) state = DoggoState.RUNAWAY;
                    timer = 0;
                }
                timer += Time.deltaTime;
                break;
            case DoggoState.ATTACK:

                break;
            
            case DoggoState.SEARCH:
                //Temp
                state = DoggoState.RUNTOWARD;
                break;
            case DoggoState.RUNAWAY:
                agent.destination = player.transform.position + Random.Range(5,10)*(transform.position-player.transform.position);
                break;
            default:
                break;
        }
    }
}
