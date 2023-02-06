using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int multiplayerSceneIndex;//Number for the build index to the multiplay scene.

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }


    public override void OnDisable() 
    {
        PhotonNetwork.RemoveCallbackTarget(this);    
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        StartGame();

    }
    public void StartGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }

}
