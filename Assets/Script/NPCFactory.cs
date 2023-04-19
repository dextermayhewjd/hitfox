using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPCFactory : MonoBehaviour {

    public GameObject npc;
    public GameObject dog;
    public float spawnFrequency = 30;
    PhotonView view;
    float spawnTimer = 0;
    // Start is called before the first frame update
    void Start(){
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update(){
        if (view.IsMine) {
            if (spawnTimer <= 0) {
                spawnNpc();
                spawnTimer = spawnFrequency;
            }
            spawnTimer -= Time.deltaTime;
        }
    }

    void spawnNpc() {
        GameObject newNpc = PhotonNetwork.Instantiate(npc.name, transform.position+Vector3.up, Quaternion.identity);
        GameObject[] factories = GameObject.FindGameObjectsWithTag("NPCFactory");
        GameObject goTo;
        do {
            int i = Random.Range(0, factories.Length);
            goTo = factories[i];
        } while (goTo==gameObject);
        NPC_Behaviour npcBehaviour = newNpc.GetComponent<NPC_Behaviour>();
        npcBehaviour.goTo = goTo;
        if (Random.Range(0, 10) < 2) {
            GameObject newDog = PhotonNetwork.Instantiate(dog.name, transform.position + Vector3.up, Quaternion.identity);
            DoggoBehaviour dogBehaviour = newDog.GetComponent<DoggoBehaviour>();
            dogBehaviour.owner = newNpc;
            dogBehaviour.destination = goTo;
            npcBehaviour.dog = newDog;
        }
    }
}
