using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// How long the ping timer will last for until it gets destroyed.
public class PingTimer : MonoBehaviour
{
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
        
    }
}
