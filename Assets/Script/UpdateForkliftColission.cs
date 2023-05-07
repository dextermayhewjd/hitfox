using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateForkliftColission : MonoBehaviour
{
    public Transform forklift;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = forklift.position;
        this.transform.rotation = forklift.rotation;
    }
}
