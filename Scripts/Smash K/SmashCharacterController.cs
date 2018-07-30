using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashCharacterController : MonoBehaviour
{

    public static CharacterController CharacterController;
    public static SmashCharacterController Instance; //hold reference to current instance of itself

    public enum animationState
    {
        Idling, Walking, Jogging, Running, Rolling, Stagger, Stunned, L1Attack, L1AttackC2, Jumping, Gliding, Aiming, DoubleJumping, Falling
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
    public bool isDoubleJumping = false;
    public bool isFalling = false;
    public bool isFacingStageRight = false;

    public bool isAiming = false;
    public bool firedBow = false;

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        
        Instance = this;
    }


    void Update()
    {
        if (Camera.main == null) //check for main camera, if none exists, exit.
            return;

        //if (GameMenu.Instance.getMenuStatus() == true) //if we are currently in a menu, remove control of character, by ending this update run
            //return;

        //handle actions before locomotion, since actions will at times lock the player into place
        HandleActionInput();

        //if we arent locked in an animation, continue with locomotion.
        if (!this.isAnimationLocked)
        {
            GetLocomotionInput();
        }

        //handling jumpstate after locomotion specifically. Unlike the other actions, this applies directly into the locomotion processes
        //and if it came before it would be overrident by the verticalVelocity check in GetLocomotionInput
        HandleJumpInput();

        //update the motor even if we didnt have locomotion, as the motor also calculates things like gravity.
        SmashCharacterMotor.Instance.UpdateMotor();

    }

    void GetLocomotionInput()
    {
        Instance.isMoving = false;
        Instance.isAnimationLocked = false;

        SmashCharacterMotor.Instance.verticalVelocity = SmashCharacterMotor.Instance.MoveVector.y;

        SmashCharacterMotor.Instance.MoveVector = Vector3.zero; //reclacuate, prevents it from being additive. Basically restart on every update

        float xAxis = Input.GetAxis("Horizontal");

        //if we are animation locked, we will ignore axis data
        if (!Instance.isAnimationLocked)
        {
            //check if input exceeds the deadZone (check for a minimal amount of input before moving)
            if (xAxis > deadZone || xAxis < -deadZone)
            {
                Instance.isMoving = true;
                SmashCharacterMotor.Instance.MoveVector += new Vector3(xAxis, 0, 0);
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            Vector3 toRot = this.transform.rotation.eulerAngles;
            if (!isFalling)
            {
                this.transform.rotation = new Quaternion(toRot.x, 0, toRot.z, this.transform.rotation.w);
                isFacingStageRight = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector3 toRot = this.transform.rotation.eulerAngles;
            if (!isFalling)
            {
                this.transform.rotation = new Quaternion(toRot.x, 180, toRot.z, this.transform.rotation.w);
                isFacingStageRight = true;
            }
        }

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

        //check for aiming
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            playerAnimationState = animationState.Aiming;

            //if we have chosen to fire during aiming
            if (Input.GetMouseButtonDown(0))
                firedBow = true;
            else
                firedBow = false;

        }
        //on frame aiming has stopped
        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (currentAttackChain)
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
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PlayerObject.Instance.equipPlayer("Recurve Bow");
            PlayerObject.Instance.equipPlayer("Katana");
        }
    }

    void HandleJumpInput()
    {

        //when our character hits the ground, reset our states
        if (CharacterController.isGrounded)
        {
            isJumping = false;
            isGliding = false;
            isDoubleJumping = false;
            isFalling = false;
        }
        else
            isFalling = true;

        //jump action
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if we are gliding, another space will cancel the glide
            //if (isGliding)
            //{
             //   isGliding = false;
              //  return;
            //}

            //if we arent already jumping
            if (isJumping == false && isFalling == false)
            {
                isJumping = true;
                SmashCharacterMotor.Instance.Jump();
            }

            //if we are still in jumping state, and space was triggered. We can glide
            else if (isJumping == true || isFalling == true)
            {
                if(isDoubleJumping != true)
                {
                    isDoubleJumping = true;
                    SmashCharacterMotor.Instance.Jump();
                }
                isGliding = true;
            }
        }

        //check and apply jump / glide animations
        if (isJumping)
            playerAnimationState = animationState.Jumping;
        if (isGliding)
            playerAnimationState = animationState.Gliding;
        if (isDoubleJumping)
            playerAnimationState = animationState.DoubleJumping;

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
