//Program Information///////////////////////////////////////////////////////////
/*
 * @file GameHandler.cs
 *
 *
 * @game-version 0.75 
 *          Kristopher Moore (17 May 2019)
 *          Modified for reimplementation of Game Menu
 *          
 *          Responsible as a base for the handling of game critical actions. Will retrieve
 *          static information passed from the Main Menu (such as when a new character is created)
 *          and handle any relevant actions that need to occur from that.
 *          Additionally it holds dev actions like toggling main cursor state, etc.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The GameHandler is responsible for upkeeping a valid game state, serves to lock the state of the game if in a menu, and other such actions
public class GameHandler : MonoBehaviour {

    //Public itemMasterList, so other methods can utilize the single instance of the MasterList
    public static ItemsMasterList itemsMasterList = new ItemsMasterList();

    //Public fileHandler instance, for same reason
    public static FileHandler fileHandler = new FileHandler();

    private Player player;

    //when loading in
    void Awake ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.Find("Player").GetComponent<Player>();

        //check for MainMenu's Selections
        SaveInfo saveInfo = new SaveInfo();
        player.setName(MainMenuSelection.nameSelection);
        saveInfo.playerObject = player;
        saveInfo.spawnLocation = new Vector3(16f, 5, 0f);

        switch (MainMenuSelection.modeSelection)
        {
            
            case "PLAY":
                
                break;
            
            //handles creation of new game with character selected
            case "NEW GAME":
                fileHandler.SaveGame(saveInfo);

                break;

            //handles the loading of game, as requested from main menu
            case "LOAD GAME":
                saveInfo.playerObject = fileHandler.LoadGame(saveInfo);
                break;

            default:
                Debug.Log("ERROR in GameHandler, switch on mode selection");
                break;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {

        //dev action, kill game
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Application.Quit();
        }

        //dev action
        if (InputHandler.Instance.checkAction(InputHandler.actions.MENU) )
        {
            toggleCursor();
            GameMenu.Instance.changeMenuStatus();

        }

	}

    //toggleCursor action, to allow us to change the cursor state on calls
    private void toggleCursor()
    {
        //invert the cursor visibility
        Cursor.visible = !Cursor.visible;

        //decide to lock or unlock, since we cant invert enums
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
