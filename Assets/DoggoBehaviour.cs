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
                    if (distance > Mathf.Lerp(2,5,fearfulness/100))
                        transform.Translate(speed * Vector3.Normalize(player.transform.position - transform.position), Space.World);
                    else state = DoggoState.BARK;
                }
                break;
            case DoggoState.BARK:
                if (timer > 20) {
                    if (distance > 0.5 * reactivity) state = DoggoState.SEARCH;
                    if (Random.Range(0, 100) < aggression)
                        state = DoggoState.ATTACK;
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
            default:
                break;
        }
    }
}
