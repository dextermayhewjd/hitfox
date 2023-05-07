using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Taxi : MonoBehaviourPun
{
    public bool hedgehogSitting;
    public GameObject hedgehog;

    private void Start() {
        hedgehogSitting = false;
        hedgehog.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hedgehog")) {
            if (!hedgehog.activeSelf && GetComponent<NewCarUserControl>().driving) {
                this.photonView.RPC("RPC_PickUpHedgehog", RpcTarget.AllBuffered);
                PhotonNetwork.Destroy(other.gameObject);
            }
        }
    }

    public void drop() {
        this.photonView.RPC("RPC_DropHedgehog", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_PickUpHedgehog() {
        hedgehog.SetActive(true);
    }

    [PunRPC]
    void RPC_DropHedgehog() {
        hedgehog.SetActive(false);
    }
}
