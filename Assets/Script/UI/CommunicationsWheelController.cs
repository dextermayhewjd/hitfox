using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CommunicationsWheelController : MonoBehaviour
{
    // Todo
    // - Expand to suit needs.

    // Main UI Controller.
    private GameObject uiControllerObj;
    private UIController uiController;

    private InputController inputController;

    [SerializeField] private KeyCode wheelKey = KeyCode.Tab;

    // The parent object of the wheel.
    [SerializeField] private GameObject wheelParent;

    // Array of parent object of wheels of differnt sizes.
    [SerializeField] private GameObject[] wheelParents;

    // Crosshair object.
    [SerializeField] private GameObject crossHair;

    // Layers;
    private int layerGround;
    private int layerObject;
    private int layerPickableObjects;
    private int currLayer;

    // Rays.
    private bool castRays = false;
    private Ray ray;
    private RaycastHit hit;

    // Settings.
    // If the ping is set to the ground. It will offset it by this much.
    [SerializeField] private float groundYOffset;

    // Defines the parameters of a single sector of the wheel.
    public class CommunicationPing
    {
        public float sectorStart;
        public float sectorEnd;
        public string message;

        public CommunicationPing(float sectorStart, float sectorEnd, string message)
        {
            this.sectorStart = sectorStart;
            this.sectorEnd = sectorEnd;
            this.message = message;
        }
    }

    // Wheels
    // Expand to add more communications depending on what the player is looking at.
    enum Wheel
    {
        GROUND,
        PICKABLE
    }

    [SerializeField] private float[] sectorStartsDegrees;

    // Holds all communication fields that can be placed on the ground.
    [SerializeField] private List<string> groundWheel;

    // Holds all of the fields when the player is captured.
    [SerializeField] private List<string> capturedWheel;

    // Holds all the communication pings regarding any ojbects that can be picked up.
    [SerializeField] private List<string> pickableObjectsWheel;

    private Wheel currWheel;
    private List<CommunicationPing> currWheelList;

    // Start is called before the first frame update
    void Start()
    {
        uiControllerObj = GameObject.Find("UIController");
        uiController = uiControllerObj.GetComponent<UIController>();

        inputController = GameObject.Find("InputController").GetComponent<InputController>();

        SetLayers();

        if (wheelParent != null)
        {
            wheelParent.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputController.GetInputDown(wheelKey))
        {
            OpenWheel();
        }
        else if (inputController.GetInputUp(wheelKey))
        {
            CommunicationPing selectedPing = GetSelectedPing(currWheelList);
            if (selectedPing != null) {
                Debug.Log(selectedPing.message);
                StartPing(selectedPing);
            }
            CloseWheel();
        }

        if (castRays)
        {
            CastRaysFromCrossHair();

            switch(currWheel)
            {
                case Wheel.GROUND:
                    currWheelList = CreateWheel(groundWheel);
                    break;
                case Wheel.PICKABLE:
                    currWheelList = CreateWheel(pickableObjectsWheel);
                    break;
                default:
                    currWheelList = CreateWheel(groundWheel);
                    break;

            }
        }
    }

    private void SetLayers()
    {
        layerGround = LayerMask.NameToLayer("Ground");
        layerObject = LayerMask.NameToLayer("Object");
        layerPickableObjects = LayerMask.NameToLayer("PickableObjects");
    }

    private void OpenWheel()
    {
        if (wheelParent != null)
        {
            wheelParent.SetActive(true);
            castRays = true;
            uiController.LockFreeLook();
            uiController.UnlockCursor();
        }
        else
        {
            Debug.Log("OpenWheel(): Wheel Parent Missing");
        }
    }

    private void CloseWheel()
    {
        if (wheelParent != null)
        {
            wheelParent.SetActive(false);
            castRays = false;
            uiController.UnlockFreeLook();
            uiController.LockCursor();
        }
        else
        {
            Debug.Log("CloseWheel(): Wheel Parent Missing");
        }
    }

    private void CastRaysFromCrossHair()
    {
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 crossHairPos = screenCentre;

        ray = Camera.main.ScreenPointToRay(crossHairPos);

        // So the ray ignores the player layer.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Player")))
        {
            currLayer = hit.transform.gameObject.layer;

            if (currLayer == layerGround)
            {
                currWheel = Wheel.GROUND;
            }
            else if (currLayer == layerObject)
            {
            }
            else if (currLayer == layerPickableObjects)
            {
                currWheel = Wheel.PICKABLE;
            }
            else
            {
                currWheel = Wheel.GROUND;
            }
        }
    }

    private CommunicationPing GetSelectedPing(List<CommunicationPing> wheel)
    {
        foreach (CommunicationPing currPing in wheel)
        {
            if (WithinSector(currPing.sectorStart, currPing.sectorEnd, Input.mousePosition))
            {
                return currPing;
            }
        }

        return null;
    } 

    private void StartPing(CommunicationPing ping) {
        PhotonView pv = GetComponent<PhotonView>();

        Vector3 pingPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);

        Vector3 playerPos = new Vector3(0, 0, 0);

        foreach (GameObject fox in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(fox.GetComponent<PhotonView>().IsMine) {
                playerPos = fox.transform.position;
            }
        }

        // Ground Layer.
        if (currLayer == layerGround)
        {
            pingPos.y += groundYOffset;
            pv.RPC("PingGroundMarker", RpcTarget.All, pingPos, 8f, ping.message);
        }
        // Object Layer.
        else if (currLayer == layerObject || currLayer == layerPickableObjects)
        {
            PhotonView objectPV = hit.transform.gameObject.GetComponent<PhotonView>();
            if (objectPV != null)
            {
                pv.RPC("PingObjectMarker", RpcTarget.All, objectPV.ViewID, 8f, ping.message);
            }
            else
            {
                Debug.Log("Object does not have a photon view");
            }
        }
        // Place the ping on top of the player if a ground or object is not being looked at.
        else
        {
            pv.RPC("PingGroundMarker", RpcTarget.All, playerPos, 8f, ping.message);
        }
    }

    private List<CommunicationPing> CreateWheel(List<string> msgs)
    {
        List<CommunicationPing> newList = new List<CommunicationPing>();

        int numMsg = msgs.Count;

        if (numMsg == 0 || numMsg > 6)
        {
            return newList;
        }

        float sectorSize = (numMsg == 1) ? 180 : 360 / numMsg;

        float sectorStart = 0;

        sectorStart = sectorStartsDegrees[numMsg - 1];

        for (int i = 0; i < numMsg; i++)
        {
            float sectorEnd = (sectorStart + sectorSize) % 360;
            if (sectorEnd == 0)
            {
                sectorEnd = 360;
            }
            newList.Add(new CommunicationPing(sectorStart, sectorEnd, msgs[i]));
            sectorStart = sectorEnd;
        }

        GameObject currWheelObj = wheelParents[0];
        int wheelParentIndex = 0;
        foreach (GameObject wheelParent in wheelParents)
        {
            if (wheelParentIndex == numMsg - 1)
            {
                wheelParent.SetActive(true);
                currWheelObj = wheelParent;
            }
            else
            {
                wheelParent.SetActive(false);
            }
            wheelParentIndex++;
        }

        if (currWheelObj != null)
        {
            TMP_Text[] wheelTextFields;
            wheelTextFields = currWheelObj.GetComponentsInChildren<TMP_Text>();
            int textFieldsIndex = 0;
            foreach (TMP_Text textField in wheelTextFields)
            {
                textField.text = msgs[textFieldsIndex];
                textFieldsIndex++;
            }
        }

        return newList;
    }

    // Check if the point lies within the sector of the circle.
    // Max 180 degree sector.
    // @param sectorStart {float} The starting angle of the sector in degrees.
    // @param sectorEnd {float} The end angle of the sector in degrees, counter-clockwise from the sectorStart.
    // @param point {Vector2} The point in question.
    // @return {bool} Returns true if the point is within the sector, false otherwise.
    private bool WithinSector(float sectorStart, float sectorEnd, Vector2 point)
    {
        Vector2 wheelCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        float wheelDiameter = (Screen.width > Screen.height) ? Screen.height : Screen.width;
        float wheelRadius = wheelDiameter / 2;

        // Point relative to the circle centre.
        Vector2 relativePoint = new Vector2(point.x - wheelCentre.x, point.y - wheelCentre.y);

        sectorStart = sectorStart * Mathf.Deg2Rad;
        sectorEnd = sectorEnd * Mathf.Deg2Rad;

        Vector2 sectorStartVector = VectorFromAngle(sectorStart, wheelRadius, wheelCentre);
        Vector2 sectorEndVector = VectorFromAngle(sectorEnd, wheelRadius, wheelCentre);

        return !PointClockwiseToVector(sectorStartVector, relativePoint)
            && PointClockwiseToVector(sectorEndVector, relativePoint)
            && PointOutsideCentre(0.1f * wheelRadius, relativePoint); // change first input to match wheel UI. This portion gets ignored as part of the wheel.
    }

    private bool PointOutsideCentre(float threshold, Vector2 point)
    {
        return point.x * point.x + point.y * point.y > threshold * threshold;
    }
    private bool PointClockwiseToVector(Vector2 vector, Vector2 point)
    {
        return -vector.x * point.y + vector.y * point.x > 0;
    }

    private bool PointWithinRadius(float radius, Vector2 point)
    {
        return point.x * point.x + point.y * point.y <= radius * radius;
    }

    private Vector2 VectorFromAngle(float theta, float radius, Vector2 circleCentre)
    {
        return new Vector2(Mathf.Cos(theta) * radius , Mathf.Sin(theta) * radius);
    }
}
