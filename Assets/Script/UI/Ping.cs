using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// How long the ping timer will last for until it gets destroyed.
public class Ping : MonoBehaviour
{
    private TMP_Text[] markerTextFields;

    public float timer;
    public GameObject player;
    public GameObject target;

    public string userName;
    public string message;

    // Start is called before the first frame update
    void Start()
    {
        markerTextFields = GetComponentsInChildren<TMP_Text>();
        markerTextFields[0].text = userName; 
        markerTextFields[1].text = message; 
    }

    // Update is called once per frame
    void Update()
    {
        markerTextFields[2].text = Distance(player.transform.position, target.transform.position).ToString() + "m";

        if (target != null)
        {
            this.transform.position = target.transform.position;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                StopPing();
            }
        }
    }
        
    public void StopPing()
    {
        Destroy(this.gameObject);
    }

    private float Distance(Vector3 origin, Vector3 target)
    {
        return Mathf.Round(Vector3.Distance(origin, target));
    }
}
