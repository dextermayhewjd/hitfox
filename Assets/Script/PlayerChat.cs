using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerChat : MonoBehaviour {
    public ChatBubbleDisplay chatbubble;
    void Start() {
        chatbubble = GetComponent<ChatBubbleDisplay>();
    }

    void Update() {
        if(Input.GetKey("1")) {
            chatbubble.ChangeTextForSeconds("testing... 1 sec", 1);
        }
    }
}