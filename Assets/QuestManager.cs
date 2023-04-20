using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{    
    public Text questText;
    public Quest[] quests; // An array to store all the quests
    public float questDisplayDuration = 5f;
    
    public int trashCount;
    public int collectedTrash = 0;

    public int FireToPutOut;


    void Start()
    {
        questText = GetComponentInChildren<Text>();
        trashCount = GameObject.FindGameObjectsWithTag("Trash").Length;
        

        // Define each quest
        quests = new Quest[] {
            new Quest() { ID = 0, description = "Find a bucket", completed = false },
            new Quest() { ID = 1, description = "Fill up the bucket with water ", completed = false },
            new Quest() { ID = 2, description = $"Extinguish the fire", completed = false },
            new Quest() { ID = 3, description = "Rescue the fox", completed = false },
            new Quest() { ID = 4, description = "try stop the lumberjack by stealing his axe ", completed = false },
            new Quest() { ID = 5, description = "try the forklift~~", completed = false },
            new Quest() { ID = 6, description = "Rescue the fox", completed = false },
            new Quest() { ID = 7, description = $"Collect all the trash {collectedTrash}/{trashCount}", completed = false, trashCount = trashCount, collectedTrashCount = 0 } 
        };
        
            DisplayQuest(); 

    }

      void Update()
    {

            foreach (Quest quest in quests)
        {
            if (quest.ID == 7 && !quest.completed)
            {
                if (quest.collectedTrashCount == quest.trashCount)
                {
                    CompleteQuest(7);
                }
            }
        }
    }
    
    public void DisplayQuest()
    {
            // questText.text += "- " + quest.description + "\n";
        questText.text = "";
        foreach (Quest quest in quests)
        {
            if(quest.completed)
            {
                questText.text += "- " + "<color=green>" + quest.description+ "</color>"+ "\n";
            }else{
                questText.text += "- " + quest.description + "\n";

            }
        } 


    }

    
    public void CompleteQuest(int id)
    {
        // Find the quest with the specified ID

    Quest quest = System.Array.Find(quests, q => q.ID == id);   
    quest.completed = true;
    DisplayQuest();
    }

}

public class Quest : MonoBehaviour
{
    public int ID;
    public string description;
    public bool completed;
    public int trashCount;
    public int collectedTrashCount;
    public int totalFireToPutOut;
}

