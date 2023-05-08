using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviourPun
{
    public TextMeshProUGUI timerText;
    private float startTime;
    // private float maxtime = 300f;//5 minutes
    public float playtime;// for testing 
    public Image victoryText;
    public Image failedText;
    public int currentPoints;
    public int requiredPoints;
    bool wait2sec = false;
    float tsectimer = 0f;
    bool gameOver = false;

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
                this.photonView.RPC("RPC_Victory", RpcTarget.AllBuffered);
                // this.photonView.RPC("RPC_UpdateTimer", RpcTarget.AllBuffered, time);
                this.photonView.RPC("RPC_UpdateScore", RpcTarget.OthersBuffered, currentPoints);
                tsectimer += Time.deltaTime;
                if (tsectimer >= 5f)//waits 5 seconds before switching onto credits
                {
                    SceneManager.LoadScene(2);//loads credits scene
                }
                return;
            }

            else if (rem <= 0)
            {
                this.photonView.RPC("RPC_Lose", RpcTarget.AllBuffered);
                tsectimer += Time.deltaTime;
                if (tsectimer >= 5f)//waits 5 seconds before switching onto credits
                {
                    SceneManager.LoadScene(2);//loads credits scene
                }
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
                this.photonView.RPC("RPC_UpdateTimer", RpcTarget.All, time);
                
            }
            //Note: change later so when reaches 1 min left turns red/blue
        }
    }

    public void IncreaseScore(int amount)
    {
        if (!gameOver) {
            this.GetComponent<PhotonView>().RPC("RPC_IncreaseScore", RpcTarget.MasterClient, amount);
        }
    }

    [PunRPC]
    void RPC_IncreaseScore(int amount)
    {
        currentPoints += amount;
        this.photonView.RPC("RPC_UpdateScore", RpcTarget.Others, currentPoints);
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

    [PunRPC]
    void RPC_Victory()
    {
        gameOver = true;
        victoryText.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_Lose()
    {
        gameOver = true;
        failedText.gameObject.SetActive(true);
    }
}
