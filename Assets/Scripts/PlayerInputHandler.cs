using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {

    //class responsible for watching for player inputs and sending off for movement, animations, and events
    //also houses the system for reading current keybinding settings from the .ini file

    public static PlayerInputHandler Instance;

    private KeyCode[] keybinds = { KeyCode.Space, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.E,
                                    KeyCode.Q, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3};
    private string[] actions = {"Jump", "MoveForward", "MoveBackward", "MoveLeft", "MoveRight", "r1", "r2", "l1",
                                 "l2", "Wep1Switch", "Wep2Switch", "Wep3Switch"};

    void Awake()
    {
        Instance = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    //will be called externally by the KeybindChanging method, to let them know keybinds have been changed, and to reload them for the InputHandler to be correct
    public void ReloadKeybinds()
    {
        ReadFileAndSetKeybinds();
    }

    private void ReadFileAndSetKeybinds()
    {
        string readString = "";

    }

    private void WriteToFile()
    {

    }
}
