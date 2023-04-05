using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ChatBubbleDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public string textus;

    void Start() {
        text = GetComponent<TMP_Text>();
        textus = "";
        text.text = textus;
    }

    void Update() {
        // text.text = textus;
    }

    public IEnumerator ChangeTextForSeconds(string newtext, int secs) {
        this.textus = newtext;
        yield return new WaitForSeconds(secs);
        this.textus = "";
    }

}
