using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class NetworkController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connects to photon master servers

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster(){
        Debug.Log("We are now connect to the " + PhotonNetwork.CloudRegion + " server!");
    }
}
