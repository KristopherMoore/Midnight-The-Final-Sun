using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{

    public static CharacterController CharacterController;
    public static PlayerCharacterController Instance; //hold reference to current instance of itself

    public enum animationState
    {
        Idling, Walking, Jogging, Running, Rolling, Stagger, Stunned
    }
    public animationState playerAnimationState { get; set; }

    public enum direction
    {
        Stationary, Forward, Left, Backward, Right, LeftFwd, RightFwd, LeftBck, RightBck
    }
    public direction MoveDirection { get; set; }

    public float deadZone = 0.01f;

    public bool isMoving;
    public bool isAnimationLocked;

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;  
    }



    //
    void Update()
    {
        if (Camera.main == null) //check for camera, if no main camera, stop.
            return;

        //handle actions before locomotion, since actions will at times lock the player into place
        HandleActionInput();

        //if we arent locked in an animation, continue with locomotion.
        if(!this.isAnimationLocked)
        {
            GetLocomotionInput();
        }

        //update the motor even if we didnt have locomotion, as the motor also calculates things like gravity.
        PlayerCharacterMotor.Instance.UpdateMotor();

    }

    void GetLocomotionInput()
    {
        Instance.isMoving = false;
        Instance.isAnimationLocked = false;

        PlayerCharacterMotor.Instance.verticalVelocity = PlayerCharacterMotor.Instance.MoveVector.y;

        PlayerCharacterMotor.Instance.MoveVector = Vector3.zero; //reclacuate, prevents it from being additive. Basically restart on every update

        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //if we are animation locked, we will ignore axis data
        if(!Instance.isAnimationLocked)
        {
            //check if input exceeds the deadZone (check for a minimal amount of input before moving)
            if (yAxis > deadZone || yAxis < -deadZone)
            {
                Instance.isMoving = true;
                PlayerCharacterMotor.Instance.MoveVector += new Vector3(0, 0, yAxis);
            }
            if (xAxis > deadZone || xAxis < -deadZone)
            {
                Instance.isMoving = true;
                PlayerCharacterMotor.Instance.MoveVector += new Vector3(xAxis, 0, 0);
            }
        }

    }

    void HandleActionInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            playerAnimationState = animationState.Jogging;
            if (Input.GetKey(KeyCode.LeftShift))
                playerAnimationState = animationState.Running;
            else if (Input.GetKey(KeyCode.Z))
                playerAnimationState = animationState.Walking;
        }
        else
            playerAnimationState = animationState.Idling;
        //rolling action
        if (Input.GetKey(KeyCode.Space))
        {
            isAnimationLocked = true;
            playerAnimationState = animationState.Rolling;
            
            //TODO:
            //Corroutine here to wait for rolling to finish, then when done set isAnimationLocked = false; Encapsulate Handle Action
            //Inputs inside of another check for isAnimationLocked, that way any prior engaged animations are messed with until co
            //routine finishes.
            //StartCoroutine(RollingWait());
        }
        //test case for animation locks
        if (Input.GetKey(KeyCode.E))
            isAnimationLocked = true;
        if (Input.GetKey(KeyCode.R))
            isAnimationLocked = false;
    }

}