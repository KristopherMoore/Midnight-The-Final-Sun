using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    //This class serves as the base for items that can be utilized in game
    //Weapons, Armor, and Consumables will all dervice from this base.
    //Important to have the base because our Inventory will be able to store itself base on the Item parent objects and utilize
    //polymorphism to its advantage

    private string name;
    private float weight;
    private float price;

    void Awake()
    {
        
    }

    //base constructor, will intialization values
    public Item()
    {
        setName("");
        setWeight(0f);
        setPrice(0f);
    }

    //class responsible for constructing an item from the name sent, can be Weapon, Armor, or Consumable
    public Item itemFactory(string itemName, ItemsMasterList itemMasterList)
    {
        Item toReturn;

        //find the index of our item by Name
        int indexOf = itemMasterList.itemListNames.IndexOf(itemName);

        if(indexOf == -1)
        {
            Debug.Log("ITEM.itemFactory, item was not found in list");
            return null;
        }

        //Debug.Log(itemMasterList);
        //Debug.Log(itemMasterList.itemListNames[0]);
        //Debug.Log(itemMasterList.itemListNames[1]);
        //Debug.Log(indexOf);

        //if our item is a Weapon, construct weapon class
        if (itemMasterList.itemListTypes[indexOf] == "Weapon")
            return toReturn = new Weapon(itemMasterList.itemListNames[indexOf], itemMasterList.itemListWeights[indexOf], itemMasterList.itemListPrices[indexOf]);

        //if our item is a Consumable
        else if (itemMasterList.itemListTypes[indexOf] == "Consumable")
            return toReturn = new Consumable(itemMasterList.itemListNames[indexOf], itemMasterList.itemListWeights[indexOf], itemMasterList.itemListPrices[indexOf]);

        //if our item is armor
        else if (itemMasterList.itemListTypes[indexOf] == "Armor")
            return toReturn = new Armor(itemMasterList.itemListNames[indexOf], itemMasterList.itemListWeights[indexOf], itemMasterList.itemListPrices[indexOf]);

        return toReturn = new Item(); //satisfy compiler, we should never get here
    }

    //Getters and Setters, encapsulation of private methods ///////////////////////////////////////////////
    public string getName()
    {
        return this.name;
    }
    public void setName(string toSet)
    {
        this.name = toSet;
    }

    public float getWeight()
    {
        return this.weight;
    }
    public void setWeight(float toSet)
    {
        this.weight = toSet;
    }

    public float getPrice()
    {
        return this.price;
    }
    public void setPrice(float toSet)
    {
        this.price = toSet;
    }
}
