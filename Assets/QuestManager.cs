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
        int trashCount = GameObject.FindGameObjectsWithTag("trash").Length;
        

        // Define each quest
        quests = new Quest[] {
            new Quest() { ID = 0, description = "Find a bucket", completed = false },
            new Quest() { ID = 1, description = "Collect water with the bucket", completed = false },
            new Quest() { ID = 2, description = $"Extinguish the fire", completed = false },
            new Quest() { ID = 3, description = "Rescue the fox", completed = false },
            new Quest() { ID = 4, description = "try stop the lumberjack by stealing his axe ", completed = false },
            new Quest() { ID = 5, description = "try the forklift~~", completed = false },
            new Quest() { ID = 6, description = "Rescue the fox", completed = false }
            new Quest() { ID = 7, description = $"Collect all the trash {collectedTrash}/{trashCount}", completed = false, trashCount = trashCount, collectedTrashCount = 0 } 
        };
        
        foreach (Quest quest in quests)
        {
            DisplayQuest(quest);
        }
    }

      void Update()
    {
        // // Update the quest list UI
        // string questListText = "";
        // foreach (Quest quest in quests)
        // {
        //     string questText = quest.description;
        //     if (quest.completed)
        //     {
        //         questText = "<color=green>" + questText + "</color>";
        //     }
        //     questListText += "- " + questText + "\n";
        // }
        // questList.text = questListText;
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
    
    public void DisplayQuest(Quest quest)
    {
        questText.text += "- " + quest.description + "\n";
    }

    
    public void CompleteQuest(int id)
    {
        // Find the quest with the specified ID
    Quest quest = System.Array.Find(quests, q => q.ID == id);   



        if (quest != null)
        {
            // Mark the quest as completed
            quest.completed = true;
            quest.description = "<color=green>" + questText + "</color>";
            // Remove the quest from the UI after a delay
            Invoke("RemoveQuest", questDisplayDuration);
        }
    }
    private void RemoveQuest()
    {
        // Clear the quest text
        questText.text = "";

        // Display only the active quests
        foreach (Quest quest in quests)
        {
            if (!quest.completed)
            {
                DisplayQuest(quest);
            }
        }
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

