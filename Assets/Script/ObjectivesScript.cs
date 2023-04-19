using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectivesScript : MonoBehaviourPun
{
    public int requiredPoints = 10;
    public int currentPoints = 0;

    void Update()
    {
        if (currentPoints >= requiredPoints)
        {
            Debug.Log("VICTORY!");
            // load victory screen
        }

        // if time runs out or all players are captured -> load lose screen 
    }

    public void IncreaseScore(int amount)
    {
        currentPoints += amount;
    }
}
