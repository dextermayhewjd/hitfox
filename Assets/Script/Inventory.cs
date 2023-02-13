using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    private List<Item> itemList;

    public Inventory() {
        itemList = new List<Item>();
        AddItem(new Item {itemType = Item.ItemType.PlaceholderItem, amount = 1});

        Debug.Log("Inventory set up");
        Debug.Log(itemList.Count);
    }

    public void AddItem(Item item) {
        itemList.Add(item);
        Debug.Log("got item");
    }

    public int NumberOfItems(Item.ItemType itemType) {
        foreach (Item item in itemList) {
            if(item.itemType == itemType && item.amount > 0) {
                return item.amount;
            }
        }

        return 0;
    }

}
