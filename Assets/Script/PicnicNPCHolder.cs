using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicnicNPCHolder : MonoBehaviour{
    public GameObject npc;
    public bool isOccupied = false;

    // Update is called once per frame
    void Update(){
        
    }

    public bool setNPC(GameObject newNpc) {
        if (isOccupied) return false;
        npc = newNpc;
        isOccupied = true;
        return true;
    }
}
