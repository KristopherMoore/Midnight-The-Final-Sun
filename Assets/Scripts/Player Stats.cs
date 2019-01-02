using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : UnitStats {

    protected int Vigor;
    protected int Endurance;
    protected int Vitality;
    protected int Strength;
    protected int Dexterity;
    protected int Intellect;
    protected int Faith;

    // run on game load, initialize with default values
    void Awake ()
    {
        this.setStats(1,1,1,1,1,1,1,1,1,1);
	}

    //method to be called when loading a game state, takes in the players saved stats
    public void setStats(float HP, float stam, float mana, int Vig, int End, int Vit, int Str, int Dex, int Int, int Fai)
    {
        //TODO: Finish adding rest of stats to load, including max HP, max Stam, and Max Mana
        this.setHP(HP);
    }

    //method to check if player is dead. returns true for dead, and false for alive.
    public bool checkIfDead()
    {
        if (this.getHP() > 0)
            return false;
        else
            return true;
    }

   



    //Getters and Setters for Player Stats Class
    public int getVigor()
    {
        return this.Vigor;
    }
    public void setVigor(int toSet)
    {
        this.Vigor = toSet;
    }

    public int getEndurance()
    {
        return this.Endurance;
    }
    public void setEndurance(int toSet)
    {
        this.Endurance = toSet;
    }

    public int getVitality()
    {
        return this.Vitality;
    }
    public void setVitality(int toSet)
    {
        this.Vitality = toSet;
    }

    public int getStrength()
    {
        return this.Strength;
    }
    public void setStrength(int toSet)
    {
        this.Strength = toSet;
    }

    public int getDexterity()
    {
        return this.Dexterity;
    }
    public void setDexterity(int toSet)
    {
        this.Dexterity = toSet;
    }

    public int getIntellect()
    {
        return this.Intellect;
    }
    public void setItellect(int toSet)
    {
        this.Intellect = toSet;
    }

    public int getFaith()
    {
        return this.Faith;
    }
    public void setFaith(int toSet)
    {
        this.Faith = toSet;
    }

}
