using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeeFallAnim : MonoBehaviour
{
    Animator anim;
    public bool isMoving;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        anim.enabled = true;
    }
}
