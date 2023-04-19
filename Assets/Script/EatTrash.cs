using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatTrash : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Trash"))
        {
            if (other.GetComponent<PickUpObject>().addPoints == true)
            {

                GameObject objectives = GameObject.Find("ObjectivesTracker");
                Debug.Log("5 points for collecting trash");
                objectives.GetComponent<ObjectivesScript>().IncreaseScore(5);
                other.GetComponent<PickUpObject>().addPoints = false;
                // other.gameObject.SetActive(false);
            }
        }
    }
}
