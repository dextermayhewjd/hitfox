using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] public SpawnLocation spawnLocation;

    private bool alreadySpawnedPlayer = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && !alreadySpawnedPlayer)
        {
            PhotonNetwork.Instantiate(playerObject.name, spawnLocation.GetRandomPoint(), Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(playerObject.name, spawnLocation.GetRandomPoint(), Quaternion.identity);
            alreadySpawnedPlayer = true;
        }
    }
}
