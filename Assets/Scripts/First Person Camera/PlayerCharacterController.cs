using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{

    public static CharacterController CharacterController;
    public static PlayerCharacterController Instance; //hold reference to current instance of itself

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
    public bool isSneaking = false;
    public bool isAiming = false;
    public bool isFiring = false;
    public bool isReloading = false;


    //cameraAnchorPoint, used for moving the camera during crouch, etc.
    private Transform cameraAnchorPoint;

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;

        //grab Camera's anchor point, the Head
        cameraAnchorPoint = HelperK.FindSearchAllChildren(this.transform, "AnchorPointHead");
    }

    
    void Update()
    {
        if (Camera.main == null) //check for main camera, if none exists, exit.
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

        PlayerCharacterMotor.Instance.verticalVelocity = PlayerCharacterMotor.Instance.MoveVector.y;

        PlayerCharacterMotor.Instance.MoveVector = Vector3.zero; //reclacuate, prevents it from being additive. Basically restart on every update

        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        
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

        //Since arent animation locked, animate our body based on X and Y Axes
        AnimateBodyParts.Instance.setMotionAxes(xAxis, yAxis);

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
            //NEED TO REFACTOR, old animationStates into states like isSneaking.
            playerAnimationState = animationState.Jogging;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if(isSneaking == false)
                    playerAnimationState = animationState.Running;
            }
            else if (Input.GetKey(KeyCode.Z))
                playerAnimationState = animationState.Walking;
        }
        else
            playerAnimationState = animationState.Idling;

        //check for crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //check and inverse our sneaking state. //IN FUTURE; can have our player set whether crouch should be toggle or hold, ie change GetKeyDown to GetKey (Hold).
            if (isSneaking == false)
            {
                isSneaking = true;

                //set our height
                cameraAnchorPoint.SetPositionAndRotation(cameraAnchorPoint.position + Vector3.down, cameraAnchorPoint.rotation);
            }
            else
            {
                isSneaking = false;

                //set our height
                cameraAnchorPoint.SetPositionAndRotation(cameraAnchorPoint.position + Vector3.up, cameraAnchorPoint.rotation);
            }

            //set our Animation state
            AnimateBodyParts.Instance.setSneakState(isSneaking);
        }

        //check for aiming
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            playerAnimationState = animationState.Aiming;

        }
        //on frame aiming has stopped
        if(Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

        //reloading action
        if (Input.GetKey(KeyCode.R))
        {
            //isAnimationLocked = true; COMMENT OUT FOR NOW, may need to revise how anims locks were working in redesign
            Instance.isReloading = true;

            //send off animation.
            AnimateArms.Instance.setReloading(true);
            AnimateWeapon.Instance.setReloading(true);

            //wait for Animation to play. based on weapon being handled.
            StartCoroutine(WaitXSecondsForReload(3f));

        }

        //Weapon fire action, with left click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instance.isFiring = true;
        }
        else
            Instance.isFiring = false;

        //Equip item action
        if(Input.GetKeyDown(KeyCode.Y))
        {
            PlayerObject.Instance.equipPlayer("Recurve Bow");
            PlayerObject.Instance.equipPlayer("Katana");
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
            if(isGliding)
            {
                isGliding = false;
                return;
            }

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

        //check and apply jump / glide animations
        if(isJumping)
            playerAnimationState = animationState.Jumping;
        if(isGliding)
            playerAnimationState = animationState.Gliding;

    }

    IEnumerator WaitXSecondsForReload(float waitTime)
    {
        print(Time.time);
        yield return new WaitForSeconds(waitTime);
        print(Time.time);
        isAnimationLocked = false;
        isReloading = false;

        //end anims
        AnimateArms.Instance.setReloading(false);
        AnimateWeapon.Instance.setReloading(false);
    }

    IEnumerator WaitForAnimation(string animationName)
    {
        
        yield return null;
    }

}