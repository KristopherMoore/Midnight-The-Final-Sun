using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour {

    //This class will serve as a base for all unit types in the game.

    private float healthPoints;
    private float stamina;
    private float mana;

    private string unitName;

    //Default constructor
    public UnitStats()
    {
        this.healthPoints = 100f;
        this.stamina = 10f;
        this.mana = 30f;
    }

	// Use this for initialization
	void Start ()
    {
		
	}

    public float getHP()
    {
        return healthPoints;
    }

    public void setHP(float toSet)
    {
        this.healthPoints = toSet;
    }
}
