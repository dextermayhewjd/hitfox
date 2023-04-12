using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System;
using System.Linq;

public class Fire : MonoBehaviour {
    void Start() {}
    void Update() {}

    void OnCollisionEnter(Collision collision) {
        GameObject go = collision.gameObject;
        Vector3 direction = collision.relativeVelocity.normalized;
        float magnitude = collision.relativeVelocity.magnitude; 

        go.GetComponent<ImpactReceiver>().AddImpact(-direction, magnitude+5.0f); // knocks back foxes
    }
}