using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallSave : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 25f || transform.position.y > 100f) {
            transform.position = new Vector3(transform.position.x, 65f, transform.position.z);
        }
    }
}
