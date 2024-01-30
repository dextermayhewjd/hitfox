using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatTrash : MonoBehaviourPun
{
    [SerializeField] private int trashPoints;
    [SerializeField] private int axePoints;
    public AudioSource trashNoise;


    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Trash") && PhotonNetwork.IsMasterClient)
        {
            trashNoise.Play();
            GameObject objectives = GameObject.Find("Timer+point");
            objectives.GetComponent<Timer>().IncreaseScore(trashPoints);

            GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
            if (pointsDisplay != null)
            {
                pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(trashPoints);
            }
            PhotonNetwork.Destroy(other.gameObject);
        }
        else if (other.CompareTag("Axe") && PhotonNetwork.IsMasterClient)
        {
            trashNoise.Play();
            GameObject objectives = GameObject.Find("Timer+point");
            objectives.GetComponent<Timer>().IncreaseScore(axePoints);

            GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
            if (pointsDisplay != null)
            {
                pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(axePoints);
            }
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
