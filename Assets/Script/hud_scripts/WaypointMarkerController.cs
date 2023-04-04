using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

// Controls putting markers in the world space.
public class WaypointMarkerController : MonoBehaviour
{
    // Todo
    // - Make it so text always faces camera.
    // - Add audio.
    // - Add animations.

    [SerializeField] private GameObject pingMarkerGround;
    [SerializeField] private GameObject pingMarkerObject;
    [SerializeField] private GameObject waypointMarker;
    // [SerializeField] private AudioClip pingAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceGroundMarker(Vector3 pos, string userName, string message)
    {
        TMP_Text[] markerTextFields;
        markerTextFields = pingMarkerGround.GetComponentsInChildren<TMP_Text>();

        markerTextFields[0].text = userName;
        markerTextFields[1].text = message;

        // Switch to work with photon.
        Instantiate(pingMarkerGround, pos, Quaternion.identity);
    }

    public void PlaceObjectMarker()
    {

    }

    public void PlaceObjectiveMarker()
    {

    }
}
