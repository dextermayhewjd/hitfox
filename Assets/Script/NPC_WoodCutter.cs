using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC_WoodCutter : MonoBehaviour {
    // Start is called before the first frame update
    public float speed = 3;
    public float chaseDistance = 20; // distance at which it will chase a fox
    public float fov = 120;


    public WoodcutterState state = WoodcutterState.SEEKINGTREE;

    public float cutDistance = 0.5;
    
    public GameObject treeToCut;
    PhotonView view;

    UnityEngine.AI.NavMeshAgent agent;

    public enum WoodcutterState { 
        SEEKINGTREE,
        CUTTING,
        CURIOUS,
        CHASE
    }
    void Start(){
        if (treeToCut == null) {
            treeToCut = GameObject.FindGameObjectWithTag("Tree");
        }

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            float distance = 0;
            if (treeToCut != null) {
                distanceToTree = Vector3.Distance(treeToCut.transform.position, transform.position);
            } else {
                treeToCut = GameObject.FindGameObjectWithTag("Tree");
                distanceToTree = Vector3.Distance(treeToCut.transform.position, transform.position);
            }

            switch (state) {
                case WoodcutterState.SEEKINGTREE:
                    agent.destination = treeToCut.transform.position;
                    if(distanceToTree < cutDistance) {
                        state = WoodcutterState.CUTTING;
                    }
                    break;
                
                case WoodcutterState.CUTTING:
                    // TODO: cutting sound and animation
                    StartCoroutine(CutTree(20, treeToCut)); // 20 secs to cut a tree

                case WoodcutterState.CHASE:
                    // TODO: sound and animation
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    
                    foreach (GameObject player in players) {
                        float distance = Vector3.Distance(player.transform.position, transform.position);
                        if(distance < chaseDistance && CanSee(player, distance)) {

                        }
                    }
                
                case WoodcutterState.CURIOUS: // for 5 secs after loses sight of player, can still go into chase mode if they catch sight of them
                    StartCoroutine(CalmDown(5));

                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    
                    foreach (GameObject player in players) {
                        float distance = Vector3.Distance(player.transform.position, transform.position);
                        if(distance < chaseDistance && CanSee(player, distance)) {
                            state = WoodcutterState.CHASE;
                        }
                    }
                    
            }
        }
    }

    private IEnumerator CutTree(int secs, GameObject tree) {
        yield return new WaitForSeconds(secs);
        tree.tag = "CutTree";
        if (state == WoodcutterState.CUTTING) state = WoodcutterState.SEEKINGTREE;
        Debug.Log("cut a tree");
    }

    private IEnumerator CalmDown(int secs) {
        yield return new WaitForSeconds(secs);
        state = WoodcutterState.SEEKINGTREE;
        Debug.Log("Lost him!");
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
