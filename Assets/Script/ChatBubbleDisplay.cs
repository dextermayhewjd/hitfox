using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChatBubbleDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    // [SerializeField] private TMP_Text text;
    [SerializeField] TMP_Text text;
    public bool IsChangingText;

    void Start() {
        // text = GetComponentsInChildren<TMP_Text>()[0]; // IF YOU REMOVE THE USELESS UsName AND ChatBubble GAMEOBJECTS FROM FOXPLAYER THEN CHANGE THIS
        // playerPV = GetComponent<PhotonView>();
        text.text = "chat";
    }

    void Update() {

    }

    public string GetText(){
        return text.text;
    }

    public void SetText(string newText){
        text.text = newText;
    }

    public IEnumerator ChangeTextForSeconds(string newtext, float secs) {
        IsChangingText = true;
        Debug.Log("Changing text to: " + newtext);
        string oldtext = text.text;
        Debug.Log("Saving old text: " + oldtext);
        text.text = newtext;
        yield return new WaitForSeconds(secs);
        Debug.Log("Restoring old text: " + oldtext);
        text.text = oldtext;
        IsChangingText = false;
        yield break;
        // somehow it's not working here but solve the problem in playerchat instead
    }

}
