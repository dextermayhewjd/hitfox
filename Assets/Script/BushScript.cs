using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using TMPro;

public class BushScript : MonoBehaviourPun
{
    public bool inUse = false;
    public PhotonView playerInBush = null;
    public Canvas inUseSign;
    public GameObject hud;
    public PhotonView playerOutside = null;

    void Start() {
        inUseSign.enabled = false;
        hud = GameObject.Find("HUD");
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine && !inUse && !other.gameObject.GetComponent<PlayerMovement>().driving) {
            hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
            hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Hide"; // change text of action
            playerOutside = other.gameObject.GetComponent<PhotonView>();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine) {
            hud.transform.Find("EnterButton").gameObject.SetActive(false); // hide button
            playerOutside = null;
        }
    }

    void Update() {
        if (inUse && playerInBush.IsMine)
        {
            hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
            hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Stop Hiding"; // change text of action
            if (Input.GetButtonDown("Enter")) {
                hud.transform.Find("EnterButton").gameObject.SetActive(false); // show button
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = playerInBush.transform;
                cam.Follow = playerInBush.transform;
                this.photonView.RPC("RPC_PlaySound", RpcTarget.All);
                this.photonView.RPC("RPC_UnhidePlayer", RpcTarget.All);
                this.photonView.RPC("RPC_HideSign", RpcTarget.Others);
            }
            
        }

        else if (!inUse && playerOutside.IsMine) 
        {
            hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
            hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Hide"; // change text of action
            if (Input.GetButtonDown("Enter")) {
                this.photonView.RPC("RPC_PlaySound", RpcTarget.All);
                this.photonView.RPC("RPC_HidePlayer", RpcTarget.All, playerOutside.ViewID);
                this.photonView.RPC("RPC_ShowSign", RpcTarget.Others);
                if (playerInBush.IsMine) 
                {
                    CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                    cam.LookAt = transform;
                    cam.Follow = transform;
                }
            }
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (colliders.Count > 0) {
    //         colliders.RemoveAll(item => item == null);
    //     } 
    //     if (inUse && playerInBush.IsMine)
    //     {
    //         hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
    //         hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Stop Hiding"; // change text of action
    //         if (Input.GetButtonDown("Enter")) {
    //             hud.transform.Find("EnterButton").gameObject.SetActive(false); // show button
    //             CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
    //             cam.LookAt = playerInBush.transform;
    //             cam.Follow = playerInBush.transform;
    //             this.photonView.RPC("RPC_PlaySound", RpcTarget.AllBuffered);
    //             this.photonView.RPC("RPC_UnhidePlayer", RpcTarget.AllBuffered);
    //             this.photonView.RPC("RPC_HideSign", RpcTarget.OthersBuffered);
    //         }
            
    //     }

    //     else if (!inUse && colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null && 
    //         !colliders.Find(x => x.GetComponent<PlayerMovement>().driving)) 
    //     {
    //         hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
    //         hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Hide"; // change text of action
    //         if (Input.GetButtonDown("Enter")) {
    //             this.photonView.RPC("RPC_PlaySound", RpcTarget.AllBuffered);
    //             this.photonView.RPC("RPC_HidePlayer", RpcTarget.AllBuffered, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
    //             this.photonView.RPC("RPC_ShowSign", RpcTarget.OthersBuffered);
    //             if (playerInBush.IsMine) 
    //             {
    //                 CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
    //                 cam.LookAt = transform;
    //                 cam.Follow = transform;
    //             }
    //         }
    //     } else if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) == null) {
    //         hud.transform.Find("EnterButton").gameObject.SetActive(false); // hide button
    //     }
    // }

    [PunRPC]
    void RPC_UnhidePlayer()
    {
        Debug.Log("Player left hiding spot");
        playerInBush.transform.SetParent(null);
        playerInBush.gameObject.SetActive(true);
        playerInBush.gameObject.GetComponent<PlayerMovement>().hidden = false;
        // colliders.RemoveAll(item => item.gameObject.GetComponent<PhotonView>().ViewID == playerInBush.ViewID);
        playerInBush = null;
        inUse = false;
    }

    [PunRPC]
    void RPC_HidePlayer(int player)
    {
        Debug.Log("Player hidden");
        inUse = true;
        playerInBush = PhotonView.Find(player);
        playerInBush.transform.SetParent(this.transform);
        playerInBush.gameObject.SetActive(false);
        playerInBush.gameObject.GetComponent<PlayerMovement>().hidden = true;
    }

    [PunRPC]
    void RPC_ShowSign()
    {
        inUseSign.enabled = true;
    }
    [PunRPC]
    void RPC_HideSign()
    {
        inUseSign.enabled = false;
    }

    [PunRPC]
    void RPC_PlaySound()
    {
        this.GetComponent<AudioSource>().Play();
    }
}
