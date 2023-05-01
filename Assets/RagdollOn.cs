using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.AI;


public class RagdollOn : MonoBehaviour
{
    public BoxCollider mainCollider;
    public GameObject StudentRig;
    public Animator StudentAnimator;
    Stopwatch stopwatch = new Stopwatch();
    public NavMeshAgent agent;
    public GameObject target;

    void Start()
    {
        GetRagdollBits();
        RagDollModeOff();
        //check tag == Player
    }
    void Update()
    {
        Vector3 characterPosition = transform.position;
        Vector3 targetPosition = target.transform.position;

        //Ray ray = new Ray(characterPosition, direction);
        //RaycastHit hit; //then if hit then ...
        agent.SetDestination(targetPosition);
    }
    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("COllIDED");
        if (collision.gameObject.tag == "Player")
        {
            stopwatch.Start();
            UnityEngine.Debug.Log("Player" + collision.gameObject.tag);
            RagDollModeOn();
            stopwatch.Stop();
            UnityEngine.Debug.Log("Fallen"+ stopwatch.ElapsedTicks / (double)Stopwatch.Frequency);
        }
        //add another if here for if collided with a  tree PutUpPoster(){}

    }
 /*   private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision stop");
        RagDollModeOff();
    }*/

    void RagDollModeOn(){
        UnityEngine.Debug.Log("Ragdoll on");
        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = false;
        }
        //StudentAnimator.enabled = false;
        mainCollider.enabled = false;
        StudentRig.GetComponent<Rigidbody>().isKinematic = true;
    }

    void RagDollModeOff(){
        UnityEngine.Debug.Log("Ragdoll off");
        foreach(Collider col in ragdollColliders)
        {
            col.enabled = false;
        }
        foreach (Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = true;
        }
        StudentAnimator.enabled = true;
        mainCollider.enabled = true;
        StudentRig.GetComponent<Rigidbody>().isKinematic = false;
    }

    Collider[] ragdollColliders;
    Rigidbody[] limbsRigidbodies;
    void GetRagdollBits() {
        UnityEngine.Debug.Log("Ragdoll bits got");
        ragdollColliders = StudentRig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = StudentRig.GetComponentsInChildren<Rigidbody>();
    }

}
