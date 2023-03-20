using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPC_WoodCutter : MonoBehaviour {
    // Start is called before the first frame update
    public float speed = 3;
    public float stopDist = 2;
    //Distance from player that it will run towards player

    public float fov = 120;

    public WoodcutterState state = WoodcutterState.SEEKINGTREE;

    public float cutDistance = 0.5;
    
    public GameObject treeToCut;
    PhotonView view;

    UnityEngine.AI.NavMeshAgent agent;

    public enum WoodcutterState { 
        SEEKINGTREE,
        CUTTING,
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
                    
                    
            }
        }
    }

    private IEnumerator CutTree(int secs, GameObject tree) {
        yield return new WaitForSeconds(secs);
        tree.tag = "CutTree";
        if (state == WoodcutterState.CUTTING) state = WoodcutterState.SEEKINGTREE;
        Debug.Log("cut a tree");
    }

    bool CanSee(GameObject o,float distance) { 
        float angle = Vector3.Angle(Vector3.Normalize(o.transform.position - transform.position), transform.forward);
       
        if (Mathf.Abs(angle) < fov) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.Normalize(o.transform.position - transform.position), out hit, distance)) {
                if (hit.collider.transform.parent != null) {
                    
                    return hit.collider.transform.parent.gameObject.GetInstanceID() == o.GetInstanceID();
                }
                
                return false;
            }
        }
        return false;
    }
}
