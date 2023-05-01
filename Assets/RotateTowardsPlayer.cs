using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// This script is used to rotate objects towards the closest player
public class RotateTowardsPlayer : MonoBehaviour
{

    GameObject FindClosestTarget(string trgt) {
        GameObject closestGameObject = GameObject.FindGameObjectsWithTag(trgt)
                        .OrderBy(go => Vector3.Distance(go.transform.position, transform.position))
                        .FirstOrDefault();
            return closestGameObject;
    }

    void Update()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = new Vector3((FindClosestTarget("Player").transform.position - transform.position).x, 0, (FindClosestTarget("Player").transform.position - transform.position).z);

        // The step size is equal to speed times frame time.
        float singleStep = 1.0f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
