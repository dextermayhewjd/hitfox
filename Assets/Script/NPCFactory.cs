using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NPCFactory : MonoBehaviour {

    public GameObject npc;
    public GameObject dog;
    public float secondsPerSpawn = 120;
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
                spawnTimer = secondsPerSpawn;
                
            }
            spawnTimer -= Time.deltaTime;

        }
    }

    void spawnNpc() {
        
        GameObject newNpc = PhotonNetwork.Instantiate(npc.name, transform.position+Vector3.up, Quaternion.identity);
        GameObject[] factories = System.Array.FindAll(GameObject.FindGameObjectsWithTag("NPCFactory"), (x)=>!(x==gameObject));
        GameObject[] picnics = System.Array.FindAll(GameObject.FindGameObjectsWithTag("PicnicSpot"),(x)=> {
            PicnicNPCHolder npcHolder = x.GetComponent<PicnicNPCHolder>();
            return !npcHolder.isOccupied;
        });
        GameObject goTo = gameObject;
        if (Random.Range(0, 100) < 20 && picnics.Length!=0) {
            int i = Random.Range(0, picnics.Length);
            PicnicNPCHolder npcHolder = picnics[i].GetComponent<PicnicNPCHolder>();
            if (npcHolder.setNPC(newNpc)) { 
                goTo = picnics[i];
            }
        }
        if (goTo == gameObject) { 
            int i = Random.Range(0, factories.Length);
            goTo = factories[i];
        }
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
