using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FireSpread : MonoBehaviour
{
    public string objectToSpawn = "Fire";
    public float spawnDelay;
    private float timer = 0f;
    public float distanceX = 2.6f;
    public float distanceY = 2.6f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnDelay)
        {
            timer = 0f;

            // 20% chance of fire spreading
            if (Random.Range(0, 5) == 0)
            {
                Vector3 position = this.transform.position;
                RaycastHit hit;
                int location = Random.Range(0, 3);

                if (Random.Range(0, 2) == 1)
                {
                    distanceX *= -1;
                }

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
                    if (!hit.collider.CompareTag("Fire")) {
                        PhotonNetwork.Instantiate(prefabName: objectToSpawn, hit.point, Quaternion.identity);
                    } else {
                        Debug.Log("There is already a fire at this position");
                    }
                } else {
                    Debug.Log("There seems to be no ground at this position");
                }
            }
        }
    }

}
