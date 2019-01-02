using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item {

    void Awake()
    {

    }

    public Consumable(string name, float weight, float price)
    {
        this.setName(name);
        this.setWeight(weight);
        this.setPrice(price);
    }
}
