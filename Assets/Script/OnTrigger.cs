using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnTrigger : MonoBehaviourPun
{
    public List<Collider> colliders;

    public void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player touching");
            if (!colliders.Contains(col)) colliders.Add(col);
        }
    }
 
    public void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player left");
            colliders.Remove(col);
        }
    }
}
