using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPCFactory : MonoBehaviour {

    public GameObject npc;
    PhotonView view;
    float spawnTimer = 0;
    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            if (spawnTimer <= 0) {
                spawnNpc();
                spawnTimer = 30;
            }
            spawnTimer -= Time.deltaTime;
        }
    }

    void spawnNpc() {
        GameObject newNpc = PhotonNetwork.Instantiate(npc.name, transform.position, Quaternion.identity);
        GameObject[] factories = GameObject.FindGameObjectsWithTag("NPCFactory");
        GameObject goTo;
        do {
            int i = Random.Range(0, factories.Length);
            goTo = factories[i];
        } while (goTo==gameObject);
        newNpc.GetComponent<NPC_Behaviour>().goTo = goTo;
    }
}
