using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CommunicationsWheelController : MonoBehaviour
{
    // Todo
    // - Expand to add more communications depending on what the player is looking at.

    // Main UI Controller.
    private GameObject uiControllerObj;
    private UIController uiController;

    [SerializeField] private KeyCode wheelKey = KeyCode.Tab;

    // The parent object of the wheel.
    [SerializeField] private GameObject wheelParent;

    [SerializeField] private GameObject groundWheelParent;

    // Screen Variables.
    private int screenWidth;
    private int screenHeight;
    private Vector2 screenCentre;

    // Wheel Dimensions.
    private Vector2 wheelCentre;
    private float wheelDiameter;
    private float wheelRadius;

    // Layers;
    private int layerGround;
    private int layerObject;
    private int currLayer;

    // Rays.
    private bool castRays = false;
    private Ray ray;
    private RaycastHit hit;

    // Settings.
    // If the ping is set to the ground. It will offset it by this much.
    [SerializeField] private float groundYOffset;

    // Defines the parameters of a single sector of the wheel.
    [System.Serializable]
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

    private Hashtable wheels;

    // Holds all communication fields that can be placed on the ground.
    [SerializeField] private List<CommunicationPing> groundWheel;

    // Holds all of the fields when the player is captured.
    [SerializeField] private List<CommunicationPing> capturedWheel;

    // Holds all the communication pings regarding any ojbects that can be picked up.
    [SerializeField] private List<CommunicationPing> pickableObjectsWheel;

    private Wheel currWheel;

    // Start is called before the first frame update
    void Start()
    {
        uiControllerObj = transform.parent.gameObject;
        uiController = uiControllerObj.GetComponent<UIController>();

        UpdateScreenVars();
        UpdateWheelVars();
        UpdateLayers();
        // Define the wheel programatically.
        // CreateWheel();
        CloseWheel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(wheelKey))
        //if (Input.GetButtonDown(wheelKey))
        {
            OpenWheel();
        }
        else if (Input.GetKeyUp(wheelKey))
        //if (Input.GetButtonUp(wheelKey))
        {
            CommunicationPing selectedPing = GetSelectedPing();
            if (selectedPing != null) {
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
                    groundWheelParent.SetActive(true);
                    break;
            }
        }
    }

    private void UpdateScreenVars()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        screenCentre = new Vector2(screenWidth / 2, screenHeight / 2);
    }

    private void UpdateWheelVars()
    {
        wheelCentre = screenCentre;
        wheelDiameter = (screenWidth > screenHeight) ? screenHeight : screenWidth;
        wheelRadius = wheelDiameter / 2;

        wheels = new Hashtable();
        wheels[Wheel.GROUND] = groundWheel;
    }

    private void UpdateLayers()
    {
        layerGround = LayerMask.NameToLayer("Ground");
        layerObject = LayerMask.NameToLayer("Object");
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
            Debug.Log("Missing Object Parent");
            return;
        }

        // Add animations here to see part of the wheel is hovered.
    }

    private void CloseWheel()
    {
        if (wheelParent != null)
        {
            wheelParent.SetActive(false);
            groundWheelParent.SetActive(false);
            castRays = false;
            uiController.UnlockFreeLook();
            uiController.LockCursor();
        }
    }

    private void CastRaysFromCrossHair()
    {
        // Vector2 crossHairPos = new Vector2(x, y);
        Vector2 crossHairPos = screenCentre;

        ray = Camera.main.ScreenPointToRay(crossHairPos);

        // So the ray ignores the player object. Change to ignore only the the player itself and not other players.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.GetMask("Player")))
        {
            currLayer = hit.transform.gameObject.layer;

            if (currLayer == layerGround)
            {
                currWheel = Wheel.GROUND;
            }
            else if (currLayer == layerObject)
            {
                switch(hit.transform.gameObject.name)
                {
                    case "":
                        currWheel = Wheel.PICKABLE;
                        break;
                }
            }
            else
            {
                currWheel = Wheel.GROUND;
            }
        }
    }

    private CommunicationPing GetSelectedPing()
    {
        foreach (CommunicationPing currPing in (List<CommunicationPing>)wheels[currWheel])
        {
            if (WithinSector(currPing.sectorStart, currPing.sectorEnd, Input.mousePosition))
            {
                return currPing;
            }
        }

        return null;
    } 

    private void StartPing(CommunicationPing ping) {
        PingMarkerController pingController = GetComponent<PingMarkerController>();

        Vector3 pingPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);

        // GameObject player = GameObject.FindWithTag("Player");
        // string userName = player.GetComponent<PhotonView>().Owner.NickName;
        string userName = "Player";

        // Ground Layer.
        if (currLayer == layerGround)
        {
            pingPos.y += groundYOffset;
            pingController.PlaceGroundMarker(pingPos, 5, userName, ping.message);
        }
        // Object Layer.
        else if (currLayer == layerObject)
        {

        }
        // Place the ping on top of the player if a ground or object is not being looked at.
        else
        {
            // Dunno how this will work with photon but need to get the player object that the ping was called from.
            pingPos = GameObject.FindWithTag("Player").transform.position;
            pingController.PlaceGroundMarker(pingPos, 5, userName, ping.message);
        }
    }

    // Code the messages ping.
    private void CreateWheel()
    {
    }

    // Check if the point lies within the sector of the circle.
    // @param sectorStart {float} The starting angle of the sector in degrees.
    // @param sectorEnd {float} The end angle of the sector in degrees, counter-clockwise from the sectorStart.
    // @param point {Vector2} The point in question.
    // @return {bool} Returns true if the point is within the sector, false otherwise.
    private bool WithinSector(float sectorStart, float sectorEnd, Vector2 point)
    {
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
