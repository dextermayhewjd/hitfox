using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocation : MonoBehaviour
{
    public string name;

    public Vector3 centre;
    public Vector3 minPoint;
    public Vector3 maxPoint;
    public Vector3 extents;
    public Vector3 size;

    void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        centre = renderer.bounds.center;
        minPoint = renderer.bounds.min;
        maxPoint = renderer.bounds.max;
        extents = renderer.bounds.extents;
        size = renderer.bounds.size;
        renderer.enabled = false;
    }

    // Find way to work with rotation of object whilst ignoring scale factor.
    // Does not really work as intended when object is rotated.
    public Vector3 GetRandomPoint()
    {
        Vector3 point = new Vector3(
            Random.Range( minPoint.x, maxPoint.x ),
            Random.Range( minPoint.y, maxPoint.y ),
            Random.Range( minPoint.z, maxPoint.z )
        );
 
        return point;
    }
}
