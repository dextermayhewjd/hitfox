using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunicationsWheelController : MonoBehaviour
{
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

    private static int pingIndex;

    // Angle works like how a standard circle would represent it except its counter-clockwise.
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
        wheelDiameter = (screenWidth > screenHeight) ? screenWidth : screenHeight;
        wheelRadius = wheelDiameter / 2;
    }

    private void OpenWheel()
    {
        if (wheelParent != null)
        {
            wheelParent.SetActive(true);
        }
        else
        {
            Debug.Log("Missing Object Parent");
            return
        }

        // Add animations here to see part of the wheel is hovered.
    }

    private void CloseWheel()
    {
        if (wheelParent != null)
        {
            wheelParent.SetActive(false);
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
        Debug.Log(ping.message);
    }

    private void CreateWheel()
    {
        communicationPings = new List<CommunicationPing>();
        communicationPings.Add(new CommunicationPing(0, 90, "1"));
        communicationPings.Add(new CommunicationPing(90, 180, "2"));
        communicationPings.Add(new CommunicationPing(180, -90, "3"));
        communicationPings.Add(new CommunicationPing(-90, 0, "4"));
    }

    // Check if the point lies within the sector of the circle.
    // @param sectorStart {float} The starting angle of the sector in degrees.
    // @param sectorEnd {float} The end angle of the sector in degrees.
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
            && PointWithinRadius(wheelRadius, relativePoint);
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
