using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats {

    //This class will serve as a base for all unit types in the game.

    private float healthPoints;
    private float maxHP;
    private float stamina;
    private float maxStamina;
    private float mana;
    private float maxMana;

    private string unitName;

    //Default constructor
    public UnitStats()
    {
        this.DefaultValues();
    }

    //Constructor overload, name
    public UnitStats(string name)
    {
        this.DefaultValues();
        this.unitName = name;
    }

    //Method to handle HitPoint modification from damage or healing sources, negative for damage, positive for healing
    public void modifyHP(float modifyBy)
    {
        float HPcalc = getHP() + modifyBy;

        //if greater than 0, and less than or equal Max HP, good number and we can set. If below zero, set to zero. If neither than it must be higher than maxHP so set to maxHP
        if (HPcalc > 0 && HPcalc <= getMaxHP())
            setHP(HPcalc);
        else if (HPcalc < 0)
            setHP(0);
        else
            setHP(getMaxHP());
    }

    //Method handling Stamina modification, either generation or negation based on the neg/pos of number given. Clamps between 0 and max
    public void modifyStamina(float modifyBy)
    {
        float stamCalc = getStamina() + modifyBy;

        if (stamCalc > 0 && stamCalc <= getMaxStamina())
            setStamina(stamCalc);
        else if (stamCalc < 0)
            setStamina(0);
        else
            setStamina(getMaxStamina());
    }

    //Method handling Mana modification, generation or negation. Clamps between 0 and max based on number modified by
    public void modifyMana(float modifyBy)
    {
        float manaCalc = getMana() + modifyBy;

        if (manaCalc > 0 && manaCalc <= getMaxMana())
            setMana(manaCalc);
        else if (manaCalc < 0)
            setMana(0);
        else
            setMana(getMaxMana());
    }

    //only to be used within constructors for initialization. Saving repeatable code.
    private void DefaultValues()
    {
        setHP(90f);
        setMaxHP(100f);
        setStamina(10f);
        setMaxStamina(10f);
        setMana(30f);
        setMaxMana(30f);
        this.unitName = "Default";
    }





    //Getters and Setters, Encapsulation for private fields.
    public float getHP()
    {
        return healthPoints;
    }
    public void setHP(float toSet)
    {
        this.healthPoints = toSet;
    }


    public float getMaxHP()
    {
        return maxHP;
    }
    public void setMaxHP(float toSet)
    {
        this.maxHP = toSet;
    }


    public float getStamina()
    {
        return stamina;
    }
    public void setStamina(float toSet)
    {
        this.stamina = toSet;
    }


    public float getMaxStamina()
    {
        return maxStamina;
    }
    public void setMaxStamina(float toSet)
    {
        this.maxStamina = toSet;
    }


    public float getMana()
    {
        return mana;
    }
    public void setMana(float toSet)
    {
        this.mana = toSet;
    }


    public float getMaxMana()
    {
        return maxMana;
    }
    public void setMaxMana(float toSet)
    {
        this.maxMana = toSet;
    }

}
