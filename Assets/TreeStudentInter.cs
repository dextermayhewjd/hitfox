using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStudentInter : MonoBehaviour
{
    //this is the script that replaces SMTree03 with PosterTree if collided with StudentNPC 
    //and replaces PosterTree with tree is collided with plaeyr for 1sec and player held E for one second

    public float studentCollideTime = 3f;  // How long to collide with studentNPC before swapping to treewithposter
    public float playerHoldTime = 1f;  // How long to hold E to swap back to tree from treewithposter
    public GameObject treePrefab;  // The default tree prefab
    public GameObject treeWithPosterPrefab;  // The tree prefab with the poster on it

    private bool isStudentColliding = false;  // Is the tree currently colliding with studentNPC?
    private float studentCollideTimer = 0f;  // Timer for how long the tree has been colliding with studentNPC
    private bool isPlayerColliding = false;  // Is the player currently colliding with the tree?
    private float playerHoldTimer = 0f;  // Timer for how long the player has been holding E

    void Update()
    {
        // Check for player input to swap back to tree from treewithposter
        if (isPlayerColliding && Input.GetKey(KeyCode.E))
        {
            playerHoldTimer += Time.deltaTime;
            if (playerHoldTimer >= playerHoldTime)
            {
                SwapToTree();
            }
        }
        else
        {
            playerHoldTimer = 0f;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // Check for collision with studentNPC to swap to treewithposter
        if (other.gameObject.tag == "StudentNPC")
        {
            isStudentColliding = true;
        }
    }

    void OnCollisionStay(Collision other)
    {
        // Increment timer for collision with studentNPC
        if (other.gameObject.tag == "StudentNPC")
        {
            studentCollideTimer += Time.deltaTime;
            if (studentCollideTimer >= studentCollideTime)
            {
                SwapToTreeWithPoster();
            }
        }
    }

    void OnCollisionExit(Collision other)
    {
        // Reset collision and timer variables
        if (other.gameObject.tag == "StudentNPC")
        {
            isStudentColliding = false;
            studentCollideTimer = 0f;
        }
    }

    // Swap the tree prefab to treewithposter
    void SwapToTreeWithPoster()
    {
        GameObject newTree = Instantiate(treeWithPosterPrefab, transform.position, transform.rotation);
        newTree.transform.parent = transform.parent;
        Destroy(gameObject);
    }

    // Swap treewithposter back to the default tree prefab
    void SwapToTree()
    {
        GameObject newTree = Instantiate(treePrefab, transform.position, transform.rotation);
        newTree.transform.parent = transform.parent;
        Destroy(gameObject);
    }
}
