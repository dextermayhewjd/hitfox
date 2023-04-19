using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Timer : MonoBehaviourPun
{
    public Text timerText;
    private float startTime;
    // private float maxtime = 300f;//5 minutes
    public float playtime;// for testing 
    public Text victoryText;
    public Text failedText;
    public int currentPoints;
    public int requiredPoints;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //countdown from 5 minutes
            float t = Time.time - startTime;//seconds since started
            float rem = playtime - t;
            
            if (currentPoints >= requiredPoints) {
                victoryText.gameObject.SetActive(true);
                return;
            }

            if (rem <= 0)
            {
                timerText.text = "Time's up!";
                failedText.gameObject.SetActive(true);
                return;
            }
            else
            {
                string minutes = ((int)rem / 60).ToString();
                string seconds = (rem % 60).ToString("f0");
                if(rem%60 < 9.5)
                {
                    seconds = "0" + seconds;
                }
                string time = minutes + ":" + seconds;
                this.photonView.RPC("RPC_UpdateTimer", RpcTarget.AllBuffered, time);
                this.photonView.RPC("RPC_UpdateScore", RpcTarget.OthersBuffered, currentPoints);
            }
            //Note: change later so when reaches 1 min left turns red/blue
        }
    }

    public void IncreaseScore(int amount)
    {
        this.GetComponent<PhotonView>().RPC("RPC_IncreaseScore", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    void RPC_IncreaseScore(int amount)
    {
        currentPoints += amount;
    }

    [PunRPC]
    void RPC_UpdateScore(int actualPoints)
    {
        currentPoints = actualPoints;
    }

    [PunRPC]
    void RPC_UpdateTimer(string time)
    {
        timerText.text = time;
    }
}
