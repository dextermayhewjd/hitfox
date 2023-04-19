using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireOnTrigger : MonoBehaviourPun
{
    public List<Collider> colliders;

    public void OnTriggerEnter(Collider col)
    {
        Vector3 pos = GetComponent<Transform>().position;

        if (col.CompareTag("Player"))
        {
            Debug.Log("Player touching fire");
            if (!colliders.Contains(col)) colliders.Add(col);

            Vector3 direction = (pos - (col.gameObject.GetComponent<Transform>().position));

            GameObject cgo = col.gameObject;
            Debug.Log("cgo");
            Debug.Log(cgo);
            Debug.Log(cgo.GetComponentsInChildren<ImpactReceiver>());
            ImpactReceiver cgoir = cgo.GetComponentInChildren<ImpactReceiver>();
            Debug.Log("cgoir");
            Debug.Log(cgoir);
            cgoir.AddImpact(-direction+(0.5f*Vector3.up), 50.0f);

        }
    }
    // d
 
    public void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player left fire");
            colliders.Remove(col);
        }
    }
}
