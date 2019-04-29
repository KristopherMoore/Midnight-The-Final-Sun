using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The GameHandler is responsible for upkeeping a valid game state, serves to lock the state of the game if in a menu, and other such actions
public class GameHandler : MonoBehaviour {

    //Public itemMasterList, so other methods can utilize the single instance of the MasterList
    public static ItemsMasterList itemsMasterList = new ItemsMasterList();

    //when loading in
    void Awake ()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //TODO: TEMP REMOVE OF GAME MENU LOCK
        //if (GameMenu.Instance.getMenuStatus() == true) //if we are currently in a menu, remove control of character, by ending this update run
        //return;

        //dev action, kill game
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Application.Quit();
        }

        //dev action
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //toggleCursor();
            //GameMenu.Instance.changeMenuStatus();


            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //ensure that the player cursor can be relocked.
            toggleCursor();
        }

        //testing for controller input
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            Debug.Log("Controller input");

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
