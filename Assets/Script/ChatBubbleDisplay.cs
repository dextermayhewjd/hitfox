using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChatBubbleDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text text;


    void Start() {
    }

    void Update() {
    }

    public IEnumerator ChangeTextForSeconds(string newtext, int secs) {
        text.text = newtext;
        yield return new WaitForSeconds(secs);
        text.text = "";
    }

}
