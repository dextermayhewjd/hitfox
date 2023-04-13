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
    // - Test to see if GameObject.Find("FoxPlayer") only returns local fox player.

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
    public void PlaceGroundMarker(Vector3 pos, float timer, string userName, string message)
    {
        Ping pingComponent = pingMarkerGround.GetComponent<Ping>();
        pingComponent.timer = timer;
        pingComponent.userName = userName;
        pingComponent.message = message;
        pingComponent.player = GameObject.Find("FoxPlayer");
        pingComponent.target = pingMarkerGround;

        // Switch to work with photon.
        Instantiate(pingMarkerGround, pos, Quaternion.identity);
        // PhotonNetwork.Instantiate(pingMarkerGround, pos, Quaternion.identity);
    }

    // Object markers attatches itself to the object.
    public void PlaceObjectMarker(GameObject targetObj, float timer, string userName, string message)
    {
        Ping pingComponent = pingMarkerObject.GetComponent<Ping>();
        pingComponent.timer = timer;
        pingComponent.userName = userName;
        pingComponent.message = message;
        pingComponent.player = GameObject.Find("FoxPlayer");
        pingComponent.target = targetObj;

        // Switch to work with photon.
        Instantiate(pingMarkerObject, targetObj.transform.position, Quaternion.identity);
        // PhotonNetwork.Instantiate(pingMarkerGround, pos, Quaternion.identity);
    }
}
