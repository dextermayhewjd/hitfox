using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgPLayer : MonoBehaviour
{
    public Text Errortxt; 
    void OnCollisionEnter(Collision collision){
        Debug.Log("Collision");
        Errortxt.text = "Invisible wall go back!";
    }

    void OnCollisionExit(Collision collision){
        Errortxt.text = "";
    }
}
