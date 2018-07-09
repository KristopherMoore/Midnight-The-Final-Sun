using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{

    public static CharacterController CharacterController;
    public static PlayerCharacterController Instance; //hold reference to current instance of itself

    public enum animationState
    {
        Idling, Walking, Jogging, Running, Rolling, Stagger, Stunned, L1Attack, L1AttackC2, Jumping, Gliding
    }
    public animationState playerAnimationState { get; set; }

    private int currentAttackChain = 0;
    private int maxAttackChain = 3;

    public enum direction
    {
        Stationary, Forward, Left, Backward, Right, LeftFwd, RightFwd, LeftBck, RightBck
    }
    public direction MoveDirection { get; set; }

    public float deadZone = 0.01f;

    public bool isMoving;
    public bool isAnimationLocked;
    public bool isJumping = false;
    public bool isGliding = false;

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;  
    }

    
    void Update()
    {
        if (Camera.main == null) //check for main camera, if none exists, exit.
            return;

        if (GameMenu.Instance.getMenuStatus() == true) //if we are currently in a menu, remove control of character, by ending this update run
            return;

        //handle actions before locomotion, since actions will at times lock the player into place
        HandleActionInput();

        //if we arent locked in an animation, continue with locomotion.
        if(!this.isAnimationLocked)
        {
            GetLocomotionInput();
        }

        //handling jumpstate after locomotion specifically. Unlike the other actions, this applies directly into the locomotion processes
        //and if it came before it would be overrident by the verticalVelocity check in GetLocomotionInput
        HandleJumpInput();

        //update the motor even if we didnt have locomotion, as the motor also calculates things like gravity.
        PlayerCharacterMotor.Instance.UpdateMotor();

    }

    void GetLocomotionInput()
    {
        Instance.isMoving = false;
        Instance.isAnimationLocked = false;

        Debug.Log("CONTROLLER moveVector.y is " + PlayerCharacterMotor.Instance.MoveVector.y);
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
        //ESCAPE sequence, allows us to modify animationLocks manually for avoid errors
        if (Input.GetKey(KeyCode.F))
            isAnimationLocked = false;
        if (Input.GetKey(KeyCode.E))
            isAnimationLocked = true;

        //if we are anim locked, leave this function. no Actions can happen until we are no longer locked
        if (isAnimationLocked == true)
            return;

        playerAnimationState = animationState.Idling; //default state, only modified if some other action happens

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
        if (Input.GetKey(KeyCode.R))
        {
            isAnimationLocked = true;
            playerAnimationState = animationState.Rolling;

            //TODO:
            //Corroutine here to wait for rolling to finish, then when done set isAnimationLocked = false; Encapsulate Handle Action
            //Inputs inside of another check for isAnimationLocked, that way any prior engaged animations are messed with until co
            //routine finishes.

            StartCoroutine(WaitForAnimation("DS roll fwd"));

            //wait for animation, and send roll movement command to Motor
            StartCoroutine(WaitXSeconds(1f));


        }

        //L1 attack action, with left click
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch(currentAttackChain)
            {
                case 0:
                    isAnimationLocked = true;
                    playerAnimationState = animationState.L1Attack;
                    currentAttackChain += 1;
                    StartCoroutine(WaitXSeconds(1f));
                    break;
                case 1:
                    isAnimationLocked = true;
                    playerAnimationState = animationState.L1AttackC2;
                    currentAttackChain += 1;
                    StartCoroutine(WaitXSeconds(1f));
                    break;
                case 2:
                    isAnimationLocked = true;
                    playerAnimationState = animationState.L1Attack;
                    currentAttackChain += 1;
                    StartCoroutine(WaitXSeconds(1f));
                    break;
                default:
                    break;
            }
        }

        //Equip item action
        if(Input.GetKeyDown(KeyCode.Y))
        {
            PlayerObject.Instance.equipPlayer("Recurve Bow");
            PlayerObject.Instance.equipPlayer("Shield");
        }
    }

    void HandleJumpInput()
    {
        //when our character hits the ground, reset our states
        if(CharacterController.isGrounded)
        {
            isJumping = false;
            isGliding = false;
        }

        //jump action
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if we are gliding, another space will cancel the glide
            if (isGliding == true)
                isGliding = false;

            //if we arent already jumping
            if(isJumping != true)
            {
                isJumping = true;
                PlayerCharacterMotor.Instance.Jump();
            }

            //if we are still in jumping state, and space was triggered. We can glide
            else
            {
                isGliding = true;
            }
        }
    }

    IEnumerator WaitXSeconds(float waitTime)
    {
        print(Time.time);
        yield return new WaitForSeconds(waitTime);
        print(Time.time);
        isAnimationLocked = false;
        currentAttackChain = 0;
    }

    IEnumerator WaitForAnimation(string animationName)
    {
        
        yield return null;
    }

}