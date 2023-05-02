using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosterTreeToTree : MonoBehaviour 
{
    public float studentCollideTime = 3f;  // How long to collide with studentNPC before swapping to treewithposter
    public float playerHoldTime = 1f;  // How long to hold E to swap back to tree from treewithposter


    private bool isStudentColliding = false;  // Is the tree currently colliding with studentNPC?
    private float studentCollideTimer = 0f;  // Timer for how long the tree has been colliding with studentNPC
    private bool isPlayerColliding = false;  // Is the player currently colliding with the tree?
    private float playerHoldTimer = 0f;  // Timer for how long the player has been holding E

    public GameObject poster;
    void Start()
    {
        poster.SetActive(false); // disable the child renderer initially
    }

    void Update()
    {}
    //working
    void OnTrigggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("PLAYER COLLIDED");
            isPlayerColliding = true;
        }
        else if (col.gameObject.tag == "StudentNPC")
        {
            Debug.Log("Student collided");
            isStudentColliding = true;
        }
    }
    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "StudentNPC")
        {
            Debug.Log("Student collided");
            studentCollideTimer += Time.deltaTime;
            if (studentCollideTimer >= studentCollideTime)
            {
                SwapToTreeWithPoster();
            }
        }
        else if ((col.gameObject.tag == "Player") && (poster.activeSelf == true))
        {
            Debug.Log("Player collided");
            if(Input.GetKey(KeyCode.E))
            {
                playerHoldTimer += Time.deltaTime;
                if (playerHoldTimer >= playerHoldTime)
                {
                    SwapToTree();
                }
            }
       
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Reset collision and timer variables
        if (other.gameObject.tag == "StudentNPC")
        {
            isStudentColliding = false;
            studentCollideTimer = 0f;
        }
        else if(other.gameObject.tag == "Player" && Input.GetKey(KeyCode.E))
        {
            isPlayerColliding = false;
            playerHoldTime = 0f;
        }
    }

    // Swap the tree prefab to treewithposter
    void SwapToTreeWithPoster()
    {
        poster.SetActive(true);
    }

    // Swap treewithposter back to the default tree prefab
    void SwapToTree()
    {
/*        GameObject objectives = GameObject.Find("Timer+point");
        Debug.Log("Poster taken down get 5 points");
        objectives.GetComponent<Timer>().IncreaseScore(5);*/
        poster.SetActive(false);
    }
}
