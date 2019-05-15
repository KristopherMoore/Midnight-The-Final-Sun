//Program Information///////////////////////////////////////////////////////////
/*
 * @file Player.cs
 *
 *
 * @game-version 0.72 
 *          Kristopher Moore (14 May 2019)
 *          Modifications to player, for use in SaveData
 *          
 *          This class will serve as the base for the Player Object, it will be seperate from the character controller and the camera. It will Hold player info such as
 *          HP, stamina, inventory, weapon, and armor. Anything to do with modifying the player object. it also inherits from Unit for HP handling generics
 *          NOTE: DO NOT make positional changes on this script. Motor functions are handled by the motor, and camera functions are handled by the camera/cam focus point controllers.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    //while we should only have one player at a given time, to support future implementation of multiplayer, ensure these are not static.
    public Player Instance;

    //ADD array of BUFFS,
    //ADD array of DEBUFFS,

    //Private for encapsulation
    private int strength;
    private int intellect;
    private int agility;
    private int endurance;
    private int faith;
    private int vigor;

    //objects to manipulate with player (armor, inventory, etc.)
    private Inventory inventory;
    private Weapon rightHandEquipped;
    private Weapon leftHandEquipped;
    private Armor armorArms;
    private Armor armorBody;
    private Armor armorHelm;
    private Armor armorLegs;

    // Game load
    void Awake()
    {
        //start instance, and defualt values, like a temp name
        Instance = this;
        this.setName("Player Class Awake");

        //TODO: read in Save info. and set our information accordingly, if any info is missing, our original defaults we stay in place (ie New Game status)
        

        inventory = new Inventory(this.getName()); //sending Player Object as owner name

    }


    //Equip Player with the item sent to us by name. DO NOT generate a new item here, check if in inventory and respond accordingly
    public void equipPlayer(string toEquip)
    {
        Item itemToEquip = inventory.findItem(toEquip);

        //if our item is null, therefore not in our inventory
        if (itemToEquip == null)
        {
            Debug.Log("PlayerObject.equipPlayer() cant equip as our inventory doesnt contain it");
            return;
        }

        //otherwise continue with equipping, also since we are finding the item, we will have its object type so can now
        //utilize the methods made earlier, although I modified them to be privately accessable helper functions.
        if (itemToEquip.GetType().Name == "Weapon")
        {
            equipPlayer((Weapon)itemToEquip);
        }

    }

    //Equip player with given weapon
    private void equipPlayer(Weapon toEquip)
    {
       
        rightHandEquipped = toEquip;
        Debug.Log(rightHandEquipped.getName() + " is Equipped");
        equipWeaponOnPlayerModel(rightHandEquipped.getName());

    }

    private void equipWeaponOnPlayerModel(string toEquip)
    {
        //create object from resources, instantiate it on the player (this script runs on the players base transform)
        GameObject itemToAdd = (GameObject)Resources.Load("Prefabs/" + toEquip);
        Instantiate(itemToAdd, this.transform);

        //remove excess copies
        HelperK.removeChild(this.transform, toEquip + "(Clone)");
    }


    //--------------------------------------------------------Getters and Setters, Encapsulation for private fields.---------------------------------------------------------/
    public float getSTR()
    {
        return strength;
    }
    public void setSTR(int toSet)
    {
        this.strength = toSet;
    }

    public float getINT()
    {
        return intellect;
    }
    public void setINT(int toSet)
    {
        this.intellect = toSet;
    }

    public float getAGI()
    {
        return agility;
    }
    public void setAGI(int toSet)
    {
        this.agility = toSet;
    }

    public float getEND()
    {
        return endurance;
    }
    public void setEND(int toSet)
    {
        this.endurance = toSet;
    }

    public float getFAI()
    {
        return faith;
    }
    public void setFAI(int toSet)
    {
        this.faith = toSet;
    }

    public float getVIG()
    {
        return vigor;
    }
    public void setVIG(int toSet)
    {
        this.vigor = toSet;
    }

    public Inventory getInventory()
    {
        return inventory;
    }
    public void setInventory(Inventory toSet)
    {
        inventory = toSet;
    }

    public Weapon getWeaponRight()
    {
        return rightHandEquipped;
    }
    public void setWeaponRight(Weapon toSet)
    {
        rightHandEquipped = toSet;
    }

    public Weapon getWeaponLeft()
    {
        return leftHandEquipped;
    }
    public void setWeaponleft(Weapon toSet)
    {
        leftHandEquipped = toSet;
    }

    public Armor getArmorArms()
    {
        return armorArms;
    }
    public void setArmorArms(Armor toSet)
    {
        armorArms = toSet;
    }

    public Armor getArmorBody()
    {
        return armorBody;
    }
    public void setArmorBody(Armor toSet)
    {
        armorBody = toSet;
    }

    public Armor getArmorHelm()
    {
        return armorHelm;
    }
    public void setArmorHelm(Armor toSet)
    {
        armorHelm = toSet;
    }

    public Armor getArmorLegs()
    {
        return armorLegs;
    }
    public void setArmorLegs(Armor toSet)
    {
        armorLegs = toSet;
    }





}
