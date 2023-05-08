using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

// this script shifts the position of HUD buttons depedning on which ones are active
public class HUDScript : MonoBehaviourPun
{
    public Transform InteractButton; // should always be on bottom of screen
    public Transform ThrowButton; // should always be above the InteractButton if the player is holding something
    public Transform EnterButton; // should always be above the 2 upper mentioned button, depending on if they are active
    public Vector3 originalEnterButtonPosition;
    public TextMeshProUGUI ping;

    // Start is called before the first frame update
    void Start()
    {
        InteractButton = transform.Find("InteractButton");
        ThrowButton = transform.Find("ThrowButton");
        EnterButton = transform.Find("EnterButton");
        originalEnterButtonPosition = EnterButton.position;
    }

    // Update is called once per frame
    void Update()
    {
        ping.text = PhotonNetwork.GetPing().ToString();
        if (InteractButton.gameObject.activeSelf) {
            if (ThrowButton.gameObject.activeSelf) {
                EnterButton.position = originalEnterButtonPosition;
            } else {
                EnterButton.position = ThrowButton.position;
            }
        } else {
            EnterButton.position = InteractButton.position;
        }
    }
}
