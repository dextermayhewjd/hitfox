using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CageScript : OnTrigger
{

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count != 0 && Input.GetButtonDown("Interact") &&
        colliders.Find(x => x.GetComponent<PlayerMovement>().captured))
        {
            Destroy(gameObject);
        }
    }
}
