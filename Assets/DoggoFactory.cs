using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoFactory : MonoBehaviour{
    public GameObject doggo;
    // Start is called before the first frame update
    void Start(){
        Instantiate(doggo,transform,false);
    }

    // Update is called once per frame
    void Update(){
        
    }
}
