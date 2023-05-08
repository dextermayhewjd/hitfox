using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CageScript : MonoBehaviourPun
{
    public int ownerId;
    public float x, y, z;

    public GameObject hud;
    public PhotonView playerOutside;
    public float holdTime;

    [PunRPC]
    void RPC_Rescue(int playerID)
    {
        Debug.Log("Rescued");
        PhotonView player = PhotonView.Find(playerID);
        player.GetComponent<PlayerMovement>().captured = false;
        player.GetComponent<CharacterController>().detectCollisions = true;
        player.gameObject.transform.Find("Collider").gameObject.SetActive(false);
    }

    void Start() {
        x = 0;
        y = 0.3f;
        z = 0;
        playerOutside = null;
        holdTime = 0;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine && !other.gameObject.GetComponent<PlayerMovement>().driving) {
            hud.transform.Find("InteractButton").gameObject.SetActive(true); // show button
            hud.transform.Find("InteractButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Rescue"; // change text of action
            playerOutside = other.gameObject.GetComponent<PhotonView>();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine) {
            hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
            playerOutside = null;
            holdTime = 0f;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine &&
        !other.gameObject.GetComponent<PlayerMovement>().captured && !other.gameObject.GetComponent<PlayerMovement>().driving) 
        {
            Debug.Log("interacted");
            if (Input.GetButton("Interact"))
            {
                holdTime += Time.deltaTime;
                if (holdTime >= 3f) {
                    Debug.Log("destroy");
                    base.photonView.RequestOwnership();
                    this.photonView.RPC("RPC_Rescue", RpcTarget.All, ownerId);
                    PhotonNetwork.Destroy(gameObject);
                }
            } else {
                holdTime = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(PhotonView.Find(ownerId).gameObject.transform.position, this.transform.position) > 1f)
        {
            Vector3 pos = new Vector3(this.transform.position.x + x, this.transform.position.y + y, this.transform.position.z + z);
            PhotonView.Find(ownerId).gameObject.transform.position = pos;
        }
    }
}
