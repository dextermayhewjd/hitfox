using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Portal : OnTrigger
{
    [SerializeField] Transform destination;
 
<<<<<<< HEAD
    private void Update() {}
    // {
    //     if (Input.GetButtonDown("Interact"))
    //     { 
    //         if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>() != null)
    //         {
    //             colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>().Teleport(destination.position, destination.rotation);
    //         }
    //     }
    // }
=======
    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        { 
            if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine) != null)
            {
                colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>().Teleport(destination.position, Quaternion.identity);
            }
        }
    }
>>>>>>> 20b4b3a93b88d5cf1b37fb2a3cf2d5d50d46b1b4

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(destination.position, 0.4f);
        var direction = destination.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(destination.position, direction);
    }
}
