using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsPopupTimer : MonoBehaviour
{
    [SerializeField] private float timer;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
