using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour {

    //This class will serve as the base for the Player Object, it will be seperate from the Player Controller
    //but can be Utilized by such a class as it will hold the Instances of Player Stats, Inventories, and the like.
    public static PlayerObject Instance;

    private PlayerStats playerStats;
    private Inventory inventory;

    private Item itemReference; //so we can utilize item methods

	// Game load
	void Awake ()
    {
        Instance = this;
        playerStats = new PlayerStats();
        inventory = new Inventory("Player Object");
	}

    public void addItemToPlayer(string itemName)
    {
        //adds item to our inventory, gets item by calling to the itemFactory, which will construct our item to add.
        //this allows us to add items, by only knowing their name, it will be populated.
        inventory.addItem(itemReference.itemFactory("itemName"));
    }

}
