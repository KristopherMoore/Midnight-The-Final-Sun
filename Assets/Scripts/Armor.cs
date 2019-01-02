using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item {

    void Awake()
    {

    }

    public Armor(string name, float weight, float price)
    {
        this.setName(name);
        this.setWeight(weight);
        this.setPrice(price);
    }
}
