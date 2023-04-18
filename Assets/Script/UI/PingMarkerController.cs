using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Controls putting markers in the world space.
public class PingMarkerController : MonoBehaviour
{
    // Todo
    // - Make it so text always faces camera. Modify current billboard script or create new one so that 
    //   it is always perpendicular to the horizontal axis.
    // - Add audio.
    // - Add animations.

    // Ping prefabs.
    [SerializeField] private GameObject pingMarkerGround;
    [SerializeField] private GameObject pingMarkerObject;

    // Ping audio. Can have multiple audio for different types of pings.
    // [SerializeField] private AudioClip pingAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Ground markers get placed relative to the ground.
    public void PlaceGroundMarker(Vector3 pos, float timer, string message)
    {
        Ping pingComponent = pingMarkerGround.GetComponent<Ping>();

        pingComponent.useTimer = true;
        pingComponent.timer = timer;
        pingComponent.message = message;

        try
        {
            foreach (GameObject fox in GameObject.FindGameObjectsWithTag("Player")) {
                if(fox.GetComponent<PhotonView>().IsMine)
                {
                    pingComponent.origin = fox;
                }
            }
        }
        catch
        {

        }

        pingComponent.target = pingMarkerGround;

        try
        {
            PhotonNetwork.Instantiate(pingMarkerGround.name, pos, Quaternion.identity);
        } 
        catch
        {

        }
    }

    // Object markers attatches itself to the object.
    public void PlaceObjectMarker(GameObject targetObj, float timer, string message)
    {
        Ping pingComponent = pingMarkerObject.GetComponent<Ping>();

        pingComponent.useTimer = true;
        pingComponent.timer = timer;
        pingComponent.message = message;

        try
        {
            foreach (GameObject fox in GameObject.FindGameObjectsWithTag("Player")) {
                if(fox.GetComponent<PhotonView>().IsMine)
                {
                    pingComponent.origin = fox;
                }
            }
        }
        catch
        {

        }
        pingComponent.target = targetObj;

        try
        {
            PhotonNetwork.Instantiate(pingMarkerObject.name, targetObj.transform.position, Quaternion.identity);
        }
        catch
        {

        }
    }
}
