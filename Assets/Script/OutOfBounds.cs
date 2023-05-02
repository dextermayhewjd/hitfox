using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{   
    
    public gameObject Text;

    // Start is called before the first frame update
    void Start()
    {
        Text.SetActive(false);
    }

   void OnTriggerEnter()
   {
        Text.SetActive(true);
   }

   void OnTriggerExit()
   {
        Text.SetActive(false);
   }
}
