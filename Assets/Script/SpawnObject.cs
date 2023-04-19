using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnObject : MonoBehaviourPun
{
    public GameObject objectToSpawn;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float heightY;
    bool alreadySpawnedPlayer = false;

    private void Start()
    {
        if (objectToSpawn.name == "PlayerNew")
        {
            alreadySpawnedPlayer = true;
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), heightY, Random.Range(minZ, maxZ));
            PhotonNetwork.Instantiate(objectToSpawn.name, randomPosition, Quaternion.identity);
        }

        if (PhotonNetwork.IsMasterClient && !alreadySpawnedPlayer)
        {
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), heightY, Random.Range(minZ, maxZ));
            PhotonNetwork.Instantiate(objectToSpawn.name, randomPosition, Quaternion.identity);
        }
    }
}
