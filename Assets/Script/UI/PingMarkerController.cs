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

    // Ping prefabs.
    [SerializeField] private GameObject pingMarkerGround;
    [SerializeField] private GameObject pingMarkerObject;

    // Ping audio. Can have multiple audio for different types of pings.
    [Header("Ping Audio")]
    [SerializeField] private AudioClip audioWarning;
    [SerializeField] private AudioClip audioNeutral;

    // Ground markers get placed relative to the ground.
    [PunRPC]
    void PlaceGroundMarker(Vector3 pos, float timer, string message, PhotonMessageInfo info)
    {
        Ping pingComponent = pingMarkerGround.GetComponent<Ping>();

        pingComponent.useTimer = true;
        pingComponent.timer = timer;
        pingComponent.message = message;
        pingComponent.userName = info.Sender.NickName;

        Instantiate(pingMarkerGround, pos, Quaternion.identity);
    }

    // Object markers attatches itself to the object.
    [PunRPC]
    void PlaceObjectMarker(int objectViewId, float timer, string message, PhotonMessageInfo info)
    {
        Ping pingComponent = pingMarkerObject.GetComponent<Ping>();

        pingComponent.useTimer = true;
        pingComponent.timer = timer;
        pingComponent.message = message;
        pingComponent.userName = info.Sender.NickName;

        GameObject target = PhotonView.Find(objectViewId).gameObject;
        pingComponent.target = target;

        Instantiate(pingMarkerObject, target.transform.position, Quaternion.identity);
    }
}
