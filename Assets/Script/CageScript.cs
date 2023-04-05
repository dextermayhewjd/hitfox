using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageScript : OnTrigger
{

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count != 0 && Input.GetButtonDown("Interact"))
        {
            Destroy(gameObject);
        }
    }
}
