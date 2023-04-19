using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OnTriggerPickUp : MonoBehaviourPun
{
    public List<Collider> objectsToPickUp;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PickUpObject>() != null)
        {
            Debug.Log("Can pick up");
            if (!objectsToPickUp.Contains(other)) objectsToPickUp.Add(other);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PickUpObject>() != null)
        {
            Debug.Log("Can pick up");
            if (!objectsToPickUp.Contains(other)) objectsToPickUp.Add(other);
        }
    }
 
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PickUpObject>() != null)
        {
            Debug.Log("Too far to pick up");
            objectsToPickUp.Remove(other);
        }
    }
}
