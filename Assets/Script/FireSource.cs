using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireSource : MonoBehaviour
{
    PhotonView pv;

    [SerializeField] public GameObject fireObject;
    [SerializeField] private float fireSourceSpreadRate;

    // The maximum number of fires that this fire source will produce.
    [SerializeField] public int maxFires;

    [HideInInspector] public float fireSpreadRate;

    public List<int> fires;

    [SerializeField] private int points;

    void Awake()
    {
        fires = new List<int>();
    }
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        if (maxFires <= 0)
        {
            maxFires = 1;
        }

        StartFire(this.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (fires.Count == 0)
            {
                FiresPutOut();
                return;
            }

            UpdateFireList();
        }

        UpdateFireSpreadRate();
    }

    // Increase fire spread rate if there are more fires.
    private void UpdateFireSpreadRate()
    {
        if (NumFires() <= 1)
        {
            fireSpreadRate = fireSourceSpreadRate;
        }
        else
        {
            fireSpreadRate = fireSourceSpreadRate * (1 - ((float)NumFires() / (float)maxFires));
        }
    }

    private void UpdateFireList()
    {
        for (int i = 0; i < fires.Count; i++)
        {
            if (PhotonView.Find(fires[i]) == null)
            {
                RemoveFire(i);
                break;
            }
        }
    }

    public bool FireLimitReached()
    {
        return NumFires() >= maxFires;
    }

    public int NumFires()
    {
        UpdateFireList();
        return fires.Count;
    }

    public void StartFire(Vector3 location)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            object[] instanceData = new object[1];
            instanceData[0] = pv.ViewID;
            GameObject fire = PhotonNetwork.InstantiateRoomObject(fireObject.name, location, Quaternion.identity, 0, instanceData);
            AddFire(fire.GetComponent<PhotonView>().ViewID);
        }
    }

    public void AddFire(int fireID)
    {
        pv.RPC("RPC_AddFire", RpcTarget.All, fireID);
    }

    [PunRPC]
    public void RPC_AddFire(int fireID)
    {
        fires.Add(fireID);
    }

    public void RemoveFire(int index)
    {
        pv.RPC("RPC_RemoveFire", RpcTarget.All, index);
    }

    [PunRPC]
    public void RPC_RemoveFire(int index)
    {
        fires.RemoveAt(index);
    }

    private void FiresPutOut()
    {
        GameObject objectives = GameObject.Find("Timer+point");
        objectives.GetComponent<Timer>().IncreaseScore(points);

        GameObject pointsDisplay = GameObject.Find("PointsPopupDisplay");
        if (pointsDisplay != null)
        {
            pointsDisplay.GetComponent<PointsPopupDisplay>().PointsPopup(points);
        }
        PhotonNetwork.Destroy(this.gameObject);
    }
}
