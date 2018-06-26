using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {

    //class to handle the game state, holding key game details like save states, a masterList of all items. Etc.

    //Public itemMasterList, so other methods can utilize the single instance of the MasterList
    public static ItemsMasterList itemsMasterList = new ItemsMasterList();

    //when loading in
    void Awake ()
    {
        lockCursor();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //dev action
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
	}

    private void lockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
