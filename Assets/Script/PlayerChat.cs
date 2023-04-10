using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerChat : MonoBehaviour {
    public ChatBubbleDisplay chatbubble;
    void Start() {
        chatbubble = GetComponentInChildren<ChatBubbleDisplay>();
        Debug.Log("chatbubble:");
        Debug.Log(chatbubble);
    }

    void Update() {
        if(Input.GetKey("1")) {
            Debug.Log("1 hehe");
            chatbubble.ChangeTextForSeconds("testing... 1 sec", 1);
        }
    }
}