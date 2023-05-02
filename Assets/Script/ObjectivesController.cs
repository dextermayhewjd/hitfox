using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectivesController : MonoBehaviour
{
    private PhotonView pv;

    [SerializeField] private GameObject pointsControllerObject;
    [SerializeField] private GameObject objectiveObject;

    [Header("Objective Alert Display")]
    [SerializeField] private ObjectivesAlert objectivesAlert;

    [System.Serializable]
    private class ObjectiveInfo
    {
        [SerializeField] public string objectiveId;
        [SerializeField] public GameObject[] objectsToSpawn;
        [SerializeField] public SpawnLocation[] spawnLocations;
    }

    [System.Serializable]
    private class StartingObjects
    {
        [SerializeField] public string id;
        [SerializeField] public int numObjectsToSpawn;
        [SerializeField] public GameObject objectToSpawn;
        [SerializeField] public SpawnLocation[] spawnLocations;
    }

    private class ObjectiveSetup
    {
        public string objectiveId;
        public string locationId;
        public int numObjectsToSpawn;

        public ObjectiveSetup(string objectiveId, int numObjectsToSpawn = 1)
        {
            this.objectiveId = objectiveId;
            this.numObjectsToSpawn = numObjectsToSpawn;
        }

        public ObjectiveSetup(string objectiveId, string locationId, int numObjectsToSpawn = 1)
        {
            this.objectiveId = objectiveId;
            this.locationId = locationId;
            this.numObjectsToSpawn = numObjectsToSpawn;
        }
    }

    [Header("Objective Event Parameters")]
    [SerializeField] private bool dynamicObjectives;
    [SerializeField] private float startDelay;
    [SerializeField] private float maxActiveObjectives;
    [SerializeField] private float objectiveRate;

    [Header("Objective Info")]
    [SerializeField] private ObjectiveInfo[] objectiveInfoList;

    [Header("Objects To Spawn At The Start")]
    [SerializeField] private StartingObjects[] startingObjectsList;

    private Dictionary<string, ObjectiveInfo> objectiveInfoTable;

    private float timeSinceObjectiveStart;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        pv = GetComponent<PhotonView>();

        if (objectivesAlert == null)
        {
            objectivesAlert = GameObject.Find("ObjectivesAlertDisplay").GetComponent<ObjectivesAlert>();
        }

        objectiveInfoTable = new Dictionary<string, ObjectiveInfo>();

        foreach (var objectiveInfo in objectiveInfoList)
        {
            this.objectiveInfoTable[objectiveInfo.objectiveId] = objectiveInfo;
        }

        timeSinceObjectiveStart = Time.time + startDelay - objectiveRate;
        InstantiateStartingObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // Development Purposes.
        // =====
        // Fire Objective
        if (Input.GetKeyDown(KeyCode.G))
        {
            FireObjective();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            TrashObjective();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            HedgedogTaxiObjective();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            LumberjackObjective();
        }

        // =====

        // UpdateObjectiveRates();
        if (dynamicObjectives)
        {
            HandleObjectiveEvents();
        }
    }

    [PunRPC]
    public void UpdateTimeSinceObjectiveStart(float time)
    {
        this.timeSinceObjectiveStart = time;
    }

    private void InstantiateStartingObjects()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (startingObjectsList.Length == 0)
        {
            return;
        }

        foreach (var startingObject in startingObjectsList)
        {
            if (startingObject.spawnLocations.Length > 0)
            {
                int j = 0;
                for (int i = 0; i < startingObject.numObjectsToSpawn; i++)
                {
                    if (j >= startingObject.spawnLocations.Length)
                    {
                        j = 0;
                    }
                    PhotonNetwork.InstantiateRoomObject(startingObject.objectToSpawn.name, startingObject.spawnLocations[j].GetRandomPoint(), Quaternion.identity);
                    j++;
                }
            }
        }
    }

    private void HandleObjectiveEvents()
    {
        if (Time.time - timeSinceObjectiveStart <= objectiveRate)
        {
            return;
        }

        pv.RPC("UpdateTimeSinceObjectiveStart", RpcTarget.All, Time.time);

        ObjectiveSetup objectiveSetup = DecideObjective();

        if (objectiveSetup == null)
        {
            return;
        }

        StartObjective(objectiveSetup.objectiveId, objectiveSetup.numObjectsToSpawn, objectiveSetup.locationId);
    }

    private ObjectiveSetup DecideObjective()
    {
        int numActiveObjectives = 0;
        int numFireObjectives = 0;
        int numTrashObjectives = 0;

        foreach (var objectiveObject in GameObject.FindGameObjectsWithTag("Objective"))
        {
            Objective objective = objectiveObject.GetComponent<Objective>();
            switch (objective.objectiveId)
            {
                case "fire":
                    numFireObjectives++;
                    break;
                case "trash":
                    numTrashObjectives++;
                    break;
            }

            numActiveObjectives++;
        }

        if (numActiveObjectives >= maxActiveObjectives)
        {
            return null;
        }

        if (numActiveObjectives == 0)
        {
            int randNum = Random.Range(0, 2);
            switch (randNum)
            {
                case 0:
                    return new ObjectiveSetup("fire");
                    break;
                case 1:
                    return new ObjectiveSetup("trash", 5);
                    break;
            }
        }

        if (numFireObjectives == 0)
        {
            return new ObjectiveSetup("fire");
        }

        if (numTrashObjectives == 0)
        {
            return new ObjectiveSetup("trash", 5);
        }

        if (numActiveObjectives < maxActiveObjectives)
        {
            int randNum = Random.Range(0, 3);
            switch (randNum)
            {
                case 0:
                    return new ObjectiveSetup("fire");
                    break;
                case 1:
                    return new ObjectiveSetup("trash", 5);
                    break;
            }
        }

        return null;
    }

    private void StartObjective(string objectiveId, int numObjectsToSpawn, string locationId = "")
    {
        pv.RPC("UpdateTimeSinceObjectiveStart", RpcTarget.All, Time.time);
        switch(objectiveId)
        {
            case "fire":
                FireObjective();
                break;
            case "trash":
                TrashObjective(numObjectsToSpawn);
                break;
            case "hedgedogTaxi":
                HedgedogTaxiObjective();
                break;
            case "lumberjack":
                LumberjackObjective();
                break;
        }
    }

    public void TrashObjective(int numTrash = 5, string locationId = "")
    {
        string objectiveId = "trash";

        ObjectiveInfo objective = GetObjective(objectiveId);

        if (objective == null)
        {
            return;
        }
        
        List<GameObject> objectsToSpawn = new List<GameObject>();
        for (int i = 0; i < numTrash; i++)
        {
            GameObject objectToSpawn = objective.objectsToSpawn[Random.Range(0, objective.objectsToSpawn.Length)];
            objectsToSpawn.Add(objectToSpawn);
            
        }

        SpawnLocation spawnLocationArea = objective.spawnLocations[Random.Range(0, objective.spawnLocations.Length)];

        if (locationId != "")
        {
            foreach (var spawnLocation in objective.spawnLocations)
            {
                if (spawnLocation.id == locationId)
                {
                    spawnLocationArea = spawnLocation;
                }
            }
        }

        List<int> spawnedObjectsId = new List<int>();

        foreach (var objectToSpawn in objectsToSpawn)
        {
            Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
            GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, spawnLocation, Quaternion.identity);
            spawnedObjectsId.Add(spawnedObject.GetComponent<PhotonView>().ViewID);
        }

        // Spawn an Objective to track that objective.
        object[] instanceData = new object[3];
        instanceData[0] = objective.objectiveId;
        instanceData[1] = spawnedObjectsId.ToArray();
        instanceData[2] = spawnLocationArea.description;

        GameObject spawnedObjective = PhotonNetwork.InstantiateRoomObject(objectiveObject.name, spawnLocationArea.centre, Quaternion.identity, 0, instanceData);
        int objectiveViewId = spawnedObjective.GetComponent<PhotonView>().ViewID;

        // Display An Alert Of The Objective.
        pv.RPC("AddObjectiveAlert", RpcTarget.All, objective.objectiveId, spawnLocationArea.description);

        // Add Waypoint Marker To Objective.
        Vector3 waypointMarkerLocation = spawnLocationArea.centre;
        waypointMarkerLocation.y += 5f;
        pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, objectiveViewId, waypointMarkerLocation, objective.objectiveId);
    }

    public void FireObjective(string locationId = "")
    {
        string objectiveId = "fire";

        ObjectiveInfo objective = GetObjective(objectiveId);

        if (objective == null)
        {
            return;
        }

        SpawnLocation spawnLocationArea = objective.spawnLocations[Random.Range(0, objective.spawnLocations.Length)];

        if (locationId != "")
        {
            foreach (var spawnLocation in objective.spawnLocations)
            {
                if (spawnLocation.id == locationId)
                {
                    spawnLocationArea = spawnLocation;
                }
            }
        }

        GameObject objectToSpawn = objective.objectsToSpawn[0];

        List<int> spawnedObjectsId = new List<int>();

        Vector3 location = spawnLocationArea.GetRandomPoint();
        GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, location, Quaternion.identity);
        int spawnedObjectviewID = spawnedObject.GetComponent<PhotonView>().ViewID;
        spawnedObjectsId.Add(spawnedObjectviewID);

        // Spawn an Objective to track that objective.
        object[] instanceData = new object[3];
        instanceData[0] = objective.objectiveId;
        instanceData[1] = spawnedObjectsId.ToArray();
        instanceData[2] = spawnLocationArea.description;

        GameObject spawnedObjective = PhotonNetwork.InstantiateRoomObject(objectiveObject.name, spawnLocationArea.centre, Quaternion.identity, 0, instanceData);
        int objectiveViewId = spawnedObjective.GetComponent<PhotonView>().ViewID;

        // Display An Alert Of The Objective.
        pv.RPC("AddObjectiveAlert", RpcTarget.All, objective.objectiveId, spawnLocationArea.description);

        // Add Waypoint Marker To Objective.
        Vector3 waypointMarkerLocation = spawnLocationArea.centre;
        waypointMarkerLocation.y += 5f;
        pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, objectiveViewId, waypointMarkerLocation, objective.objectiveId);
    }

    public void HedgedogTaxiObjective(int numHedgehog = 2, string locationId = "")
    {
        string objectiveId = "hedgedogTaxi";

        ObjectiveInfo objective = GetObjective(objectiveId);

        if (objective == null)
        {
            return;
        }

        GameObject objectToSpawn = objective.objectsToSpawn[0];

        List<int> spawnedObjectsId = new List<int>();

        for (int i = 0; i < numHedgehog; i++)
        {
            SpawnLocation spawnLocationArea = objective.spawnLocations[Random.Range(0, objective.spawnLocations.Length)];
            Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
            GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, spawnLocation, Quaternion.identity);
            int spawnedObjectviewID = spawnedObject.GetComponent<PhotonView>().ViewID;
            spawnedObjectsId.Add(spawnedObjectviewID);
            Vector3 waypointMarkerLocation = spawnLocation;
            waypointMarkerLocation.y += 0;
            pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, spawnedObjectviewID, waypointMarkerLocation, "hedgedog");
        }

        // Spawn an Objective to track that objective.
        object[] instanceData = new object[3];
        instanceData[0] = objective.objectiveId;
        instanceData[1] = spawnedObjectsId.ToArray();
        instanceData[2] = "";

        GameObject spawnedObjective = PhotonNetwork.InstantiateRoomObject(objectiveObject.name, new Vector3(0, 0, 0), Quaternion.identity, 0, instanceData);
        int objectiveViewId = spawnedObjective.GetComponent<PhotonView>().ViewID;

        // Display An Alert Of The Objective.
        pv.RPC("AddObjectiveAlert", RpcTarget.All, objective.objectiveId, "");

        foreach (var go in GameObject.FindGameObjectsWithTag("HedgehogHome"))
        {
            Vector3 waypointMarkerLocation = go.transform.position;
            pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, objectiveViewId, waypointMarkerLocation, objective.objectiveId);
        }
    }

    public void LumberjackObjective(string location = "")
    {
    }

    public void FoxCaptured(int viewID)
    {
        // Objective capturedFoxObjective;

        // if (!objectives.TryGetValue("FoxCaptured", out capturedFoxObjective))
        // {
        //     Debug.Log("Fox Captured Objective not Set");
        //     return;
        // }

        // activeObjectives.Add(new Objective(capturedFoxObjective.id, Time.time, capturedFox, capturedFox.transform.position));

        // Add Marker at captured fox transform position of add to quest.
    }

    private ObjectiveInfo GetObjective(string objectiveId)
    {
        ObjectiveInfo objective;

        if (!objectiveInfoTable.TryGetValue(objectiveId, out objective))
        {
            Debug.Log("Objective Of ID(" + objectiveId + ") Not Set");
            return null;
        }

        if (!AssertObjective(objective))
        {
            return null;
        }

        return objective;
    }

    private bool AssertObjective(ObjectiveInfo objective)
    {
        if (objective.objectsToSpawn.Length == 0)
        {
            Debug.Log("No Objects To Spawn");
            return false;
        }
        
        if (objective.spawnLocations.Length == 0)
        {
            Debug.Log("Objective Spawn Locations Not Set");
            return false;
        }

        return true;
    }

    [PunRPC]
    public void AddObjectiveAlert(string objectiveId, string location)
    {
        objectivesAlert.AddObjectiveAlertToBuffer(objectiveId, location);
    }

    [PunRPC]
    public void AddToQuest(string id, string location, int numItems)
    {

    }
}
