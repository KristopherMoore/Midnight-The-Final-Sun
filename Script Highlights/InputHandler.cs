//Program Information///////////////////////////////////////////////////////////
/*
 * @file InputHandler.cs
 *
 *
 * @game-version 0.75 
 *          Kristopher Moore (17 May 2019)
 *          Addition to MENU option
 *          
 *          The InputHandler class will be responsible for checking for user inputs and storing them, in addition it will be flexible
 *          to user modification. The only exceptions to this will be the Horizontal and Vertical Axis which will be handled 
 *          seperately within their respective classes. This handler will manage the enumarations of states, and be placed in states
 *          given the respective bind.
 *
 *          NOTE: this script is set to execute in the highest priority since it deals with player input managing. -100 behind Default script timings
 *          
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    public static InputHandler Instance;

    private static int maxKeyBinds = 100;
    public enum actions
    {
        AIM, ROLL, RUN, JUMP, LIGHTATTACK, HEAVYATTACK, USEITEM, SWITCHSTANCE, CHANGECAM, MENU
    }

    private KeyCode[] keybinds;
    private KeyCode[] controllerbinds;

    //mouse input variables
    private bool cursorMoved;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float X_MouseSensitivity = 5f;
    private float Y_MouseSensitivity = 5f;
    private float deadZone = 0.19f;

    //model GetAxisDown with a boolean check
    private bool rightTriggerDown = false;


    void Awake ()
    {
        Instance = this;

        //default keybinds
        keybinds = new KeyCode[(int)actions.MENU + 1];
        keybinds[(int)actions.AIM] = KeyCode.F;
        keybinds[(int)actions.ROLL] = KeyCode.R;
        keybinds[(int)actions.RUN] = KeyCode.LeftShift;
        keybinds[(int)actions.JUMP] = KeyCode.Space;
        keybinds[(int)actions.LIGHTATTACK] = KeyCode.Mouse0;
        keybinds[(int)actions.HEAVYATTACK] = KeyCode.Mouse1;
        keybinds[(int)actions.USEITEM] = KeyCode.E;
        keybinds[(int)actions.SWITCHSTANCE] = KeyCode.T;
        keybinds[(int)actions.CHANGECAM] = KeyCode.U;
        keybinds[(int)actions.MENU] = KeyCode.Escape;

        //default altKeybinds
        controllerbinds = new KeyCode[(int)actions.MENU + 1];
        controllerbinds[(int)actions.AIM] = KeyCode.F; //NOTE: Left trigger is an axis, so if this shows we will check for it in CheckActions
        controllerbinds[(int)actions.ROLL] = KeyCode.JoystickButton1;
        controllerbinds[(int)actions.RUN] = KeyCode.JoystickButton8;
        controllerbinds[(int)actions.JUMP] = KeyCode.JoystickButton0;
        controllerbinds[(int)actions.LIGHTATTACK] = KeyCode.JoystickButton5;
        controllerbinds[(int)actions.HEAVYATTACK] = KeyCode.Mouse1; //NOTE: Right trigger is an axis, so if this shows we will check for it in CheckActions
        controllerbinds[(int)actions.USEITEM] = KeyCode.JoystickButton2;
        keybinds[(int)actions.SWITCHSTANCE] = KeyCode.Joystick1Button3;
        controllerbinds[(int)actions.CHANGECAM] = KeyCode.JoystickButton6;
        controllerbinds[(int)actions.MENU] = KeyCode.JoystickButton7;

        //testingcode
        setNewBind(actions.AIM);

        getPlayerInput();
    }

    private void Update()
    {
        //check if we had a release of rightTrigger
        if (rightTriggerDown == true && Input.GetAxis("RightTrigger") >= -0.1f)
            rightTriggerDown = false;
    }

    //allow exterior methods to check a desired action, based on an actions code.
    //returns relevant input data based on the corresponding KeyCodes arrays.
    public bool checkAction(actions action)
    {
        //handle specific action types that will have modifications. AIM / HEAVY, etc. That need Axis data
        if (action == actions.AIM)
        {
            //specifically GetKey to see if it is HELD down
            if (Input.GetKey(keybinds[(int)action]) || Input.GetAxis("LeftTrigger") < -0.1f)
                return true;
        }
        else if (action == actions.HEAVYATTACK)
        {
            //ensure we have a state of KeyDown, or our AxisDown emulation, then set accordingly
            if (Input.GetKeyDown(keybinds[(int)action]) || (rightTriggerDown == false && Input.GetAxisRaw("RightTrigger") < -0.1f ) )
            {
                rightTriggerDown = true;
                return true;
            }
        }
        else if(action == actions.RUN)
        {
            //specifically GetKey to see if it is HELD down
            if (Input.GetKey(keybinds[(int)action]) || Input.GetKey(controllerbinds[(int)action]))
                return true;
        }

        else if(Input.GetKeyDown(keybinds[(int)action]) || Input.GetKeyDown(controllerbinds[(int)action]))
            return true;

        //in any case not passing, return false. Removes redundant code.
        return false;
    }

    //method for handling player input and storing the values for 
    public void getPlayerInput()
    {
        //reset variables
        cursorMoved = false;

        //get Axis input
        mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;

        //check for controller input
        if (Input.GetAxis("RightJoyHorizontal") > deadZone || Input.GetAxis("RightJoyHorizontal") < -deadZone)
            mouseX += Input.GetAxis("RightJoyHorizontal") * X_MouseSensitivity;
        if (Input.GetAxis("RightJoyVertical") > deadZone || Input.GetAxis("RightJoyVertical") < -deadZone)
            mouseY -= Input.GetAxis("RightJoyVertical") * Y_MouseSensitivity;

        //check to see if the mouse has moved at all.
        if (Input.GetAxis("Mouse X") != 0)
            Instance.cursorMoved = true;
        if (Input.GetAxis("Mouse Y") != 0)
            Instance.cursorMoved = true;
    }

    public bool isCursorMoving()
    {
        return cursorMoved;
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
