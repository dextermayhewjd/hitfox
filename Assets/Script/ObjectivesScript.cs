using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectivesScript : MonoBehaviourPun
{
    public int requiredPoints = 10;
    public int currentPoints = 0;

    public void IncreaseScore(int amount)
    {
        currentPoints += amount;
    }
}
