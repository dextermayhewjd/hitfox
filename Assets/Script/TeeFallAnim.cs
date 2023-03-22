using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeeFallAnim : MonoBehaviour
{
    Animator anim;
    public bool isMoving = false;


    public static TeeFallAnim TreeOn;
    // Start is called before the first frame update
/*    void Start()
    {
        
    }*/
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (TreeOn == null)
        {
            TreeOn = this;
            anim.enabled = isMoving;

        }
    }

    // Update is called once per frame
/*    void Update()
    {
        anim.enabled = isMoving;
    }
*/}
