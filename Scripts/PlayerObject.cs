using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour {

    //This class will serve as the base for the Player Object, it will be seperate from the Player Controller
    //but can be Utilized by such a class as it will hold the Instances of Player Stats, Inventories, and the like.
    public static PlayerObject Instance;

    private PlayerStats playerStats;
    private Inventory inventory;

    

	// Game load
	void Awake ()
    {
        Instance = this;
        playerStats = new PlayerStats();
        inventory = new Inventory("Player Object");
	}

}
