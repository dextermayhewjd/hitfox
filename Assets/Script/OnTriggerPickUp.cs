using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class OnTriggerPickUp : MonoBehaviourPun
{
    public List<Collider> objectsToPickUp;
    public GameObject hud;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PickUpObject>() != null)
        {
            hud.transform.Find("InteractButton").gameObject.SetActive(true); // show button
            hud.transform.Find("InteractButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Pick Up"; // change text of action
            if (!objectsToPickUp.Contains(other)) objectsToPickUp.Add(other);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PickUpObject>() != null)
        {
            hud.transform.Find("InteractButton").gameObject.SetActive(true); // show button
            hud.transform.Find("InteractButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Pick Up"; // change text of action
            if (!objectsToPickUp.Contains(other)) objectsToPickUp.Add(other);
        }
    }
 
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PickUpObject>() != null)
        {
            hud.transform.Find("InteractButton").gameObject.SetActive(false); // hide button
            objectsToPickUp.Remove(other);
        }
    }
}
