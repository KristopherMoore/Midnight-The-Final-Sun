using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    //The InputHandler class will be responsible for checking for user inputs and storing them, in addition it will be flexible
    //to user modification. The only exceptions to this will be the Horizontal and Vertical Axis which will be handled 
    //seperately within their respective classes. This handler will manage the enumarations of states, and be placed in states
    //given the respective bind.

    //Note this script is set to execute in the highest priority since it deals with player input managing. -100 behind Default 
    //script timings

    public static InputHandler Instance;

    private static int maxKeyBinds = 100;
    protected enum actions
    {
        jump, run, walk, aim, shoot
    }

    private KeyCode[] keybinds;
    private KeyCode[] altKeybinds;

	void Awake ()
    {
        Instance = this;

        //default keybinds
        keybinds = new KeyCode[maxKeyBinds];
        keybinds[0] = KeyCode.Space;
        keybinds[1] = KeyCode.LeftShift;
        keybinds[2] = KeyCode.Z;
        keybinds[3] = KeyCode.Mouse1;
        keybinds[4] = KeyCode.Mouse0;

        //default altKeybinds
        altKeybinds = new KeyCode[maxKeyBinds];
        altKeybinds[0] = KeyCode.Joystick1Button3;
        altKeybinds[1] = KeyCode.Joystick1Button1;
        altKeybinds[2] = KeyCode.Z; //unneeded for controller
        altKeybinds[3] = KeyCode.Ampersand; //This will need to be set to true for axis input of the 3rd axis, so when negative numbers on 3rd axis happen trigger this
        altKeybinds[4] = KeyCode.At; //same as above but for right trigger. So positive towrds 1 on 3rd axis

        //testingcode
        setNewBind(actions.aim);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    private void setNewBind(actions actionToModify)
    {
        //parse through the system's KeyCodes and check if we had that key get pressed, if so set the new keycode to our keybinds array
        foreach (KeyCode indexKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(indexKey))
                keybinds[(int)actionToModify] = indexKey;
        }
    }
    
}
