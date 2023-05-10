using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class CreditsToStart : MonoBehaviour
{
    public float tsectimer = 0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        tsectimer += Time.deltaTime;
        if (tsectimer >= 20f)//waits 5 seconds before switching onto credits
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(0);//loads credits scene
        }
    }
}
