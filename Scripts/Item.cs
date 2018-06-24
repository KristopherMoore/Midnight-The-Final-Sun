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

    //These indexed lists will be used similarly to multiDimensionsal Arrays, but with these indexed lists
    //The idea being that after these are populated on load, we can be given an Item's Name. Find its index, then
    // have all other information available for our factory to create the apporiate item within the game.
    private List<string> itemListNames;
    private List<string> itemListTypes;
    private List<float> itemListWeights;
    private List<float> itemListPrices;

    private Dictionary<int, string> itemDictionary;

    void Awake()
    {
        populateItemLists();
    }

    //base constructor, will intialization values
    public Item()
    {
        setName("");
        setWeight(0f);
        setPrice(0f);
    }

    //class responsible for constructing an item from the name sent, can be Weapon, Armor, or Consumable
    public Item itemFactory(string itemName)
    {
        Item toReturn;

        //find the index of our item by Name
        int indexOf = itemListNames.IndexOf(itemName);

        //if our item is a Weapon, construct weapon class
        if (itemListTypes[indexOf] == "Weapon")
            return toReturn = new Weapon(itemListNames[indexOf], itemListWeights[indexOf], itemListPrices[indexOf]);

        //if our item is a Consumable
        else if (itemListTypes[indexOf] == "Consumable")
            return toReturn = new Consumable(itemListNames[indexOf], itemListWeights[indexOf], itemListPrices[indexOf]);

        //if our item is armor
        else if (itemListTypes[indexOf] == "Armor")
            return toReturn = new Armor(itemListNames[indexOf], itemListWeights[indexOf], itemListPrices[indexOf]);

        return toReturn = new Item(); //satify compiler, we should never get here
    }

    public void populateItemLists()
    {
        //instantiation of lists
        itemListNames = new List<string>();
        itemListTypes = new List<string>();
        itemListWeights = new List<float>();
        itemListPrices = new List<float>();

        //now build up every item, for now it will be manual. TODO: Modify this to pull from a file containing the data, and have a coroutine loop fill
        itemListNames.Add("Bastard Sword");
        itemListTypes.Add("Weapon");
        itemListWeights.Add(5.0f);
        itemListPrices.Add(10000f);

        itemListNames.Add("Katana");
        itemListTypes.Add("Weapon");
        itemListWeights.Add(2.0f);
        itemListPrices.Add(8000f);
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
