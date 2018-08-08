using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashCharacterController : MonoBehaviour
{

    private CharacterController CharacterController;
    private SmashCharacterController Instance; //hold reference to current instance of itself
    public PlayerCharacter player; // hold reference to the player Object
    private SmashCharacterMotor thisMotor; //holds this local character controllers motor
    protected bool grounded;

    public enum animationState
    {
        Idling, Walking, Jogging, Running, Rolling, Stagger, Stunned, L1Attack, L1AttackC2, Jumping, Gliding, Aiming, DoubleJumping, Falling
    }
    public animationState playerAnimationState { get; set; }

    private int currentAttackChain = 0;
    private int maxAttackChain = 3;

    public float xAxis = 0f;
    public float deadZone = 0.05f;
    public float lastAxisInput = 0f;

    public bool isMoving;
    public bool isAnimationLocked;
    public bool isJumping = false;
    public bool isGliding = false;
    public bool isDoubleJumping = false;
    public bool isFalling = false;
    public bool isFacingStageRight = false;
    public bool isStunned = false;

    public bool isAiming = false;
    public bool firedBow = false;

    private int frameWaitCounter = 0;
    private int stunnedDurationInFrames = 0;

    private void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        thisMotor = this.GetComponent("SmashCharacterMotor") as SmashCharacterMotor;
        player = GetComponent<PlayerCharacter>();
        player.setName(this.name);
        Debug.Log(CharacterController);
        Debug.Log(thisMotor);

        Instance = this;
        checkStates();
    }


    private void Update()
    {
        checkStates();
        thisMotor.testWhichMotor();

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

        //handleKnockback after Jump because our Jump shouldnt override a knockback on the same frame.
        HandleKnockBack();

        //update the motor even if we didnt have locomotion, as the motor also calculates things like gravity.
        thisMotor.UpdateMotor();

    }

    private void GetLocomotionInput()
    {
        Instance.isMoving = false;
        Instance.isAnimationLocked = false;

        thisMotor.verticalVelocity = thisMotor.MoveVector.y;

        thisMotor.MoveVector = Vector3.zero; //reclacuate, prevents it from being additive. Basically restart on every update

        xAxis = Input.GetAxis("Horizontal");

        //if we are animation locked, we will ignore axis data
        if (!Instance.isAnimationLocked)
        {
            //check if input exceeds the deadZone (check for a minimal amount of input before moving)
            if (xAxis > deadZone || xAxis < -deadZone)
            {
                Instance.isMoving = true;
                thisMotor.MoveVector += new Vector3(xAxis, 0, 0);
                this.lastAxisInput = xAxis;
            }
        }

    }

    private void HandleActionInput()
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

        if (Input.GetKeyDown(KeyCode.A) || Input.GetAxis("Horizontal") < -deadZone)
        {
            Vector3 toRot = this.transform.rotation.eulerAngles;
            if (!isFalling)
            {
                this.transform.rotation = new Quaternion(toRot.x, 0, toRot.z, this.transform.rotation.w);
                isFacingStageRight = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetAxis("Horizontal") > deadZone)
        {
            Vector3 toRot = this.transform.rotation.eulerAngles;
            if (!isFalling)
            {
                this.transform.rotation = new Quaternion(toRot.x, 180, toRot.z, this.transform.rotation.w);
                isFacingStageRight = true;
            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
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

    private void HandleJumpInput()
    {
        //avoid being able to jump when we are stunned
        if (isAnimationLocked || isStunned)
            return;

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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.Joystick1Button1))
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
                thisMotor.Jump(false, xAxis);
            }

            //if we are still in jumping state, and space was triggered. We can glide
            else if (isJumping == true || isFalling == true)
            {
                if(isDoubleJumping != true)
                {
                    isDoubleJumping = true;
                    thisMotor.Jump(false, xAxis);
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

    private void HandleKnockBack()
    {
        if (Input.GetKeyDown(KeyCode.T))
            playerIsHit(false, true, -.5f, 80);

        //TODO:, Replace this will external function call. MAKE SURE it occurs after update frame. or in late update.
        // We could just the colliders check in LateUpdate to process hits.
        if (Input.GetKeyDown(KeyCode.G))
            playerIsHit(true, true, -.5f, 30);

        //updateCheck for if we are still knocked back or otherwise stunned.
        if (isStunned)
            playerAnimationState = animationState.Stunned;

    }

    //allow external classes to hit the character, sending direction, and hitForce.(which is dependent on what hit us
    public void playerIsHit(bool sendRight, bool sendUp, float hitForce, int stunDurationInFrames)
    {
        if (hitForce < 0) //ensure the calling method doesnt send us a negative number
            hitForce = -hitForce;

        if (sendRight)
        {
            if (sendUp)
                knockBack(hitForce, hitForce, stunDurationInFrames); // hori: positive, vert: positive;
            else
                knockBack(hitForce, -hitForce, stunDurationInFrames); // hori: positve, vert: negative
        }

        else //sendLeft
        {
            if (sendUp)
                knockBack(-hitForce, hitForce, stunDurationInFrames); //hori: negative, vert: positive;
            else
                knockBack(-hitForce, -hitForce, stunDurationInFrames); //hori: negative, vert: negative;
        }

    }//followed by its helper method: knockBack
    private void knockBack(float horizontalForce, float verticalForce, int stunDurationInFrames)
    {
        //since we were knocked back. Reset our framewait counter to 0. Add in our stun duration 
        frameWaitCounter = 0;
        stunnedDurationInFrames = stunDurationInFrames;
        isStunned = true;
        thisMotor.Knockback(horizontalForce, verticalForce, player.getPercentage());
    }

    private void checkStates()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance: .3f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        //check if that threw us into a falling state
        if (CharacterController.isGrounded)
            isFalling = false;
        else
            isFalling = true;

        //run HitStunWaitFrames, if we were stunned to see when we can leave the stunned state 
        if (isStunned)
            hitStunWaitFrames();
    }

    public bool isGrounded()
    {
        return grounded;
    }

    private void hitStunWaitFrames()
    {
        //check then increment the FrameWaitCounter
        if (frameWaitCounter++ == stunnedDurationInFrames)
            isStunned = false;
        else
            isStunned = true;
    }

    IEnumerator hitStunWait()
    {
        yield return new WaitForSeconds(1f);
        isStunned = false;
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
