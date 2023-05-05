using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{   
    public GameObject Text;

    // Start is called before the first frame update
    void Start()
    {
        Text = GameObject.Find("OutOfBounds");
        if (Text != null)
        {
          Text.SetActive(false);
        }
    }

   void OnTriggerEnter()
   {
        if (Text != null)
        {
          Text.SetActive(true);
        }
   }

   void OnTriggerExit()
   {
        if (Text != null)
        {
          Text.SetActive(false);
        }
   }
}
