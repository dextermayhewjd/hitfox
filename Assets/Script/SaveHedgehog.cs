using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// To be used for the destination (HHHome prefab) of where the hedgehog should be dropped off
// 
public class SaveHedgehog : MonoBehaviourPun
{
    public GameObject hedgehog;
    float timer = 0f;

    [SerializeField] private int points;

    void Start() {
        hedgehog.SetActive(false);
        GetComponent<MeshRenderer>().enabled = false;
    }

    void Update() {
        if (hedgehog.activeSelf) {
            timer += Time.deltaTime;
            if (timer >= 10f) {
                hedgehog.SetActive(false);
                timer = 0f;
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Forklift") && other.gameObject.GetComponent<Taxi>().hedgehog.activeSelf) {
            other.gameObject.GetComponent<Taxi>().drop();

            if (PhotonNetwork.IsMasterClient)
            {
                GameObject objectives = GameObject.Find("Timer+point");
                objectives.GetComponent<Timer>().IncreaseScore(points);

                GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
                if (pointsDisplay != null)
                {
                    pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(points);
                }

                GetComponent<PhotonView>().RPC("RPC_SaveHedgehog", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_SaveHedgehog() {
        hedgehog.SetActive(true);
        timer = 0f;
    }
}
