using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System;
using System.Linq;
using static TeeFallAnim;

public class NPC_Woodcutter : MonoBehaviour, IInteractable {

    public enum WoodcutterState { 
        SEEKINGTREE,
        CUTTING,
        CURIOUS,
        CHASE
    }

    // Start is called before the first frame update
    public float speed;
    public float chaseDistance; // distance at which it will chase a fox
    public float curiousDistance;
    public float fov = 120;

    public int calmTime; // for calmTime secs after loses sight of player, can still go into chase mode if they catch sight of a player

    public WoodcutterState state = WoodcutterState.SEEKINGTREE;

    public float cutDistance;
    
    public GameObject treeToCut;
    public Animator treeAnimator;

    PhotonView view;

    UnityEngine.AI.NavMeshAgent agent;

    public float distanceToTree;

    public bool isCutting;
    public float catchDistance; // range of catch

    public GameObject chasedPlayer;

    public Vector3 dest;
    private Animator anim;


    public bool calming;


    GameObject FindClosestTarget(string trgt) {
        GameObject closestGameObject = GameObject.FindGameObjectsWithTag(trgt)
                        .OrderBy(go => Vector3.Distance(go.transform.position, transform.position))
                        .FirstOrDefault();
            return closestGameObject;
    }


    void Start(){
        GetComponent<Rigidbody>().isKinematic = true;

        isCutting = false;

        chaseDistance = 20;
        curiousDistance = 50;
        speed = 3;
        calmTime = 5;
        cutDistance = 3.0f;
        catchDistance = 5.0f;
        calming = false;


        if (treeToCut == null) {
            treeToCut = GameObject.FindGameObjectWithTag("Tree");
        }

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log(agent);
        view = GetComponent<PhotonView>();

        agent.speed = speed;

    }

    public void Interact() {
        Debug.Log("Interacted with NPC");
        chasedPlayer = FindClosestTarget("Player");
        state = WoodcutterState.CHASE;
    }

    // Update is called once per frame
    void Update(){
        dest = agent.destination;
        if (view.IsMine) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

            if (treeToCut != null) {
                distanceToTree = Vector3.Distance(treeToCut.transform.position, transform.position);
            } else {
                treeToCut = trees[UnityEngine.Random.Range(0, trees.Length)];
                distanceToTree = Vector3.Distance(treeToCut.transform.position, transform.position);
            }

            switch (state) {
                case WoodcutterState.SEEKINGTREE:
                    agent.destination = treeToCut.transform.position;

                    if (distanceToTree < cutDistance) {
                        Debug.Log("should be cutting!");
                        state = WoodcutterState.CUTTING;
                    }
                    break;
                
                case WoodcutterState.CUTTING:
                    // TODO: cutting sound and animation
                    if(!isCutting) StartCoroutine(CutTree(3, treeToCut)); // secs to cut a tree
                    isCutting = true;
                    break;

                case WoodcutterState.CHASE:
                    // TODO: sound and animation
                    // Debug.Log("chasing 1");
                    treeToCut = null;
                    
                    if(chasedPlayer == null) {
                        chasedPlayer = FindClosestTarget("Player"); // TODO: closest NON-CAGED player   
                    }

                    agent.destination = chasedPlayer.transform.position;
                    
                    float distance = Vector3.Distance(chasedPlayer.transform.position, transform.position);
                    PlayerMovement pm = chasedPlayer.GetComponent<PlayerMovement>();

                    if(distance < catchDistance && CanSee(chasedPlayer, distance) && !pm.captured) {
                        pm.Catch();
                    } else if(pm.captured) {
                        state = WoodcutterState.SEEKINGTREE;
                    }

                    if(distance < curiousDistance && distance > chaseDistance && !pm.captured) {
                        state = WoodcutterState.CURIOUS;
                        goto case WoodcutterState.CURIOUS;
                    }
                

                    break;
                
                case WoodcutterState.CURIOUS:
                    // for calmTime secs after loses sight of player, they can still go into chase mode if they catch sight of a player

                    if(!calming) {
                        StartCoroutine(CalmDown(calmTime));
                        calming = true;
                    }
                    
                    foreach (GameObject player in players) {
                        float pdistance = Vector3.Distance(player.transform.position, transform.position);
                        if(pdistance < chaseDistance && CanSee(player, pdistance)) {
                            state = WoodcutterState.CHASE;
                        }
                    }
                  break;
                
            }
        }
    }

    private IEnumerator CutTree(int secs, GameObject tree) {
        yield return new WaitForSeconds(secs);
        tree.tag = "CutTree";
        //tree.GetComponent<Animator>().enabled = true;
        treeAnimator = tree.GetComponent<Animator>();
        if (treeAnimator != null)
        {
            Debug.Log("Animator detected!");
            treeAnimator.enabled = true;
            treeAnimator.Play("TreeFalling");
            yield return new WaitForSeconds(1);
            treeAnimator.enabled = false;

        }

        // Destroy(tree);
        //tree.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        treeToCut = null;
        Debug.Log("cut a tree");
        state = WoodcutterState.SEEKINGTREE;
        isCutting = false;
    }

    private IEnumerator CalmDown(int secs) {
        yield return new WaitForSeconds(secs);
        state = WoodcutterState.SEEKINGTREE;
        Debug.Log("Lost him!");
        calming = false;
        // TODO: points
    }

    bool CanSee(GameObject obj, float distance) { 
        float angle = Vector3.Angle(Vector3.Normalize(obj.transform.position - transform.position), transform.forward);
       
        if (Mathf.Abs(angle) < fov) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.Normalize(obj.transform.position - transform.position), out hit, distance)) {
                if (hit.collider.transform.parent != null) {
                    return hit.collider.transform.parent.gameObject.GetInstanceID() == obj.GetInstanceID();
                }
                return false;
            }
        }
        return false;
    }
}
