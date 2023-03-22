using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Portal : OnTrigger
{
    [SerializeField] Transform destination;
 
    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        { 
            if (colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>() != null)
            {
                colliders.Find(x => x.GetComponent<PhotonView>().IsMine).GetComponent<PlayerMovement>().Teleport(destination.position, destination.rotation);
            }
        }
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(destination.position, 0.4f);
        var direction = destination.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(destination.position, direction);
    }
}
