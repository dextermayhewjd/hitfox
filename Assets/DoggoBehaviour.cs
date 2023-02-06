using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoBehaviour : MonoBehaviour{
    // Start is called before the first frame update
    public float speed = 3;
    public float stopDist = 2;
    //Chance to approach  
    public float reactivity;
    //Chance to run after approaching
    public float fearfulness;
    //Chance to respond to owner's call
    public float responsiveness;
    //Chance to attack
    public float aggression;

    private DoggoState state = DoggoState.WALK;
    public GameObject player;

    enum DoggoState { 
        WALK,
        RUN,
        ATTACK,
        BORK,
        SEARCH

    }
    void Start(){
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        reactivity = Random.Range(0, 10);
        fearfulness = Random.Range(0, 10); 
        responsiveness = Random.Range(0, 10);
        aggression = Random.Range(0, 10);
    }

    // Update is called once per frame
    void Update(){
        switch (state) {
            case DoggoState.WALK:

                break;
            case DoggoState.RUN:
                break;
            case DoggoState.ATTACK:
                break;
            case DoggoState.BORK:
                break;
            case DoggoState.SEARCH:
                break;
            default:
                break;
        }
    }
}
