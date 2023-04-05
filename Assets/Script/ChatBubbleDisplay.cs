using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChatBubbleDisplay : MonoBehaviour
{
    public TMP_Text text;

    void Start() {
        text = GetComponent<TMP_Text>();
        text.text = "bubble";
    }

    void Update() {

    }

    public IEnumerator ChangeTextForSeconds(string newtext, int secs) {
        this.textus = newtext;
        yield return new WaitForSeconds(secs);
        this.textus = "";
    }

}
