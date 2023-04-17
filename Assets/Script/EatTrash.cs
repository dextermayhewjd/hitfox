using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EatTrash : MonoBehaviourPun
{
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Trash"))
        {
            PhotonNetwork.Destroy(other.gameObject);
        }
    }
}
