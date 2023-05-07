using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof (NewCarController))]
public class NewCarUserControl : MonoBehaviourPun
{
    private NewCarController m_Car; // the car controller we want to use
    public bool driving = false;
    public PhotonView driver = null;
    public float h = 0f, v = 0f, handbrake = 1;
    public GameObject hud;
    public PhotonView playerOutside = null;

    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<NewCarController>();
        hud = GameObject.Find("HUD");
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine&& !other.gameObject.GetComponent<PlayerMovement>().driving && !other.gameObject.GetComponent<PlayerMovement>().hidden) {
            hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
            hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Drive"; // change text of action
            playerOutside = other.gameObject.GetComponent<PhotonView>();
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.GetComponent<PhotonView>().IsMine) {
            hud.transform.Find("EnterButton").gameObject.SetActive(false); // hide button
            playerOutside = null;
        }
    }

    private void FixedUpdate()
    {
        // pass the input to the car!
        if (driving) {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            handbrake = Input.GetAxis("Jump");
            
        } else {
            if (handbrake >= 1f) {
                handbrake = 1f;
            } else {
                handbrake += 0.01f;
            }
        }
        m_Car.Move(h, v, v, handbrake);
    }

    private void Update()
    {
        if (driving && driver.IsMine)
        {
            hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
            hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Exit"; // change text of action
            if (Input.GetButtonDown("Enter"))
            {
                hud.transform.Find("EnterButton").gameObject.SetActive(false); // show button
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = driver.transform;
                cam.Follow = driver.transform;
                this.photonView.RPC("RPC_ExitAccPlayer", RpcTarget.All);
                
            }
        }
        else if (!driving && playerOutside.IsMine && !playerOutside.gameObject.GetComponent<PlayerMovement>().hidden) 
        {
            hud.transform.Find("EnterButton").gameObject.SetActive(true); // show button
            hud.transform.Find("EnterButton").Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>().text = "Drive"; // change text of action
            if (Input.GetButtonDown("Enter")) {
                CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
                cam.LookAt = transform;
                cam.Follow = transform;
                base.photonView.RequestOwnership();
                this.photonView.RPC("RPC_EnterAccPlayer", RpcTarget.All, playerOutside.ViewID);
            }
        }
    }

    [PunRPC]
    void RPC_ExitAccPlayer()
    {
        Debug.Log("Player left vehicle");
        driver.transform.position += Camera.main.transform.forward;
        driver.transform.SetParent(null);
        driver.GetComponent<PlayerMovement>().driving = false;
        driver.GetComponent<CharacterController>().enabled = true;
        driver.GetComponent<PlayerInteraction>().enabled = true;
        driver.GetComponent<PlayerMovement>().enabled = true;
        // colliders.RemoveAll(item => item.gameObject.GetComponent<PhotonView>().ViewID == driver.ViewID);
        // driver.gameObject.SetActive(true);
        driver = null;
        driving = false;
    }

    [PunRPC]
    void RPC_EnterAccPlayer(int player)
    {
        Debug.Log("Player entered vehicle");
        driving = true;
        driver = PhotonView.Find(player);
        driver.transform.SetParent(this.transform.Find("AccelerateSeat"));
        // driver.gameObject.SetActive(false);
        driver.transform.localPosition = Vector3.zero;
        driver.transform.localRotation = Quaternion.identity;
        driver.GetComponent<PlayerMovement>().driving = true;
        driver.GetComponent<CharacterController>().enabled = false;
        driver.GetComponent<PlayerInteraction>().enabled = false;
        driver.GetComponent<PlayerMovement>().enabled = false;
    }
}

