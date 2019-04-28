using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//This class will serve as a base for all unit types in the game. Handling health, stamina, life state, and their rigidbody
public class Unit : MonoBehaviour {

    private float health;
    private float maxHP;
    private float stamina;
    private float maxStamina;
    private float damageResistance;
    private float maxDR;

    private string unitName;
    private bool isAlive = true;

    //rigidbody components (IE, Ragdoll bones)
    private Rigidbody[] rigidbodyArray;

    //Default constructor
    public Unit()
    {
        this.DefaultValues();
    }

    //Constructor overload, for named units
    public Unit(string name)
    {
        this.DefaultValues();
        this.unitName = name;
    }

    private void Awake()
    {
        this.findRigidbodyBones(this.transform.root);
        setRigidbody(false);
    }

    //Method to handle HitPoint modification from damage or healing sources, negative for damage, positive for healing
    public void modifyHP(float modifyBy)
    {
        float HPcalc = getHP() + modifyBy;

        //if greater than 0, and less than or equal Max HP, good number and we can set. If below zero, set to zero. If neither than it must be higher than maxHP so set to maxHP
        if (HPcalc > 0 && HPcalc <= getMaxHP())
            setHP(HPcalc);
        else if (HPcalc < 0)
        {
            setHP(0);
            isAlive = false;
            setRigidbody(true);
            //turn off animator / character controller / navmesh agent, if present
            transform.root.GetComponent<Animator>().enabled = false;
            if(transform.root.GetComponent<EnemyCharacterController>() != null)
                transform.root.GetComponent<EnemyCharacterController>().enabled = false;
            if (transform.root.GetComponent<NavMeshAgent>() != null)
                transform.root.GetComponent<NavMeshAgent>().enabled = false;
        }
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
    public void modifyDR(float modifyBy)
    {
        float manaCalc = getDamageResistance() + modifyBy;

        if (manaCalc > 0 && manaCalc <= getMaxDR())
            setDamageResistance(manaCalc);
        else if (manaCalc < 0)
            setDamageResistance(0);
        else
            setDamageResistance(getMaxDR());
    }

    //only to be used within constructors for initialization. Saving repeatable code.
    private void DefaultValues()
    {
        setHP(92f);
        setMaxHP(100f);
        setStamina(10f);
        setMaxStamina(10f);
        setDamageResistance(0f);
        setMaxDR(100f);
        this.unitName = "Default";
    }

    //public return for object alive status
    public bool isUnitAlive()
    {
        return isAlive;
    }

    //private method to search through the transform and pick up any and all rigibodies (which we are using for the ragdoll)
    private void findRigidbodyBones(Transform objRootTransform)
    {
        rigidbodyArray = GetComponentsInChildren<Rigidbody>();
    }
    //private method to either set the rigibody "ON" or "OFF", ON will be when the gravity is off, and the body isKinematic. OFF is the opposite
    private void setRigidbody(bool setStatus)
    {
        for(int i = 0; i < rigidbodyArray.Length; i++)
        {
            //because these are opposites of one another, ie gravity is OFF when we are Kinematic, we utilize the negation properties when setting
            rigidbodyArray[i].isKinematic = !setStatus;
            rigidbodyArray[i].useGravity = setStatus;
        }
    }


    //--------------------------------------------------------Getters and Setters, Encapsulation for private fields.---------------------------------------------------------/
    public float getHP()
    {
        return health;
    }
    public void setHP(float toSet)
    {
        this.health = toSet;
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


    public float getDamageResistance()
    {
        return damageResistance;
    }
    public void setDamageResistance(float toSet)
    {
        this.damageResistance = toSet;
    }


    public float getMaxDR()
    {
        return maxDR;
    }
    public void setMaxDR(float toSet)
    {
        this.maxDR = toSet;
    }

    public string getName()
    {
        return unitName;
    }
    public void setName(string toSet)
    {
        unitName = toSet;
    }
}
