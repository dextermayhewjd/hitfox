using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
//using System.Diagnostics;
//new version
public class Launcher : MonoBehaviourPunCallbacks
{
		[SerializeField] TMP_InputField roomNameInputField;
		[SerializeField] TMP_Text errorText;
		[SerializeField] TMP_Text roomNameText;
		// [SerializeField] Transform roomListContent;
		// [SerializeField] GameObject roomListItemPrefab;
		// [SerializeField] Transform playerListContent;
		// [SerializeField] GameObject PlayerListItemPrefab;
		// [SerializeField] GameObject startGameButton;		
    void Start()
    {
        UnityEngine.Debug.Log("Connnected to Master");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        UnityEngine.Debug.Log("Connnected to Master");
        PhotonNetwork.JoinLobby();
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

		// Player[] players = PhotonNetwork.PlayerList;

		// foreach(Transform child in playerListContent)
		// {
		// 	Destroy(child.gameObject);
		// }

		// for(int i = 0; i < players.Count(); i++)
		// {
		// 	Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		// }

		// startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}	
    
    
		public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
	  Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
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

}