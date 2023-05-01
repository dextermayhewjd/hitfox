using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireSource : MonoBehaviour
{
    [SerializeField] public GameObject fireObject;
    [SerializeField] private float fireSourceSpreadRate;

    // The maximum number of fires that this fire source will produce.
    [SerializeField] public int maxFires;

    [HideInInspector] public float fireSpreadRate;

    public List<GameObject> fires;

    // Start is called before the first frame update
    void Start()
    {
        if (maxFires <= 0)
        {
            maxFires = 1;
        }

        fires = new List<GameObject>();        
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
            UpdateFireSpreadRate();
        }
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
            if (fires[i] == null)
            {
                fires.RemoveAt(i);
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
            GameObject fire = PhotonNetwork.InstantiateRoomObject(fireObject.name, location, Quaternion.identity);
            fire.GetComponent<FireSpread>().fireSource = this;
            fires.Add(fire);
        }
    }

    public void AddFire(GameObject fire)
    {
        fires.Add(fire);
    }

    private void FiresPutOut()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}
