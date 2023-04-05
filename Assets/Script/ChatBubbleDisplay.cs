using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChatBubbleDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] private TMP_Text text;


    void Start() {
        text = GetComponentsInChildren<TMP_Text>()[0]; // IF YOU REMOVE THE USELESS UsName AND ChatBubble GAMEOBJECTS FROM FOXPLAYER THEN CHANGE THIS
        playerPV = GetComponent<PhotonView>();
        text.text = "chat";
    }

    void Update() {

    }

    public IEnumerator ChangeTextForSeconds(string newtext, int secs) {
        Debug.Log("ctfs");
        Debug.Log(newtext);
        Debug.Log("^newtext");
        TMP_Text oldtext = text;
        text.text = newtext;
        yield return new WaitForSeconds(secs);
        text = oldtext;
    }

}
