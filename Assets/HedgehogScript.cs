using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class HedgehogScript : OnTrigger
{
    public Canvas canvas;
    public TextMeshProUGUI text;

    void Start() {
        text.text = "Can you drive me back home please?";
        canvas.enabled = false;
    }

    void Update() {
        if (colliders.Count > 0) {
            canvas.enabled = true;
        } else {
            canvas.enabled = false;
        }
    }
}
