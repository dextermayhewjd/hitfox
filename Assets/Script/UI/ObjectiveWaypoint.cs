using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ObjectiveWaypoint : MonoBehaviour
{
    [Header("Main Canvas")]
    [SerializeField] private GameObject canvas;

    [Header("Default Waypoint Marker Prefabs")]
    [SerializeField] private GameObject defaultWaypointMarker;

    [System.Serializable]
    private class ObjectiveWaypointInfo
    {
        public string objectiveId;
        [TextArea] public string header;
        [TextArea] public string subHeader;
        [SerializeField] public GameObject waypointMarker;
    }

    [SerializeField] private ObjectiveWaypointInfo[] objectiveWaypointInfoList;
    private Dictionary<string, ObjectiveWaypointInfo> objectiveWaypointInfoTable;

    // Start is called before the first frame update
    void Start()
    {
        if (canvas == null)
        {
            canvas = GameObject.FindGameObjectWithTag("UICanvas");
        }        

        objectiveWaypointInfoTable = new Dictionary<string, ObjectiveWaypointInfo>();

        foreach (var objectiveWaypointInfo in objectiveWaypointInfoList)
        {
            this.objectiveWaypointInfoTable[objectiveWaypointInfo.objectiveId] = objectiveWaypointInfo;
        }
    }

    [PunRPC]
    void ObjectiveWaypointMarker(int objectiveViewId, Vector3 pos, string objectiveId)
    {
        ObjectiveWaypointInfo objectiveWaypointInfo;

        if (!objectiveWaypointInfoTable.TryGetValue(objectiveId, out objectiveWaypointInfo))
        {
            return;
        }

        GameObject waypointMarkerToUse;

        if (objectiveWaypointInfo.waypointMarker == null)
        {
            waypointMarkerToUse = defaultWaypointMarker;
        }
        else
        {
            waypointMarkerToUse = objectiveWaypointInfo.waypointMarker;
        }

        GameObject waypointMarker = Instantiate(waypointMarkerToUse, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
        Waypoint waypoint = waypointMarker.GetComponent<Waypoint>();

        waypoint.targetPos = pos;
        waypoint.header = objectiveWaypointInfo.header;
        waypoint.subHeader = objectiveWaypointInfo.subHeader;
        waypoint.targetObject = PhotonView.Find(objectiveViewId).gameObject;
        waypoint.trackObject = true;
    }
}
