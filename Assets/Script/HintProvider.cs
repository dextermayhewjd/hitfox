using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HintProvider : MonoBehaviour
{
    //public GameObject bucket;
    public Text HintBox;
    public GameObject bucket;
    //FF: include other objects that will give players hints 
    //public GameObject fire;
 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == bucket)
        {
            HintBox.text = "You can use bucket by pressing q";
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        HintBox.text = "";
    }
}
