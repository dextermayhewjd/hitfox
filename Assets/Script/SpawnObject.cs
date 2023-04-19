using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnObject : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    public float heightY;

    private void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), heightY, Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(objectToSpawn.name, randomPosition, Quaternion.identity);
    }
}
