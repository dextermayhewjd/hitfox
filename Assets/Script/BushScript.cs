using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class BushScript : OnTrigger
{
    public bool inUse = false;
    public PhotonView playerInBush = null;
    public Canvas inUseSign;

    private void Start() {
        inUseSign.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (inUse && playerInBush.IsMine)
            {
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = playerInBush.transform;
                cam.Follow = playerInBush.transform;
                this.photonView.RPC("RPC_PlaySound", RpcTarget.AllBuffered);
                this.photonView.RPC("RPC_UnhidePlayer", RpcTarget.AllBuffered);
                this.photonView.RPC("RPC_HideSign", RpcTarget.OthersBuffered);
                
            }

            else if (!inUse && colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null && 
                !colliders.Find(x => x.GetComponent<PlayerMovement>().driving)) 
            {
                this.photonView.RPC("RPC_PlaySound", RpcTarget.AllBuffered);
                this.photonView.RPC("RPC_HidePlayer", RpcTarget.AllBuffered, colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PhotonView>().ViewID);
                this.photonView.RPC("RPC_ShowSign", RpcTarget.OthersBuffered);
                if (playerInBush.IsMine) 
                {
                    CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                    cam.LookAt = transform;
                    cam.Follow = transform;
                }
            }
        }
    }

    [PunRPC]
    void RPC_UnhidePlayer()
    {
        Debug.Log("Player left hiding spot");
        playerInBush.transform.SetParent(null);
        playerInBush.gameObject.SetActive(true);
        playerInBush.gameObject.GetComponent<PlayerMovement>().hidden = false;
        colliders.RemoveAll(item => item.gameObject.GetComponent<PhotonView>().ViewID == playerInBush.ViewID);
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
