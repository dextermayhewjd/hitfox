using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Objective : MonoBehaviour
{
    private PhotonView pv;

    public int viewId;
    public ObjectiveId objectiveId;
    public float startTime;
    public int[] spawnedObjectsId;
    public AreaId spawnLocationAreaId;
    public int spawnLocationIndex;
    public ObjectiveWaypointId objectiveWaypointId;
    public Vector3 objectiveWaypointPos;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        object[] instanceData = pv.InstantiationData;
        viewId = pv.ViewID;
        objectiveId = (ObjectiveId)instanceData[0];
        spawnedObjectsId = (int[])instanceData[1];
        spawnLocationAreaId = (AreaId)instanceData[2];
        spawnLocationIndex = (int)instanceData[3];
        objectiveWaypointId = (ObjectiveWaypointId)instanceData[4];
        objectiveWaypointPos = (Vector3)instanceData[5];
        startTime = Time.time;    
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActiveObjects();
        
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if(NumActiveObjects() == 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public int NumObjectsSpawned()
    {
        if (objectiveId == ObjectiveId.Fire)
        {
            return PhotonView.Find(spawnedObjectsId[0]).GetComponent<FireSource>().NumFires();
        }

        return spawnedObjectsId.Length;
    }

    private void UpdateActiveObjects()
    {
        for (int i = 0; i < spawnedObjectsId.Length; i++)
        {
            if (PhotonView.Find(spawnedObjectsId[i]) == null)
            {
                spawnedObjectsId[i] = -1;
            }
        }
    }
    public int NumActiveObjects()
    {
        int numActive = 0;

        for (int i = 0; i < spawnedObjectsId.Length; i++)
        {
            if (spawnedObjectsId[i] > 0)
            {
                if (PhotonView.Find(spawnedObjectsId[i]) != null)
                {
                    numActive++;
                }
            }
        }

        return numActive;
    }
}
