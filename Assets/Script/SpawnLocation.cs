using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public Vector3 minPoint;
    public Vector3 maxPoint;

    void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        minPoint = renderer.bounds.min;
        maxPoint = renderer.bounds.max;
        renderer.enabled = false;
    }
}
