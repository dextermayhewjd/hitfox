using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatTrash : MonoBehaviourPun
{
    [SerializeField] private int trashPoints;
    [SerializeField] private int axePoints;


    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Trash") && PhotonNetwork.IsMasterClient)
        {
            GameObject objectives = GameObject.Find("Timer+point");
            objectives.GetComponent<Timer>().IncreaseScore(trashPoints);

            GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
            if (pointsDisplay != null)
            {
                pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(trashPoints);
            }
            other.GetComponent<PickUpObject>().hasBeenDeleted = true;
            PhotonNetwork.Destroy(other.gameObject);
        }
        else if (other.CompareTag("Axe") && PhotonNetwork.IsMasterClient)
        {
            GameObject objectives = GameObject.Find("Timer+point");
            objectives.GetComponent<Timer>().IncreaseScore(axePoints);

            GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
            if (pointsDisplay != null)
            {
                pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(axePoints);
            }
            other.GetComponent<PickUpObject>().hasBeenDeleted = true;
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
