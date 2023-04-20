using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

// How long the ping timer will last for until it gets destroyed.
public class Ping : MonoBehaviour
{
    // Photon view component.
    private PhotonView pv;

    // Ping text fields.
    // [0] = Username
    // [1] = Message
    // [2] = Distance
    private TMP_Text[] markerTextFields;

    // How long the ping marker will last for. 0 for Infinite.
    public bool useTimer;
    public float timer;

    // Where the ping was casted from. i.e. the player.
    private GameObject origin;
    // The object if any that the ping will attatch to.
    public GameObject target;

    // For the username text field.
    public string userName;

    // For the message text field.
    public string message;

    // The scale of the ping object when within minimum distance.
    [SerializeField] private float minScale;

    // The scale of the ping object when further than max distance.
    [SerializeField] private float maxScale;

    // The distance of the player to the ping object that it's scale will be minScale.
    [SerializeField] private float minDistance;

    // The distance of the player to the ping object that it's scale will be maxScale.
    [SerializeField] private float maxDistance;

    // The original scale of the ping marker prefab.
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        // pv = GetComponent<PhotonView>();
        originalScale = this.transform.localScale;

        markerTextFields = GetComponentsInChildren<TMP_Text>();
        markerTextFields[0].text = userName; 
        markerTextFields[1].text = message; 

        foreach (GameObject fox in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(fox.GetComponent<PhotonView>().IsMine)
            {
                origin = fox;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Distance(origin.transform.position, target.transform.position);
        markerTextFields[2].text = distance.ToString() + "m";

        float scaleFactor = minScale;

        if (distance < maxDistance)
        {
            if (distance <= minDistance)
            {
                scaleFactor = minScale;
            }
            else
            {
                scaleFactor = minScale + ((maxScale - minScale) * (distance / maxDistance));
            }
        }
        else
        {
            scaleFactor = maxScale;
        }

        // Scale ping object based on player's distance to the ping. Only visible locally.
        this.transform.localScale = originalScale * scaleFactor;

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
        Destroy(this.gameObject);
    }

    private float Distance(Vector3 origin, Vector3 target)
    {
        return Mathf.Round(Vector3.Distance(origin, target));
    }
}
