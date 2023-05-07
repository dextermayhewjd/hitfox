using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FireSpread : MonoBehaviour
{
    public int fireSourceID;

    private float timer = 0f;

    [SerializeField] private float distanceX;
    [SerializeField] private float distanceY;

    void Start()
    {
        fireSourceID = (int)GetComponent<PhotonView>().InstantiationData[0];
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonView.Find(fireSourceID) == null)
        {
            return;
        }

        FireSource fireSource = PhotonView.Find(fireSourceID).gameObject.GetComponent<FireSource>();

        if (fireSource.FireLimitReached())
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= fireSource.fireSpreadRate)
        {
            timer = 0f;

            // 20% chance of fire spreading
            if (Random.Range(0, 5) == 0)
            {
                Vector3 position = this.transform.position;
                RaycastHit hit;
                int location = Random.Range(0, 3);

                // 50% chance to change x direction
                if (Random.Range(0, 2) == 1)
                {
                    distanceX *= -1;
                }

                // 50% chance to change y direction
                if (Random.Range(0, 2) == 1)
                {
                    distanceY *= -1;
                }

                switch (location)
                {
                    case 0:
                        position += new Vector3(distanceX, 0, 0);
                        break;
                    case 1:
                        position += new Vector3(0, 0, distanceY);
                        break;
                    case 2:
                        position += new Vector3(distanceX, 0, distanceY);
                        break;
                }

                //Do a raycast along Vector3.down -> if you hit something the result will be given to you in the "hit" variable
                if (Physics.Raycast(position + new Vector3 (0, 100.0f, 0), Vector3.down, out hit, 200.0f)) {
                    if (hit.collider.CompareTag("Navigation Static")) {
                        Vector3 spawnPoint = hit.point;
                        spawnPoint.y -= 0.5f;
                        object[] instanceData = new object[1];
                        instanceData[0] = fireSourceID;
                        GameObject spawnedFire = PhotonNetwork.InstantiateRoomObject(fireSource.fireObject.name, spawnPoint, Quaternion.identity, 0, instanceData);
                        PhotonView.Find(fireSourceID).GetComponent<FireSource>().AddFire(spawnedFire.GetComponent<PhotonView>().ViewID);
                    } 
                    else 
                    {
                        Debug.Log("Fire can't spawn at this position");
                    }
                } 
                else 
                {
                    Debug.Log("There seems to be no ground at this position");
                }
            }
        }
    }
}
