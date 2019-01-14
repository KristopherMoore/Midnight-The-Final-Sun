using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    //This class will serve as the base for the Player Object, it will be seperate from the character controller and the camera. It will Hold player info such as
    //HP, stamina, inventory, weapon, and armor. Anything to do with modifying the player object. it also inherits from Unit for HP handling generics
    //NOTE: DO NOT make positional changes on this script. Motor functions are handled by the motor, and camera functions are handled by the camera/cam focus point controllers.

    //while we should only have one player at a given time, to support future implementation of multiplayer, ensure these are not static.
    public Player Instance;

    //ADD array of BUFFS,
    //ADD array of DEBUFFS,

    private int strength;
    private int intellect;
    private int agility;

    //objects to manipulate with player (armor, inventory, etc.)
    private Inventory inventory;
    private Weapon rightHandEquipped;
    private Weapon leftHandEquipped;
    private Armor armorBody;
    private Armor armorArms;

    // Game load
    void Awake()
    {
        //start instance, and defualt values, like a temp name
        Instance = this;
        this.setName("Player Class Awake");

        //read in Save info. and set our information accordingly, if any info is missing, our original defaults we stay in place (ie New Game status)
        getSaveInfo();

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

    public Inventory getInventoryObject()
    {
        return inventory;
    }

    private void getSaveInfo()
    {

    }

}
