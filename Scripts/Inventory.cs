using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    //This class will serve as the Inventory base for all unit's that have such things. Each individual will be responsible
    //for creating their own instance of an Inventory, this class simply will manage what can be done with them.

    //Can contain Items (Weapons, Armor, Consumables)

    private static List<Item> inventory = new List<Item>();
    private string owner; //owner name, mostly for debugging purposes

    private float totalWeight = 0.0f;

    //constructor with owner name
    public Inventory(string nameOfOwner)
    {
        owner = nameOfOwner;
    }

    public void addItem(Item itemToAdd)
    {
        inventory.Add(itemToAdd);
        totalWeight += itemToAdd.getWeight();
    }

    public void removeItem(Item itemToRemove)
    {
        inventory.Remove(itemToRemove);
        totalWeight -= itemToRemove.getWeight();
    }

    public void sortInventory()
    {
        inventory.Sort();
    }

    public void removeAll()
    {
        inventory.Clear();
        totalWeight = 0.0f;
    }
}
