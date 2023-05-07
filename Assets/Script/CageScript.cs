using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CageScript : OnTrigger
{
    public int ownerId;
    public float x, y, z;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(PhotonView.Find(ownerId).gameObject.transform.position, this.transform.position) > 1f)
        {
            Vector3 pos = new Vector3(this.transform.position.x + x, this.transform.position.y + y, this.transform.position.z + z);
            PhotonView.Find(ownerId).gameObject.transform.position = pos;
        }

        if (Input.GetButtonDown("Interact")) 
        {
            Debug.Log("interacted");
            if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null &&
            !colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>().captured)
            {
                Debug.Log("destroy");
                base.photonView.RequestOwnership();
                this.photonView.RPC("RPC_Rescue", RpcTarget.AllBuffered, ownerId);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
