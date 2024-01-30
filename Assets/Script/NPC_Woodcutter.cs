using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System;
using System.Linq;
using static TeeFallAnim;
using TMPro;

public class NPC_Woodcutter : MonoBehaviourPun {

    public enum WoodcutterState { 
        SEEKINGTREE,
        CUTTING,
        CURIOUS,
        CHASE
    }

    // Start is called before the first frame update
    public float speed;
    public float pauseTime = 0f; // the time lumberjack is stuck
    public float chaseDistance; // distance at which it will chase a fox
    public float curiousDistance;
    public float fov = 120;

    public int calmTime; // for calmTime secs after loses sight of player, can still go into chase mode if they catch sight of a player

    public WoodcutterState state = WoodcutterState.SEEKINGTREE;

    public float cutDistance;
    
    public GameObject treeToCut;
    public Animator treeAnimator;

    PhotonView view;

    public UnityEngine.AI.NavMeshAgent agent;

    public float distanceToTree;

    public bool isCutting;
    public float catchDistance; // range of catch

    public GameObject chasedPlayer;

    public Vector3 dest;
    private Animator anim;


    public bool calming;
    public bool isStunned;
    public Canvas angrySign;
    public GameObject axe;

    public AudioSource grunt;
    public AudioSource chopping;
    public GameObject hud;
    public PhotonView playerOutside = null;


    GameObject FindClosestTarget(string trgt) {
        GameObject closestGameObject = GameObject.FindGameObjectsWithTag(trgt)
                        .OrderBy(go => Vector3.Distance(go.transform.position, transform.position))
                        .FirstOrDefault();
            return closestGameObject;
    }


    void Start(){
        // GetComponent<Rigidbody>().isKinematic = true; // otherwise he slides down the hill

        isCutting = false;
        isStunned = false;
        chaseDistance = 60;
        curiousDistance = 150;
        speed = 3;
        calmTime = 5;
        cutDistance = 3.0f;
        catchDistance = 2.0f;
        calming = false;
        angrySign.enabled = false;
        anim = GetComponentInChildren<Animator>();

        if (treeToCut == null) {
            treeToCut = GameObject.FindGameObjectWithTag("Tree");
        }

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log(agent);
        view = GetComponent<PhotonView>();

        agent.speed = speed;
        hud = GameObject.Find("HUD");
    }

    public IEnumerator Interact(int secs) {
        // view.RPC("RPC_trigger", RpcTarget.AllBuffered, "RunAngry");
        grunt.Play();
        anim.SetBool("RunAngry", true);
        Debug.Log("Angry Trigger");
        Debug.Log("Interacted with NPC");
        chasedPlayer = FindClosestTarget("Player");
        agent.speed = 0;
        this.photonView.RPC("RPC_UpdateLumberjack", RpcTarget.All, true);
        this.photonView.RPC("RPC_ShowAngrySign", RpcTarget.All);
        if (axe.activeSelf) {
            axe.SetActive(false);
            PhotonNetwork.Instantiate("Axe", axe.transform.position, axe.transform.rotation);
            // this.photonView.RPC("RPC_DropAxe", RpcTarget.All);
        }   
        yield return new WaitForSeconds(secs);
        // view.RPC("RPC_trigger", RpcTarget.AllBuffered, "AngryChase");
        anim.SetBool("AngryChase", true);
        state = WoodcutterState.CHASE;
        this.photonView.RPC("RPC_UpdateLumberjack", RpcTarget.All, false);
        agent.speed = speed * 2;
    }

    public IEnumerator PauseAfterCatch(int secs)
    {   
        // view.RPC("RPC_trigger", RpcTarget.AllBuffered, "ChaseAngry");
        anim.SetBool("ChaseAngry", true);
        anim.SetBool("AngryChase", false);
        Debug.Log("Angry Trigger");
        agent.speed = 0;
        state = WoodcutterState.SEEKINGTREE;
        // disable collisions otherwise he pushes the player out of the cage
        this.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(2);
        axe.SetActive(true);
        agent.speed = speed;
        this.photonView.RPC("RPC_HideAngrySign", RpcTarget.All);
        // view.RPC("RPC_trigger", RpcTarget.AllBuffered, "AngryRun");
        anim.SetBool("AngryRun", true);
        anim.SetBool("RunAngry", false);
        yield return new WaitForSeconds(secs);
        this.GetComponent<CapsuleCollider>().enabled = true; 
    }

    // Update is called once per frame
    void Update(){
        if (PhotonNetwork.IsMasterClient)
        {
            if(pauseTime > 0f)
            {
                pauseTime -= Time.deltaTime;
                agent.speed = 0f;
                Debug.Log("woodcutter stoped");
                if (pauseTime <= 0f)
                {
                    pauseTime = 0f;
                    agent.speed = speed;
                }
            }else{
                if (!isStunned)
                {
                    dest = agent.destination;
                    // if (view.IsMine) {
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
                                    // view.RPC("RPC_trigger", RpcTarget.AllBuffered, "RunChop");
                                    anim.SetBool("RunChop", true);
                                    goto case WoodcutterState.CUTTING;
                                }
                                break;
                            
                            case WoodcutterState.CUTTING:
                                // TODO: cutting sound and animation
                                // Debug.Log("CutTrigger Active");
                                if(!isCutting) StartCoroutine(CutTree(3, treeToCut)); // secs to cut a tree
                                isCutting = true;
                                break;

                            case WoodcutterState.CHASE:
                                // TODO: sound and animation
                                // Debug.Log("chasing 1");
                                treeToCut = null;
                                // chaseAudio.enabled = true;
                                // themeAudio.enabled = false; 
                                
                                if(chasedPlayer == null) {
                                    chasedPlayer = FindClosestTarget("Player");
                                }

                                agent.destination = chasedPlayer.transform.position;
                                
                                float distance = Vector3.Distance(chasedPlayer.transform.position, transform.position);
                                PlayerMovement pm = chasedPlayer.GetComponent<PlayerMovement>();

                                // wasn't working in the latest merge, this way he captures the fox if he collides with it, doesn't need to see him that way
                                // if(distance < catchDistance && CanSee(chasedPlayer, distance) && !isStunned) {
                                //     pm.Catch();
                                //     StartCoroutine(PauseAfterCatch(3));
                                // }

                                if(distance < curiousDistance && distance > chaseDistance && !pm.captured) {
                                    agent.speed = speed;
                                    this.photonView.RPC("RPC_HideAngrySign", RpcTarget.All);
                                    state = WoodcutterState.CURIOUS;
                                    goto case WoodcutterState.CURIOUS;
                                }
                            
                                // chaseAudio.enabled = false;
                                // themeAudio.enabled = true;
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
                        // }
                }
            }
        }
    }

    private IEnumerator CutTree(int secs, GameObject tree) {
        chopping.Play();
        yield return new WaitForSeconds(secs);
        tree.tag = "Untagged";
        //tree.GetComponent<Animator>().enabled = true;
        treeAnimator = tree.GetComponent<Animator>();
        if (treeAnimator != null)
        {
            Debug.Log("Animator detected!");
            treeAnimator.enabled = true;
            treeAnimator.Play("TreeFalling");
            anim.SetBool("ChopRun", true);
            anim.SetBool("RunChop", false);
            yield return new WaitForSeconds(1);
            treeAnimator.enabled = false;

        }

        // Destroy(tree);
        //tree.transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        treeToCut = null;
        Debug.Log("cut a tree");
        if (state != WoodcutterState.CHASE)
        {
            // view.RPC("RPC_trigger", RpcTarget.AllBuffered, "ChopRun");
            anim.SetBool("ChopRun", true);
            state = WoodcutterState.SEEKINGTREE;
        }
        isCutting = false;
    }

    private IEnumerator CalmDown(int secs) {
        anim.SetBool("ChaseAngry", true);
        anim.SetBool("AngryChase", false);
        yield return new WaitForSeconds(secs);
        state = WoodcutterState.SEEKINGTREE;
        Debug.Log("Lost him!");
        calming = false;
        anim.SetBool("AngryRun", true);
        anim.SetBool("RunAngry", false);
        axe.SetActive(true);
        // TODO: points
    }

    bool CanSee(GameObject obj, float distance) { 
        float angle = Vector3.Angle(Vector3.Normalize(obj.transform.position - transform.position), transform.forward);
       
        if (Mathf.Abs(angle) < fov) {
            Debug.Log("Mathf");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.Normalize(obj.transform.position - transform.position), out hit, distance)) {
                Debug.Log("Raycast");
                if (hit.collider.transform.parent != null) {
                    Debug.Log("parent");
                    return hit.collider.transform.parent.gameObject.GetInstanceID() == obj.GetInstanceID();
                }
                return false;
            }
        }
        return false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (other.gameObject.GetComponent<PhotonView>().IsMine && !other.gameObject.GetComponent<PlayerMovement>().driving && !other.gameObject.GetComponent<PlayerMovement>().captured) {
                hud.transform.Find("InteractButton").gameObject.SetActive(true); // show button
                hud.transform.Find("InteractButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Bite"; // change text of action
                playerOutside = other.gameObject.GetComponent<PhotonView>();
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            if (other.gameObject.GetComponent<PhotonView>().IsMine) {
                hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
                playerOutside = null;
            }
        }
    }

    public void MakeAngry() {
        this.GetComponent<PhotonView>().RPC("RPC_InteractWithLumberjack", RpcTarget.MasterClient);
    }

    public void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            if (Input.GetButtonDown("Interact") && !isStunned && other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine && !other.gameObject.GetComponent<PlayerMovement>().captured) 
            {
                Debug.Log("interacted with lubmerjack");
                this.GetComponent<PhotonView>().RPC("RPC_InteractWithLumberjack", RpcTarget.MasterClient);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                if (!other.gameObject.GetComponent<PlayerMovement>().captured && state == WoodcutterState.CHASE)
                {
                    other.gameObject.GetComponent<PlayerMovement>().Catch();
                    StartCoroutine(PauseAfterCatch(2));
                }
            }
        }
    }

    [PunRPC]
    void RPC_InteractWithLumberjack()
    {
        StartCoroutine(Interact(2));
    }

    [PunRPC]
    void RPC_UpdateLumberjack(bool stun)
    {
        isStunned = stun;
    }

    [PunRPC]
    void RPC_ShowAngrySign()
    {
        angrySign.enabled = true;
    }
    [PunRPC]
    void RPC_HideAngrySign()
    {
        angrySign.enabled = false;
    }

    [PunRPC]
    void RPC_DropAxe()
    {
        Debug.Log("Axe dropped");
        axe.transform.SetParent(null);
        axe.GetComponent<Rigidbody>().isKinematic = false;
        axe.GetComponent<Rigidbody>().detectCollisions = true;
        // this.transform.position = playerView.transform.Find("Mouth").position;
        // this.transform.rotation = playerView.transform.Find("Mouth").rotation;
        axe.GetComponent<PhotonRigidbodyView>().enabled = true;
    }

    [PunRPC]
    void RPC_trigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
}
