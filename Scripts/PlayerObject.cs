using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour {

    //This class will serve as the base for the Player Object, it will be seperate from the Player Controller
    //but can be Utilized by such a class as it will hold the Instances of Player Stats, Inventories, and the like.
    public static PlayerObject Instance;

    private string playerName;
    private PlayerStats playerStats;
    private Inventory inventory;
    private Weapon rightHandEquipped;
    private Weapon leftHandEquipped;
    private Armor armorHead;
    private Armor armorBody;
    private Armor armorLegs;
    private Armor armorArms;
    private Item itemEquipped;

    private Item itemReference; //so we can utilize item methods

	// Game load
	void Awake ()
    {
        Instance = this;
        playerName = "Player Object Awake Instance";
        playerStats = new PlayerStats();
        inventory = new Inventory(playerName); //sending Player Object as owner name
        itemReference = new Item();

        //test add item
        addItemToPlayerInventory("Bastard Sword", GameHandler.itemsMasterList);

	}

    //Equip Player with the item sent to us by name. DO NOT generate a new item here, check if in inventory and respond accordingly
    public void equipPlayer(string toEquip)
    {
        Item itemToEquip = inventory.findItem(toEquip);
        Debug.Log(itemToEquip.getName());

        //if our item is null, therefore not in our inventory
        if (itemToEquip == null)
        {
            Debug.Log("PlayerObject.equipPlayer() cant equip as our inventory doesnt contain it");
            return;
        }

        //otherwise continue with equipping, also since we are finding the item, we will have its object type so can now
        //utilize the methods made earlier, although I modified them to be privately accessable helper functions.
        if (itemToEquip.GetType().Name == "Weapon")
            equipPlayer((Weapon)itemToEquip);

    }

    //Equip player with given weapon
    private void equipPlayer(Weapon toEquip)
    {
        rightHandEquipped = toEquip;
        Debug.Log(rightHandEquipped.getName() + " is Equipped");
    }

    private void equipPlayer(Armor toEquip, int armorSlot)
    {
        //check which armorSlot based on our given value, 0: Head, 1: Body, 2:Legs, 3:Arms
        switch(armorSlot)
        {
            case 0:
                armorHead = toEquip;
                break;
            case 1:
                armorBody = toEquip;
                break;
            case 2:
                armorLegs = toEquip;
                break;
            case 3:
                armorArms = toEquip;
                break;
            default:
                break;
        }
    }

    public void addItemToPlayerInventory(string itemName, ItemsMasterList itemsMasterList)
    {
        //adds item to our inventory, gets item by calling to the itemFactory, which will construct our item to add.
        //this allows us to add items, by only knowing their name, it will be populated.
        inventory.addItem(itemReference.itemFactory(itemName, itemsMasterList));
    }


    public string getPlayerName()
    {
        return playerName;
    }

    public Inventory getInventoryObject()
    {
        return inventory;
    }

    /////////////////////////////////////Getter methods for equipped Gear//////////////////////////////////////////////
    public Weapon getRightHandWeapon()
    {
        return rightHandEquipped;
    }

    public Weapon getLeftHandWeapon()
    {
        return leftHandEquipped;
    }

    public Armor getArmorHead()
    {
        return armorHead;
    }

    public Armor getArmorBody()
    {
        return armorBody;
    }

    public Armor getArmorLegs()
    {
        return armorLegs;
    }

    public Armor getArmorArms()
    {
        return armorArms;
    }

}
