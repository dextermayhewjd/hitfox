using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] private bool triggerActive = false;
    private Collider collider;

    public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent<PlayerMovement>(out var player))
            {
                triggerActive = true;
                collider = other;
            }
        }
 
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerMovement>(out var player))
        {
            triggerActive = false;
            collider = null;
        }
    }
 
    private void Update()
    {
        if (triggerActive && Input.GetButton("Interact"))
        {
            if (collider.TryGetComponent<PlayerMovement>(out var player))
            {
                player.Teleport(destination.position + new Vector3(0, 0, 3), destination.rotation);
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
