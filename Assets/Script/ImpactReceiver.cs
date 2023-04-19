using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System;
using System.Linq;

class ImpactReceiver : MonoBehaviour {
    public float mass;
    public Vector3 impact;
    public CharacterController characterController;

    void Start() {
        Debug.Log("In ImpactReceiver Start().");
        Debug.Log(characterController);
        mass = 3.0f;
        impact = Vector3.zero;
        characterController = GetComponent<CharacterController>();
    }

    public void AddImpact(Vector3 dir, float force) {
        Debug.Log("adding impact");
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        dir.Normalize();
        impact += dir * (force / mass);
    }

    void Update() {
        if (impact.magnitude > 0.2f) characterController.Move(impact * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
    }
}