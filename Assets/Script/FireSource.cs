using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireSource : MonoBehaviour
{
    [SerializeField] private GameObject fireObject;

    public List<GameObject> fires;

    // Start is called before the first frame update
    void Start()
    {
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

    public int NumFires()
    {
        UpdateFireList();
        return fires.Count;
    }

    public void StartFire(Vector3 location)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject fire = PhotonNetwork.Instantiate(fireObject.name, location, Quaternion.identity);
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
