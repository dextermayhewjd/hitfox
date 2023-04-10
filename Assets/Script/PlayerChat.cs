
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using TMPro;

public class PlayerChat : MonoBehaviour {
    public ChatBubbleDisplay chatbubble;
    private string oldText;
    private bool isChangingText = false;


    void Start() {
        chatbubble = GetComponentInChildren<ChatBubbleDisplay>();
        Debug.Log("chatbubble:");
        // Debug.Log(chatbubble);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (!isChangingText) {
                isChangingText = true;
                oldText = chatbubble.GetText();
                StartCoroutine(chatbubble.ChangeTextForSeconds("testing... 1 sec", 3));
            }
        }
        if (isChangingText && !chatbubble.IsChangingText) {
            chatbubble.SetText(oldText);
            isChangingText = false;
        }
    }
}