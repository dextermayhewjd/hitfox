using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Diagnostics;
//new version

public class Player : MonoBehaviour
{
    public float speed = 3.5f;
    
    public float wBound = 3.8f;
    public float sBound = -3.8f;
    public float aBound = 3.8f;
    public float dBound = -3.8f;
    // define the speed of an object


    // Start is called before the first frame update
    void Start()
    {
        // take the current position = new position (0,0,0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); 
        // moving towards left or right using key A and D
        
        float verticalInput = Input.GetAxis("Vertical");    
        //moving forwards or backwards using key W and S
        
        
        
        //transform.Translate(Vector3.right * horizontalInput *speed * Time.deltaTime);
        //transform.Translate(Vector3.forward * verticalInput *speed * Time.deltaTime); 
        
        Vector3 direction = new Vector3(horizontalInput,0,verticalInput);
        transform.Translate(direction * speed * Time.deltaTime);
        // used to replace the two line of code above 
        //https://docs.unity3d.com/ScriptReference/Vector3.html
        // the url above is the doc for vector3

        
        
        //code used to constriain the movement of the object
        // restrict the movement of W and S 

        
        if(transform.position.z >=wBound){
            transform.position = new Vector3(transform.position.x, transform.position.y, wBound );
        }
        else if(transform.position.z <= sBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, sBound );
        }
        
        // restrict the movement of A and D
        if(transform.position.x >=aBound){
            transform.position = new Vector3(aBound, transform.position.y, transform.position.z );
        }
        else if(transform.position.x <= dBound)
        {
            transform.position = new Vector3(dBound, transform.position.y, transform.position.z );
        }
    }
}

