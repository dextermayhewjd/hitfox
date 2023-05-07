using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Controls putting markers in the world space.
public class PingMarkerController : MonoBehaviour
{
    // Todo
    //  - Modify current billboard script or create new one so that 
    //   it is always perpendicular to the horizontal axis.
    // - Add audio.
    // - Add animations.

    [Header("Main Canvas")]
    [SerializeField] private GameObject canvas;

    [Header("Waypoint Marker Prefabs")]
    [SerializeField] private GameObject waypointMarkerGround;
    [SerializeField] private GameObject waypointMarkerObject;

    [Header("Ping Audio")]
    [SerializeField] private AudioClip audioWarning;
    [SerializeField] private AudioClip audioNeutral;

    // Start is called before the first frame update
    void Start()
    {
        if (canvas == null)
        {
            canvas = GameObject.FindGameObjectWithTag("UICanvas");
        }
    }

    // Ground markers get placed relative to the ground.
    [PunRPC]
    void PingGroundMarker(Vector3 pos, float timer, string message, PhotonMessageInfo info)
    {
        if (this.waypointMarkerGround == null)
        {
            return;
        }
        GameObject waypointMarker = Instantiate(this.waypointMarkerGround, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
        Waypoint waypoint = waypointMarker.GetComponent<Waypoint>();

        waypoint.useTimer = true;
        waypoint.timer = timer;

        waypoint.targetPos = pos;

        waypoint.header = info.Sender.NickName;
        waypoint.subHeader = message;

    }

    // Object markers attatches itself to the object.
    [PunRPC]
    void PingObjectMarker(int objectViewId, float timer, string message, PhotonMessageInfo info)
    {
        if (this.waypointMarkerObject == null)
        {
            return;
        }
        GameObject waypointMarker = Instantiate(this.waypointMarkerObject, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
        Waypoint waypoint = waypointMarker.GetComponent<Waypoint>();

        waypoint.useTimer = true;
        waypoint.timer = timer;

        waypoint.header = info.Sender.NickName;
        waypoint.subHeader = message;

        GameObject target = PhotonView.Find(objectViewId).gameObject;
        waypoint.trackObject = true;
        waypoint.trackObjectPos = true;
        waypoint.targetObject = target;

    }
}
