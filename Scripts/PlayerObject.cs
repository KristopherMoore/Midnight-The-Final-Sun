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

        //test add item // TODO: implement a load from save functionality for the Inventory
        addItemToPlayerInventory("Bastard Sword", GameHandler.itemsMasterList);
        addItemToPlayerInventory("Katana", GameHandler.itemsMasterList);
        addItemToPlayerInventory("Dagger", GameHandler.itemsMasterList);
        addItemToPlayerInventory("Quiver", GameHandler.itemsMasterList);
        addItemToPlayerInventory("Shield", GameHandler.itemsMasterList);
        addItemToPlayerInventory("Recurve Bow", GameHandler.itemsMasterList);

    }

    //TODO: Clean up all these equip functions.

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
            if (itemToEquip.getName().Contains("Shield") || itemToEquip.getName().Contains("Bow"))
                equipPlayer((Weapon)itemToEquip, true);
            else
                equipPlayer((Weapon)itemToEquip, false);
        }

    }

    //Equip player with given weapon
    private void equipPlayer(Weapon toEquip, bool isShield)
    {
        if (isShield == false)
        {
            rightHandEquipped = toEquip;
            Debug.Log(rightHandEquipped.getName() + " is Equipped");
            equipWeaponOnPlayerModel(rightHandEquipped.getName(), isShield);
        }
        else
        {
            leftHandEquipped = toEquip;
            Debug.Log(leftHandEquipped.getName() + " is Equipped");
            equipWeaponOnPlayerModel(leftHandEquipped.getName(), isShield);
        }
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

    private void equipWeaponOnPlayerModel(string toEquip, bool isShield)
    {
        //create object from resources, instantiate it on the player (this script runs on the players base transform)
        GameObject itemToAdd = (GameObject)Resources.Load("Prefabs/" + toEquip);
        Instantiate(itemToAdd, this.transform);

        //attach the weapon to the proper hand, adding in clone to compensate for Unity adding clone to prefabs on awake
        if(isShield == false)
            attachWeaponToRightHand(toEquip + "(Clone)");
        else
            attachWeaponToLeftHand(toEquip + "(Clone)");

        //remove excess copies
        HelperK.removeChild(this.transform, toEquip + "(Clone)");
    }

    //method that will attachWeapons to the right hand player bone.
    private void attachWeaponToRightHand(string weaponName)
    {
        //Find the weapon object on the Player Character
        //Transform weapon = this.transform.Find(weaponName);
        Transform weapon = HelperK.FindSearchAllChildren(this.transform, weaponName);

        //Find the rWeaponBone of the player character. Searching through all children.
        Transform rWeaponBone = HelperK.FindSearchAllChildren(this.transform, "R_Weapon");

        //Remove any other children from this WeaponBone
        for (int i = 0; i < rWeaponBone.childCount; i++)
            Destroy(rWeaponBone.GetChild(i).gameObject);

        try
        {
            //make the weapon a child of the rWeaponBone, that way it will follow it in all its animations. And place its transform at the handbone location
            weapon.transform.parent = rWeaponBone;
            weapon.transform.SetPositionAndRotation(rWeaponBone.position, rWeaponBone.rotation);

            //compensating for our model rips base rotation being 180degrees off,
            weapon.transform.Rotate(weapon.transform.rotation.x, weapon.transform.rotation.y, weapon.transform.rotation.z + 180);
        }
        catch (MissingComponentException ex)
        {
            Debug.Log("Throwing Null Exception");
        }
    }

    //method that will attachWeapons to the left hand player bone.
    private void attachWeaponToLeftHand(string weaponName)
    {
        //Find the weapon object on the Player Character
        Transform weapon = HelperK.FindSearchAllChildren(this.transform, weaponName);

        //Find the lWeaponBone of the player character. Searching through all children.
        Transform lWeaponBone = HelperK.FindSearchAllChildren(this.transform, "L_Weapon");

        //Remove any other children from this WeaponBone
        for (int i = 0; i < lWeaponBone.childCount; i++)
            Destroy(lWeaponBone.GetChild(i).gameObject);

        try
        {
            //make the weapon a child of the lWeaponBone, that way it will follow it in all its animations. And place its transform at the handbone location
            weapon.transform.parent = lWeaponBone;
            weapon.transform.SetPositionAndRotation(lWeaponBone.position, lWeaponBone.rotation);

            //compensating for our model rips base rotation being 180degrees off, which extra y rotation due to it being in the opposite of dominant hand
            weapon.transform.Rotate(weapon.transform.rotation.x, weapon.transform.rotation.y + 180, weapon.transform.rotation.z + 180);

            //if a bow do not add an additional base rotation
            if(weapon.name.Contains("Bow"))
                weapon.transform.Rotate(weapon.transform.rotation.x, weapon.transform.rotation.y, weapon.transform.rotation.z + 180);
        }
        catch (MissingComponentException ex)
        {
            Debug.Log("Throwing Null Exception");
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
