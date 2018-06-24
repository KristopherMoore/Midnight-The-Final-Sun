using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    //This class will serve as the Inventory base for all unit's that have such things. Each individual will be responsible
    //for creating their own instance of an Inventory, this class simply will manage what can be done with them.

    //Can contain Items (Weapons, Armor, Consumables)

    private static List<Item> inventory;
    private string owner; //owner name, mostly for debugging purposes

    //constructor with owner name
    public Inventory(string nameOfOwner)
    {
        owner = nameOfOwner;
    }

    public void addItem(Item itemToAdd)
    {
        inventory.Add(itemToAdd);
    }

    public void removeItem(Item itemToRemove)
    {
        inventory.Remove(itemToRemove);
    }

    public void sortInventory()
    {
        inventory.Sort();
    }

    public void removeAll()
    {
        inventory.Clear();
    }
}
