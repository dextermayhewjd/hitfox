using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectivesController : MonoBehaviour
{
    [System.Serializable]
    public class Objective
    {
        public string id;
        public string description;

        // Variables to add when spawning objects.
        public GameObject[] objectsToSpawn;
        public SpawnLocation[] spawnLocations; 

        // The time that the objective started.
        private float startTime;

        private List<GameObject> spawnedObjects;
        // This is where the objective objects get spawned.
        private SpawnLocation spawnLocation;

        public Objective(string id, float startTime, List<GameObject> spawnedObjects, SpawnLocation spawnLocation)
        {
            this.id = id;
            this.startTime = startTime;
            this.spawnedObjects = spawnedObjects;
            this.spawnLocation = spawnLocation;
        }

        // This is for objects like the cage, etc.
        private GameObject objectiveObjectRef;
        // To not be confused with spawnLocation, this is the location of things like the cage object when foxes get captured.
        // Potentially NPCS, etc.
        private Vector3 objectiveLocation;

        public Objective(string id, float startTime, GameObject objectRef, Vector3 location)
        {
            this.id = id;
            this.startTime = startTime;
            this.objectiveObjectRef = objectRef;
            this.objectiveLocation = location;
        }

        public int NumbActiveObjects()
        {
            int i = 0;
            foreach (var go in spawnedObjects)
            {
                if (go != null)
                {
                    i++;
                }
            }

            if (objectiveObjectRef != null)
            {
                i++;
            }

            return i;
        }

        // This is only the number of active fires of one fire objective.
        public int NumActiveFires()
        {
            if (this.id != "Fire")
            {
                Debug.Log("Not A Fire Objective");
                return 0;
            }

            FireSource fireSource = spawnedObjects[0].GetComponent<FireSource>();
            return fireSource.NumFires();
        }
    }

    private class ObjectiveInfo
    {
        public string name;
        public int numObjectsToSpawn;
        public string location;
    }

    [Header("Objectives Parameters")]
    [SerializeField] private float objectivesEventRate;
    [SerializeField] private float objectivesEventMax;
    [SerializeField] private int numObjectivesPerEvent;

    [Header("Objectives")]
    [SerializeField] public List<Objective> objectivesList;

    [SerializeField] private GameObject[] trashObjects;

    private Dictionary<string, Objective> objectives;
    private List<Objective> activeObjectives;

    private float previousEventStart;

    // Start is called before the first frame update
    void Start()
    {
        objectives = new Dictionary<string, Objective>();

        foreach (var objective in objectivesList)
        {
            this.objectives[objective.id] = objective;
        }

        activeObjectives = new List<Objective>();

        previousEventStart = -objectivesEventRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnFire();
            // SpawnTrash(1);
        }

        UpdateObjectives();
        UpdateObjectiveRates();
        // HandleObjectiveEvents();
        
    }

    private void UpdateObjectives()
    {
        for (int i = 0; i < activeObjectives.Count; i++)
        {
            Objective objective = activeObjectives[i];
            if (objective.NumbActiveObjects() == 0)
            {
                activeObjectives.RemoveAt(i);
                Debug.Log(objective.id + " Objective at Index " + i + " Completed");
                break;
            }
        }
    }

    private void HandleObjectiveEvents()
    {
        if (Time.time <= previousEventStart + objectivesEventRate)
        {
            return;
        }

        if (activeObjectives.Count < objectivesEventMax)
        {
            StartObjectiveEvent();
        }
    }

    private void StartObjectiveEvent()
    {
        // int playerCount = PhotonNetwork.PlayerList.Length;

        // if (playerCount == 1)
        // {

        // }
        for (int i = 0; i < numObjectivesPerEvent; i++)
        {
            ObjectiveInfo objectiveToStart = DecideObjective();
            StartObjective(objectiveToStart.name, objectiveToStart.numObjectsToSpawn);
        }

        previousEventStart = Time.time;
    }

    private ObjectiveInfo DecideObjective()
    {
        ObjectiveInfo info = new ObjectiveInfo();
        info.name = "Trash";
        info.numObjectsToSpawn = 5;
        return info;
    }

    private void StartObjective(string objectiveId, int numObjects, string location = "")
    {
        switch(objectiveId)
        {
            case "Trash":
                SpawnTrash(numObjects, location);
                break;
            case "Fire":
                SpawnFire(location);
                break;
        }

        Debug.Log(objectiveId + " Objective Begins At " + Time.time);
    }

    // Update objective rates based on number of players.
    private void UpdateObjectiveRates()
    {
        // TODO
    }

    public void SpawnTrash(int numTrash = 5, string location = "")
    {
        Objective trashObjective;
        
        if (!objectives.TryGetValue("Trash", out trashObjective))
        {
            Debug.Log("Trash Objective not Set");
            return;
        }

        GameObject[] objectsToSpawn = trashObjective.objectsToSpawn;
        if (objectsToSpawn.Length == 0)
        {
            Debug.Log("No Trash Objects to spawn");
            return;
        }

        foreach (var go in objectsToSpawn)
        {
            if (!IsTrashObject(go.name))
            {
                Debug.Log("A non trash object found in objects to spawn");
                return;
            }
        }

        int numSpawnLocations = trashObjective.spawnLocations.Length;

        if (numSpawnLocations == 0)
        {
            Debug.Log("Trash spawn location not set");
            return;
        }

        int spawnLocationIndex = Random.Range(0, numSpawnLocations);
        SpawnLocation spawnLocationArea = trashObjective.spawnLocations[spawnLocationIndex];

        if (location != "")
        {
            foreach (var loc in trashObjective.spawnLocations)
            {
                if (loc.name == location)
                {
                    spawnLocationArea = loc;
                }
            }
        }

        List<GameObject> spawnedObjects = new List<GameObject>();

        for (int i = 0; i < numTrash; i++)
        {
            Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
            GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
            GameObject spawnedObject = PhotonNetwork.Instantiate(objectToSpawn.name, spawnLocation, Quaternion.identity);
            spawnedObjects.Add(spawnedObject);
            // Debug.Log(objectToSpawn.name + " Spawned At " + spawnLocation);
        }

        activeObjectives.Add(new Objective(trashObjective.id, Time.time, spawnedObjects, spawnLocationArea));

        // Add Marker at center of spawnLocationArea and add to quest.
    }

    public void SpawnFire(string location = "")
    {
        Objective fireObjective;

        if (!objectives.TryGetValue("Fire", out fireObjective))
        {
            Debug.Log("Fire Objective not Set");
            return;
        }

        GameObject objectToSpawn = fireObjective.objectsToSpawn[0];
        if (objectToSpawn == null)
        {
            Debug.Log("FireSource Object not set");
            return;
        }

        if (objectToSpawn.name != "FireSource")
        {
            Debug.Log("Object is not a FireSource object");
            return;
        }

        int numSpawnLocations = fireObjective.spawnLocations.Length;

        if (numSpawnLocations == 0)
        {
            Debug.Log("Fire spawn location not set");
            return;
        }

        int spawnLocationIndex = Random.Range(0, numSpawnLocations);
        SpawnLocation spawnLocationArea = fireObjective.spawnLocations[spawnLocationIndex];

        if (location != "")
        {
            foreach (var loc in fireObjective.spawnLocations)
            {
                if (loc.name == location)
                {
                    spawnLocationArea = loc;
                }
            }
        }

        Vector3 spawnLocation = spawnLocationArea.GetRandomPoint();
        // Vector3 spawnLocation = spawnLocationArea.centre;

        List<GameObject> spawnedObjects = new List<GameObject>();

        GameObject spawnedObject = PhotonNetwork.Instantiate(objectToSpawn.name, spawnLocation, Quaternion.identity);
        // Debug.Log("Fire Spawned At " + spawnLocation);
        spawnedObjects.Add(spawnedObject);

        activeObjectives.Add(new Objective(fireObjective.id, Time.time, spawnedObjects, spawnLocationArea));


        // Add Marker at center of spawnLocationArea and add to quest.
    }

    public void SpawnLumberJack()
    {
        // TODO
    }

    public void SpawnDog()
    {
        // TODO
    }

    public void FoxCaptured(GameObject capturedFox)
    {
        Objective capturedFoxObjective;

        if (!objectives.TryGetValue("FoxCaptured", out capturedFoxObjective))
        {
            Debug.Log("Fox Captured Objective not Set");
            return;
        }

        activeObjectives.Add(new Objective(capturedFoxObjective.id, Time.time, capturedFox, capturedFox.transform.position));
        // TODO
    }

    private bool IsTrashObject(string name)
    {
        foreach (var go in trashObjects)
        {
            if (go.name == name)
            {
                return true;
            }
        }

        return false;
    }
}
