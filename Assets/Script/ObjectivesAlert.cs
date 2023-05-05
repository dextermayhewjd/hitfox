using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesAlert : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject objectiveAlertDisplayParent;

    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text subHeader;
    [SerializeField] private TMP_Text body;

    [System.Serializable]
    private class ObjectiveAlert 
    {
        public ObjectiveId objectiveId;
        [HideInInspector] public string location;
        [TextArea] public string header;
        [TextArea] public string subHeader;
        [TextArea] public string body;

        public ObjectiveAlert(ObjectiveId objectiveId, string location)
        {
            this.objectiveId = objectiveId;
            this.location = location;
        }
    }

    [Header("Alert Options")]
    [SerializeField] private float objectiveAlertDuration;

    [Header("Objectives Info")]
    [SerializeField] private ObjectiveAlert[] objectiveAlertList;
    private Dictionary<ObjectiveId, ObjectiveAlert> objectiveAlertTable;


    private List<ObjectiveAlert> objectiveAlertBuffer;

    private float objectiveAlertDisplayStartTime;
    private bool objectiveAlertDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        objectiveAlertTable = new Dictionary<ObjectiveId, ObjectiveAlert>();

        foreach (var objectiveAlert in objectiveAlertList)
        {
            this.objectiveAlertTable[objectiveAlert.objectiveId] = objectiveAlert;
        }

        if (objectiveAlertDisplayParent != null)
        {
            objectiveAlertDisplayParent.SetActive(false);
        } 

        objectiveAlertBuffer = new List<ObjectiveAlert>();
    }

    // Update is called once per frame
    void Update()
    {
        if (objectiveAlertBuffer.Count == 0)
        {
            return;
        }

        if (!objectiveAlertDisplayed)
        {
            objectiveAlertDisplayStartTime = Time.time;
        }

        if (Time.time - objectiveAlertDisplayStartTime <= objectiveAlertDuration)
        {
            if(!objectiveAlertDisplayed)
            {
                DisplayObjectiveAlert(objectiveAlertBuffer[0]);
            }
            objectiveAlertDisplayed = true;
        }
        else
        {
            if (objectiveAlertDisplayed)
            {
                objectiveAlertBuffer.RemoveAt(0);
                HideObjectiveAlert();
            }
            objectiveAlertDisplayed = false;
        }
    }

    public void AddObjectiveAlertToBuffer(ObjectiveId objectiveId, string location)
    {
        objectiveAlertBuffer.Add(new ObjectiveAlert(objectiveId, location));
    }

    private void DisplayObjectiveAlert(ObjectiveAlert objectiveAlert)
    {
        ObjectiveAlert objectiveAlertPreset = objectiveAlertTable[objectiveAlert.objectiveId];
        header.text = objectiveAlertPreset.header;
        subHeader.text = objectiveAlertPreset.subHeader + objectiveAlert.location;
        body.text = objectiveAlertPreset.body;
        objectiveAlertDisplayParent.SetActive(true);
    }

    private void HideObjectiveAlert()
    {
        objectiveAlertDisplayParent.SetActive(false);
    }
}
