using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum ObjectiveWaypointId
{
    None,
    Fire,
    Trash,
    HedgedogHome,
    Hedgedog,
    Lumberjack,
}

public class ObjectiveWaypoint : MonoBehaviour
{
    [Header("Main Canvas")]
    [SerializeField] private GameObject canvas;

    [Header("Default Waypoint Marker Prefabs")]
    [SerializeField] private GameObject defaultWaypointMarker;

    [System.Serializable]
    private class ObjectiveWaypointInfo
    {
        public ObjectiveWaypointId objectiveWaypointId;
        [TextArea] public string header;
        [TextArea] public string subHeader;
        [SerializeField] public GameObject waypointMarker;
    }

    [SerializeField] private ObjectiveWaypointInfo[] objectiveWaypointInfoList;
    private Dictionary<ObjectiveWaypointId, ObjectiveWaypointInfo> objectiveWaypointInfoTable;

    // Start is called before the first frame update
    void Start()
    {
        if (canvas == null)
        {
            canvas = GameObject.FindGameObjectWithTag("UICanvas");
        }        

        objectiveWaypointInfoTable = new Dictionary<ObjectiveWaypointId, ObjectiveWaypointInfo>();

        foreach (var objectiveWaypointInfo in objectiveWaypointInfoList)
        {
            this.objectiveWaypointInfoTable[objectiveWaypointInfo.objectiveWaypointId] = objectiveWaypointInfo;
        }

        foreach (var objectiveObj in GameObject.FindGameObjectsWithTag("Objective"))
        {
            Objective objective = objectiveObj.GetComponent<Objective>();

            if (objective.objectiveId == ObjectiveId.HedgedogTaxi)
            {
                foreach (var go in GameObject.FindGameObjectsWithTag("HedgehogHome"))
                {
                    Vector3 waypointMarkerLocation = go.transform.position;
                    ObjectiveWaypointMarker(objective.viewId,  go.transform.position, ObjectiveWaypointId.HedgedogHome, true);
                }

                foreach (var hedgedogId in objective.spawnedObjectsId)
                {
                    Vector3 hedgedogPos = PhotonView.Find(hedgedogId).gameObject.transform.position;
                    hedgedogPos.y += 1f;
                    ObjectiveWaypointMarker(hedgedogId, hedgedogPos, ObjectiveWaypointId.Hedgedog, false);
                }
            }
            else
            {
                ObjectiveWaypointMarker(objective.viewId, objective.objectiveWaypointPos, objective.objectiveWaypointId, true);
            }
        }
    }

    [PunRPC]
    void ObjectiveWaypointMarker(int objectiveViewId, Vector3 pos, ObjectiveWaypointId objectiveWaypointId, bool trackDistance)
    {
        ObjectiveWaypointInfo objectiveWaypointInfo;

        if (!objectiveWaypointInfoTable.TryGetValue(objectiveWaypointId, out objectiveWaypointInfo))
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
        waypoint.trackDistance = trackDistance;
    }
}
