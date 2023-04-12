using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

// Controls putting markers in the world space.
public class PingMarkerController : MonoBehaviour
{
    // Todo
    // - Make it so text always faces camera. Modify current billboard script or create new one so that 
    //   it is always perpendicular to the horizontal axis.
    // - Add audio.
    // - Add animations.
    // - Dynamically calculate distance between a player and the ping so distance is different from other players
    //   viewpoints.

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
        pingMarkerGround.GetComponent<PingTimer>().timer = timer;

        TMP_Text[] markerTextFields;
        markerTextFields = pingMarkerGround.GetComponentsInChildren<TMP_Text>();

        markerTextFields[0].text = userName;
        markerTextFields[1].text = message;
        markerTextFields[2].text = "20m";

        // Switch to work with photon.
        Instantiate(pingMarkerGround, pos, Quaternion.identity);
        // PhotonNetwork.Instantiate(pingMarkerGround, pos, Quaternion.identity);
    }

    // Object markers attatches itself to the object.
    public void PlaceObjectMarker()
    {

    }
}
