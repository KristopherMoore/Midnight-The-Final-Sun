using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //This class will serve as the base for the Player Object, it will be seperate from the character controller and the camera. It will Hold player info such as
    //HP, stamina, inventory, weapon, and armor. Anything to do with modifying the player object.
    //NOTE: DO NOT make positional changes on this script. Motor functions are handled by the motor, and camera functions are handled by the camera/cam focus point controllers.

    //while we should only have one player at a given time, to support future implementation of multiplayer, ensure these are not static.
    public Player Instance;

    //player information
    private string playerName = "";
    private bool isAlive = true;
    private float health = 1;
    private float stamina = 1;
    private float damageResistance = 0;

    //player dot information, how much dotDamage to take, and how long we take it for.
    private float dotDamage;
    private float dotTime;

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
        //start instance, and defualt values, like player being alive and a temp name
        Instance = this;
        playerName = "Player Awake Instance";
        isAlive = true;

        //read in Save info. and set our information accordingly, if any info is missing, our original defaults we stay in place (ie New Game status)
        getSaveInfo();

        inventory = new Inventory(playerName); //sending Player Object as owner name

        //test add item // TODO: implement a load from save functionality for the Inventory
        //addItemToPlayerInventory("Bastard Sword", GameHandler.itemsMasterList);
        //addItemToPlayerInventory("Katana", GameHandler.itemsMasterList);
        //addItemToPlayerInventory("Dagger", GameHandler.itemsMasterList);
        //addItemToPlayerInventory("Quiver", GameHandler.itemsMasterList);
        //addItemToPlayerInventory("Shield", GameHandler.itemsMasterList);
        //addItemToPlayerInventory("Recurve Bow", GameHandler.itemsMasterList);

    }

    //on each frame update cycle
    void Update()
    {
        if (dotTime > 0)
            calcDamageOverTime();
    }

    private void adjustHP(float amount)
    {
        health = health + amount;
        if (health <= 0)
            isAlive = false;
    }

    //public method so exterior scripts can damage the player.
    public void takeDamage(float amount)
    {

    }
    //method to continue damageing our character over time
    private void calcDamageOverTime()
    {
        //here we make sure to negate the calc'd damage, for our adjust method, if we want to subtract HP we send in a negative
        adjustHP(-(dotDamage / dotTime));
        dotTime = dotTime - Time.deltaTime;
        if (dotTime < 0)
            dotTime = 0;
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

    public string getPlayerName()
    {
        return playerName;
    }

    public Inventory getInventoryObject()
    {
        return inventory;
    }

    private void getSaveInfo()
    {

    }

}
