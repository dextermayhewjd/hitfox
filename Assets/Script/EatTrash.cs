using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatTrash : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Trash") && PhotonNetwork.IsMasterClient)
        {
            GameObject objectives = GameObject.Find("Timer+point");
            Debug.Log("5 points for collecting trash");
            objectives.GetComponent<Timer>().IncreaseScore(5);

            GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
            if (pointsDisplay != null)
            {
                pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(5);
            }
            other.GetComponent<PickUpObject>().hasBeenDeleted = true;
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
