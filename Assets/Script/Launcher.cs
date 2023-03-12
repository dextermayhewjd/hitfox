using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
		public static Launcher Instance;

		[SerializeField] TMP_InputField roomNameInputField;
		[SerializeField] TMP_Text errorText;
		[SerializeField] TMP_Text roomNameText;
		[SerializeField] Transform roomListContent;
		[SerializeField] Transform playerListContent;
		[SerializeField] GameObject roomListItemPrefab; 
		[SerializeField] GameObject PlayerListItemPrefab;
		[SerializeField] GameObject startGameButton;		

		
		void Awake()
		{
			Instance = this;
		}


    void Start()
    {
        UnityEngine.Debug.Log("Connnected to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        UnityEngine.Debug.Log("Connnected to Master");
        PhotonNetwork.JoinLobby();
				PhotonNetwork.AutomaticallySyncScene = true;
				//When all players sync scenes
    }
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title"); // maybe
        UnityEngine.Debug.Log("Joined Lobby");
				// later would be replaced with user name system
    }
    
	public void CreateRoom()
	{
		if(string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}
	
	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		 Player[] players = PhotonNetwork.PlayerList;

		foreach(Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for(int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}	

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}
	// if the host of the game leaves
    
    
		public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
	  Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}
	 public void StartGame()
	{
		PhotonNetwork.LoadLevel(1); 
		// 1 is the build index of our game scene
		// as we set in the build settings
	}

   public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}
		public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("title");
	}
	
	public override void OnRoomListUpdate(List<RoomInfo> roomList) // get from the roomlist
	{
		foreach(Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}
	//destory all the current button and add them into current rooms
		for(int i = 0; i < roomList.Count; i++)
		{
			if(roomList[i].RemovedFromList)
				continue;
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		}
	}
	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");


	}
	
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}
}