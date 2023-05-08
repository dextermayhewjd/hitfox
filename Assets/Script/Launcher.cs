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

		/* Add the roomListItems dictionary here*/
    private Dictionary<string, RoomListItem> roomListItems = new Dictionary<string, RoomListItem>();
		private Dictionary<int, PlayerListItem> playerListItems = new Dictionary<int, PlayerListItem>();
		/* to improve efficiency*/
		
		

		void Awake()
		{
			Instance = this;
		}


    void Start()
    {
        PhotonNetwork.SendRate = 40; /// added
		PhotonNetwork.SerializationRate = 40;//
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
		// UPDATED
		playerListItems.Clear();
		// for(int i = 0; i < players.Count(); i++)
		// {
		// 	// Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		// }
		 foreach (Player player in players)
        {
            AddPlayerListItem(player);
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
	
	// public override void OnRoomListUpdate(List<RoomInfo> roomList) // get from the roomlist
	// {
	// 	foreach(Transform trans in roomListContent)
	// 	{
	// 		Destroy(trans.gameObject);
	// 	}
	// //destory all the current button and add them into current rooms
	// 	for(int i = 0; i < roomList.Count; i++)
	// 	{
	// 		if(roomList[i].RemovedFromList)
	// 			continue;
	// 		Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
	// 	}
	// }

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
{
    foreach (RoomInfo roomInfo in roomList)
    {
        if (roomInfo.RemovedFromList)
        {
            if (roomListItems.ContainsKey(roomInfo.Name))
            {
                Destroy(roomListItems[roomInfo.Name].gameObject);
                roomListItems.Remove(roomInfo.Name);
            }
        }
        else
        {
            if (!roomListItems.ContainsKey(roomInfo.Name))
            {
                RoomListItem roomListItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
                roomListItem.SetUp(roomInfo);
                roomListItems.Add(roomInfo.Name, roomListItem);
            }
            else
            {
                roomListItems[roomInfo.Name].SetUp(roomInfo);
            }
        }
    }
}


	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");


	}
	
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
			 AddPlayerListItem(newPlayer);
		// Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}

		private void AddPlayerListItem(Player player)
	{
			if (!playerListItems.ContainsKey(player.ActorNumber))
			{
					PlayerListItem playerListItem = Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>();
					playerListItem.SetUp(player);
					playerListItems.Add(player.ActorNumber, playerListItem);
			}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
{
    if (playerListItems.ContainsKey(otherPlayer.ActorNumber))
    {
        Destroy(playerListItems[otherPlayer.ActorNumber].gameObject);
        playerListItems.Remove(otherPlayer.ActorNumber);
    }
}	

public void ExitTheGame()
{
	Application.Quit();
}



}

