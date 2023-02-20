using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
 
public class ColourChanger : MonoBehaviourPun, IInteractable {
 
    Material mat;
 
    private void Start() {
        mat = GetComponent<MeshRenderer>().material;
    }
 
    public void Interact() {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        this.photonView.RPC("RPC_ColourChange", RpcTarget.All, r,g,b);
    }

    [PunRPC]
    void RPC_ColourChange(float r, float g, float b)
    {
        mat.color = new Color(r, g, b, 1f);
    }
}