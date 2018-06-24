using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	void Awake()
    {

    }

    public Weapon(string name, float weight, float price)
    {
        this.setName(name);
        this.setWeight(weight);
        this.setPrice(price);
    }
}
