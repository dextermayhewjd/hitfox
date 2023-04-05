using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UsernameDisPlay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    public TMP_Text text;

    void Start()
    {
        playerPV = GetComponent<PhotonView>();
        text = GetComponentsInChildren<TMP_Text>()[0];
        Debug.Log(text);
        text.text = "username";
    }
}