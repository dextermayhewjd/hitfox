using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ObjectiveId
{
    None,
    Fire,
    Trash,
    HedgedogTaxi,
    Lumberjack,
}

public enum StartingObjectId
{
    None,
    Bucket,
    Forklift,
}

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
        [SerializeField] public ObjectiveId objectiveId;
        [SerializeField] public GameObject[] objectsToSpawn;
        [SerializeField] public SpawnLocation[] spawnLocations;
        [SerializeField] public int maxActiveObjectives;
    }

    [System.Serializable]
    private class StartingObjects
    {
        [SerializeField] public StartingObjectId id;
        [SerializeField] public int numObjectsToSpawn;
        [SerializeField] public GameObject objectToSpawn;
        [SerializeField] public SpawnLocation[] spawnLocations;
    }

    private class ObjectiveSetup
    {
        public ObjectiveId objectiveId;
        public int spawnLocationIndex;
        public int numObjectsToSpawn;

        public ObjectiveSetup(ObjectiveId objectiveId, int spawnLocationIndex = -1)
        {
            this.objectiveId = objectiveId;
            this.numObjectsToSpawn = DecideNumObjectsToSpawn();
            this.spawnLocationIndex = spawnLocationIndex;
        }

        private int DecideNumObjectsToSpawn()
        {
            int numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

            switch(this.objectiveId)
            {
                case ObjectiveId.Trash:
                    return ObjectsToSpawnMultipler(5, numPlayers);
                    break;
                case ObjectiveId.HedgedogTaxi:
                    return ObjectsToSpawnMultipler(2, numPlayers);
            }

            return 1;
        }

        private int ObjectsToSpawnMultipler(int initialNumber, int numPlayers)
        {
            return (int)Mathf.Round((float)initialNumber * (1f + (Mathf.Log((float)numPlayers) / 2)));
        }
    }

    private class ActiveObjectiveInfo
    {
        public ObjectiveId objectiveId;
        public int numActiveObjectives;
        public List<int> usedSpawnLocationIndex;

        public ActiveObjectiveInfo(ObjectiveId objectiveId)
        {
            this.objectiveId = objectiveId;
            this.numActiveObjectives = 0;
            this.usedSpawnLocationIndex = new List<int>();
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

    private Dictionary<ObjectiveId, ObjectiveInfo> objectiveInfoTable;

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

        objectiveInfoTable = new Dictionary<ObjectiveId, ObjectiveInfo>();

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

        StartObjective(objectiveSetup.objectiveId, objectiveSetup.numObjectsToSpawn, objectiveSetup.spawnLocationIndex);
    }

    private ObjectiveSetup DecideObjective()
    {
        int numActiveObjectives = 0;
        Dictionary<ObjectiveId, ActiveObjectiveInfo> activeObjectivesInfo = new Dictionary<ObjectiveId, ActiveObjectiveInfo>();

        foreach (ObjectiveId objectiveId in objectiveInfoTable.Keys)
        {
            activeObjectivesInfo[objectiveId] = new ActiveObjectiveInfo(objectiveId);
        }

        foreach (var objectiveObject in GameObject.FindGameObjectsWithTag("Objective"))
        {
            Objective objective = objectiveObject.GetComponent<Objective>();

            ObjectiveId objectiveId = objective.objectiveId;

            if (activeObjectivesInfo.ContainsKey(objectiveId))
            {
                activeObjectivesInfo[objectiveId].numActiveObjectives++;
                activeObjectivesInfo[objectiveId].usedSpawnLocationIndex.Add(objective.spawnLocationIndex);
            }

            numActiveObjectives++;
        }

        if (numActiveObjectives >= maxActiveObjectives)
        {
            return null;
        }
        else
        {
            ObjectiveId objectiveToStart =  ObjectiveRandomizer(activeObjectivesInfo);

            if (objectiveToStart == ObjectiveId.None)
            {
                return null;
            }
            else
            {
                return new ObjectiveSetup(objectiveToStart, DecideSpawnLocation(objectiveToStart, activeObjectivesInfo[objectiveToStart].usedSpawnLocationIndex));
            }
        }
    }

    private ObjectiveId ObjectiveRandomizer(Dictionary<ObjectiveId, ActiveObjectiveInfo> activeObjectivesInfo)
    {
        List<ObjectiveId> availableObjectives = new List<ObjectiveId>();

        foreach(KeyValuePair<ObjectiveId, ActiveObjectiveInfo> activeObjectiveInfo in activeObjectivesInfo)
        {
            if (activeObjectiveInfo.Value.numActiveObjectives < objectiveInfoTable[activeObjectiveInfo.Key].maxActiveObjectives)
            {
                availableObjectives.Add(activeObjectiveInfo.Key);
            }
        }

        if (availableObjectives.Count == 0)
        {
            return ObjectiveId.None;
        }

        return availableObjectives[Random.Range(0, availableObjectives.Count)];
    }

    private int DecideSpawnLocation(ObjectiveId objectiveId, List<int> usedSpawnLocationIndex)
    {
        ObjectiveInfo objectiveInfo = GetObjective(objectiveId);

        if (usedSpawnLocationIndex.Count == 0)
        {
            return Random.Range(0, objectiveInfo.spawnLocations.Length);
        }

        List<int> unusedSpawnLocationIndex = new List<int>();

        for (int i = 0; i < objectiveInfo.spawnLocations.Length; i++)
        {
            if (!usedSpawnLocationIndex.Contains(i))
            {
                unusedSpawnLocationIndex.Add(i);
            }
        }

        if (unusedSpawnLocationIndex.Count == 0)
        {
            return -1;
        }
        else
        {
            return unusedSpawnLocationIndex[Random.Range(0, unusedSpawnLocationIndex.Count)];
        }
    }

    private void StartObjective(ObjectiveId objectiveId, int numObjectsToSpawn, int spawnLocationIndex = -1)
    {
        pv.RPC("UpdateTimeSinceObjectiveStart", RpcTarget.All, Time.time);
        switch(objectiveId)
        {
            case ObjectiveId.Fire:
                FireObjective(spawnLocationIndex);
                break;
            case ObjectiveId.Trash:
                TrashObjective(numObjectsToSpawn, spawnLocationIndex);
                break;
            case ObjectiveId.HedgedogTaxi:
                HedgedogTaxiObjective(numObjectsToSpawn, spawnLocationIndex);
                break;
            case ObjectiveId.Lumberjack:
                LumberjackObjective(numObjectsToSpawn, spawnLocationIndex);
                break;
        }
    }

    public void TrashObjective(int numTrash = 5, int spawnLocationIndex = -1)
    {
        ObjectiveId objectiveId = ObjectiveId.Trash;

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

        if (spawnLocationIndex == -1)
        {
            spawnLocationIndex = Random.Range(0, objective.spawnLocations.Length);
        }

        SpawnLocation spawnLocationArea = objective.spawnLocations[spawnLocationIndex];

        List<int> spawnedObjectsId = new List<int>();

        foreach (var objectToSpawn in objectsToSpawn)
        {
            Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
            GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, spawnLocation, Quaternion.identity);
            spawnedObjectsId.Add(spawnedObject.GetComponent<PhotonView>().ViewID);
        }

        Vector3 waypointMarkerLocation = spawnLocationArea.centre;
        waypointMarkerLocation.y += 5f;

        // Spawn an Objective to track that objective.
        object[] instanceData = new object[6];
        instanceData[0] = objective.objectiveId;
        instanceData[1] = spawnedObjectsId.ToArray();
        instanceData[2] = spawnLocationArea.areaId;
        instanceData[3] = spawnLocationIndex;
        instanceData[4] = ObjectiveWaypointId.Trash;
        instanceData[5] = waypointMarkerLocation;

        GameObject spawnedObjective = PhotonNetwork.InstantiateRoomObject(objectiveObject.name, spawnLocationArea.centre, Quaternion.identity, 0, instanceData);
        int objectiveViewId = spawnedObjective.GetComponent<PhotonView>().ViewID;

        // Display An Alert Of The Objective.
        pv.RPC("AddObjectiveAlert", RpcTarget.All, objective.objectiveId, spawnLocationArea.description);

        // Add Waypoint Marker To Objective.
        pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, objectiveViewId, waypointMarkerLocation, ObjectiveWaypointId.Trash, true);
    }

    public void FireObjective(int spawnLocationIndex = -1)
    {
        ObjectiveId objectiveId = ObjectiveId.Fire;

        ObjectiveInfo objective = GetObjective(objectiveId);

        if (objective == null)
        {
            return;
        }

        if (spawnLocationIndex == -1)
        {
            spawnLocationIndex = Random.Range(0, objective.spawnLocations.Length);
        }

        SpawnLocation spawnLocationArea = objective.spawnLocations[spawnLocationIndex];

        GameObject objectToSpawn = objective.objectsToSpawn[0];

        List<int> spawnedObjectsId = new List<int>();

        Vector3 location = spawnLocationArea.GetRandomPoint();
        GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, location, Quaternion.identity);
        int spawnedObjectviewID = spawnedObject.GetComponent<PhotonView>().ViewID;
        spawnedObjectsId.Add(spawnedObjectviewID);

        Vector3 waypointMarkerLocation = spawnLocationArea.centre;
        waypointMarkerLocation.y += 5f;

        // Spawn an Objective to track that objective.
        object[] instanceData = new object[6];
        instanceData[0] = objective.objectiveId;
        instanceData[1] = spawnedObjectsId.ToArray();
        instanceData[2] = spawnLocationArea.areaId;
        instanceData[3] = spawnLocationIndex;
        instanceData[4] = ObjectiveWaypointId.Fire;
        instanceData[5] = waypointMarkerLocation;

        GameObject spawnedObjective = PhotonNetwork.InstantiateRoomObject(objectiveObject.name, spawnLocationArea.centre, Quaternion.identity, 0, instanceData);
        int objectiveViewId = spawnedObjective.GetComponent<PhotonView>().ViewID;

        // Display An Alert Of The Objective.
        pv.RPC("AddObjectiveAlert", RpcTarget.All, objective.objectiveId, spawnLocationArea.description);

        // Add Waypoint Marker To Objective.
        pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, objectiveViewId, waypointMarkerLocation, ObjectiveWaypointId.Fire, true);
    }

    public void HedgedogTaxiObjective(int numHedgehog = 2, int spawnLocationIndex = -1)
    {
        ObjectiveId objectiveId = ObjectiveId.HedgedogTaxi;

        ObjectiveInfo objective = GetObjective(objectiveId);

        if (objective == null)
        {
            return;
        }

        GameObject objectToSpawn = objective.objectsToSpawn[0];

        List<int> spawnedObjectsId = new List<int>();

        for (int i = 0; i < numHedgehog; i++)
        {
            if (spawnLocationIndex == -1)
            {
                spawnLocationIndex = Random.Range(0, objective.spawnLocations.Length);
            }

            SpawnLocation spawnLocationArea = objective.spawnLocations[spawnLocationIndex];
            Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
            GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, spawnLocation, Quaternion.identity);
            int spawnedObjectviewID = spawnedObject.GetComponent<PhotonView>().ViewID;
            spawnedObjectsId.Add(spawnedObjectviewID);
            Vector3 waypointMarkerLocation = spawnedObject.transform.position;
            waypointMarkerLocation.y += 0f;
            pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, spawnedObjectviewID, waypointMarkerLocation, ObjectiveWaypointId.Hedgedog, false);
        }

        // Spawn an Objective to track that objective.
        object[] instanceData = new object[6];
        instanceData[0] = objective.objectiveId;
        instanceData[1] = spawnedObjectsId.ToArray();
        instanceData[2] = AreaId.None;
        instanceData[3] = 0;
        instanceData[4] = ObjectiveWaypointId.None;
        instanceData[5] = new Vector3(0, 0, 0);

        GameObject spawnedObjective = PhotonNetwork.InstantiateRoomObject(objectiveObject.name, new Vector3(0, 0, 0), Quaternion.identity, 0, instanceData);
        int objectiveViewId = spawnedObjective.GetComponent<PhotonView>().ViewID;

        // Display An Alert Of The Objective.
        pv.RPC("AddObjectiveAlert", RpcTarget.All, objective.objectiveId, "");

        foreach (var go in GameObject.FindGameObjectsWithTag("HedgehogHome"))
        {
            Vector3 waypointMarkerLocation = go.transform.position;
            pv.RPC("ObjectiveWaypointMarker", RpcTarget.All, objectiveViewId, waypointMarkerLocation, ObjectiveWaypointId.HedgedogHome, true);
        }
    }

    public void LumberjackObjective(int numLumberjacks = 1, int spawnLocationIndex = -1)
    {
        ObjectiveId objectiveId = ObjectiveId.Lumberjack;

        ObjectiveInfo objective = GetObjective(objectiveId);

        if (objective == null)
        {
            return;
        }

        GameObject objectToSpawn = objective.objectsToSpawn[0];
        SpawnLocation spawnLocationArea = objective.spawnLocations[Random.Range(0, objective.spawnLocations.Length)];
        Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
        GameObject spawnedObject = PhotonNetwork.InstantiateRoomObject(objectToSpawn.name, spawnLocation, Quaternion.identity);
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

    private ObjectiveInfo GetObjective(ObjectiveId objectiveId)
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
    public void AddObjectiveAlert(ObjectiveId objectiveId, string location)
    {
        objectivesAlert.AddObjectiveAlertToBuffer(objectiveId, location);
    }

    [PunRPC]
    public void AddToQuest(string id, string location, int numItems)
    {

    }
}
