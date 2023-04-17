using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

// How long the ping timer will last for until it gets destroyed.
public class Ping : MonoBehaviour
{
    // TODO
    // - Add photon view component.

    private TMP_Text[] markerTextFields;

    // How long the ping marker will last for. 0 for Infinite.
    public bool useTimer;
    public float timer;

    // Where the ping was casted from. i.e. the player.
    public GameObject origin;
    // The object if any that the ping will attatch to.
    public GameObject target;

    // For the username text field.
    public string userName;

    // For the message text field.
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
        markerTextFields[2].text = Distance(origin.transform.position, target.transform.position).ToString() + "m";

        if (target != null)
        {
            this.transform.position = target.transform.position;
            // To use if the object has been interacted with such as if it was picked up for example.
            // Need to set a property in the object for this.
            // if (target.property)
            // {
            //     StopPing;
            // }
        }

        if (useTimer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                StopPing();
            }
        }
    }
        
    private void StopPing()
    {
        // PhotonNetwork.Destroy(this.gameObject);
        Destroy(this.gameObject);
    }

    private float Distance(Vector3 origin, Vector3 target)
    {
        return Mathf.Round(Vector3.Distance(origin, target));
    }
}
