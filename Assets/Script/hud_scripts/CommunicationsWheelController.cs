using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CommunicationsWheelController : MonoBehaviour
{
    // Todo.
    // Get username and add it to marker.
    // Lock camera when opening wheel.

    // Change in the future to use Unity's project input manager.
    [SerializeField] private KeyCode wheelKey = KeyCode.Tab;

    // The parent object of the wheel.
    [SerializeField] private GameObject wheelParent;

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

    // Player Object.
    // [SerializeField] private GameObject player;

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

    public List<CommunicationPing> communicationPings;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScreenVars();
        UpdateWheelVars();
        UpdateLayers();
        CreateWheel();
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
            // Lock camera rotation.
            // Unlock cursor.
            wheelParent.SetActive(true);
            castRays = true;
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
            castRays = false;
            // Unlock camera rotation.
            // Lock cursor.
        }
    }

    private void CastRaysFromCrossHair()
    {
        ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currLayer = hit.transform.gameObject.layer;
        }
    }

    private CommunicationPing GetSelectedPing()
    {
        foreach (CommunicationPing currPing in communicationPings)
        {
            if (WithinSector(currPing.sectorStart, currPing.sectorEnd, Input.mousePosition))
            {
                return currPing;
            }
        }

        return null;
    } 

    private void StartPing(CommunicationPing ping) {
        WaypointMarkerController pingController = GetComponent<WaypointMarkerController>();

        Vector3 pingPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);

        // player = GameObject.FindWithTag("Player");

        // string userName = player.GetComponent<PhotonView>().Owner.NickName;
        string userName = "Player";

        // Ground Layer.
        if (currLayer == layerGround)
        {
            pingPos.y += groundYOffset;
            pingController.PlaceGroundMarker(pingPos, userName, ping.message);
        }
        else if (currLayer == layerObject)
        {

        }
        else
        {
            pingPos = GameObject.FindWithTag("Player").transform.position;
            pingController.PlaceGroundMarker(pingPos, userName, ping.message);
        }
    }

    private void CreateWheel()
    {
        // Code the messages ping.
        communicationPings = new List<CommunicationPing>();
        communicationPings.Add(new CommunicationPing(45, 135, "Wants to Attack"));
        communicationPings.Add(new CommunicationPing(135, 225, "Needs Assistance"));
        communicationPings.Add(new CommunicationPing(225, 315, "Wants to be Freed"));
        communicationPings.Add(new CommunicationPing(315, 45, "Is feeling lucky"));
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
            && PointOutsideCentre(0.1f * wheelRadius, relativePoint);
            // && PointWithinRadius(wheelRadius, relativePoint);
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
