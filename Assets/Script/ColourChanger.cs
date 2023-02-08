using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ColourChanger : MonoBehaviour, IInteractable {
 
    Material mat;
 
    private void Start() {
        mat = GetComponent<MeshRenderer>().material;
    }
 
    public void Interact() {
        mat.color = new Color(Random.value, Random.value, Random.value);
    }
}