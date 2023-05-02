using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

// How long the ping timer will last for until it gets destroyed.
public class Waypoint : MonoBehaviour
{
    [SerializeField] Image img;

    [SerializeField] TMP_Text headerTextField;
    [SerializeField] TMP_Text subHeaderTextField;
    [SerializeField] TMP_Text distanceTextField;

    [SerializeField] string distanceUnit;

    public bool useTimer = false;
    public float timer;

    // Where to calculate from distance.
    private GameObject origin;

    // Values to apply.
    [HideInInspector] public Vector3 targetPos;
    [HideInInspector] public GameObject targetObject;
    [HideInInspector] public string header;
    [HideInInspector] public string subHeader;

    private float distance;

    public bool trackObject = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject fox in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(fox.GetComponent<PhotonView>().IsMine)
            {
                origin = fox;
            }
        }

        headerTextField.text = header;
        subHeaderTextField.text = subHeader;

    }

    // Update is called once per frame
    void Update()
    {
        if (trackObject)
        {
            if (targetObject == null)
            {
                Destroy();
            }
        }
        if (targetObject != null)
        {
            targetPos = targetObject.transform.position;
            // To use if the object has been interacted with such as if it was picked up for example.
            // Need to set a property in the object for this.
            // if (target.property)
            // {
            //     StopPing;
            // }
        }

        distance = Distance(origin.transform.position, targetPos);
        distanceTextField.text = distance.ToString() + distanceUnit;

        float minX = img.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(targetPos);

        if (Vector3.Dot((targetPos - Camera.main.transform.position), Camera.main.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX; 
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        img.transform.position = pos;

        if (useTimer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Destroy();
            }
        }
    }
        
    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    private float Distance(Vector3 origin, Vector3 target)
    {
        return Mathf.Round(Vector3.Distance(origin, target));
    }
}
