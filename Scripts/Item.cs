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

    //base constructor, will intialization values
    public Item()
    {
        setName("");
        setWeight(0f);
        setPrice(0f);
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
