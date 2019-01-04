using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    //IMPORTANT NOTE: I have offloaded redundancy into this Controller, primarily because the Animation scripts were very redundant in state checking
    //since this controller already has to check these states we can drastically reduce overhead by simply adding in statement load. Furthermore,
    //this controller already has exit conditions for optimization, so that is expontential in our overall statement execution reduction.

    public static CharacterController CharacterController;
    public static PlayerCharacterController Instance; //hold reference to current instance of itself

    public enum animationState
    {
        Idling, Walking, Jogging, Running, Rolling, Stagger, Stunned, L1Attack, L1AttackC2, Jumping, Gliding, Aiming, DoubleJumping, Falling
    }
    public animationState playerAnimationState { get; set; }

    private int currentAttackChain = 0;
    private int maxAttackChain = 3;

    public float deadZone = 0.01f;


    private int frameCounter = 0;


    //player states, used for checks on the controller, especially for animations. Used to use an enumerator, but multiple calls from exterior methods were required for checks,
    //whereas if we have some redundancy here we avoid exterior redundancy. I feel like this is the best choice.
    //TODO: add encapsulation to these values.
    public bool isMoving;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isAnimationLocked = false;
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

        //updateOurAnimators
        updateAnimations();

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
                //cant run if we are sneaking, so check that and proceed accordingly
                if (isSneaking == false)
                {
                    isRunning = true;
                    AnimateArms.Instance.setRunning(true);
                }
                else
                {
                    isRunning = false;
                    AnimateArms.Instance.setRunning(false);
                }
            }
            else if (Input.GetKey(KeyCode.Z))
                isWalking = true;
            else
                isWalking = false;
        }
        else
            playerAnimationState = animationState.Idling;

        //check for crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //check and inverse our sneaking state.
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
            Instance.isReloading = true;

            //send off animation.
            AnimateArms.Instance.setReloading(true);
            AnimateWeapon.Instance.setReloading(true);

            //wait for Animation to play. based on weapon being handled.
            StartCoroutine(WaitXSecondsForReload(4f));

        }

        //Weapon fire action, with left click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //run the FireWeapon coroutine, if we arent currently in a isFiring state.
            if(!isFiring)
                StartCoroutine(FireWeapon(.3f));
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

    private void updateAnimations()
    {
        switch(playerAnimationState)
        {
            case animationState.Jogging:
                AnimateArms.Instance.setRunning(true);
                break;

            
            default:
                break;

        }
    }

    IEnumerator WaitXSecondsForReload(float waitTime)
    {
        //wait the time out
        yield return new WaitForSeconds(waitTime);
        isAnimationLocked = false;
        isReloading = false;

        //end anims
        AnimateArms.Instance.setReloading(false);
        AnimateWeapon.Instance.setReloading(false);
    }

    IEnumerator FireWeapon(float waitTime)
    {
        //PRE-WAIT ACTIONS
        Instance.isFiring = true;
        //play our fire sound
        SoundManager.Instance.PlaySound("Fire");
        //send off animation
        AnimateArms.Instance.setFired(true);
        AnimateWeapon.Instance.setFired(true);

        //wait the time out, in frames
        yield return new WaitForSeconds(waitTime);

        //POST-WAIT ACTIONS
        isAnimationLocked = false;
        isFiring = false;

        //end anims
        AnimateArms.Instance.setFired(false);
        AnimateWeapon.Instance.setFired(false);
    }

    IEnumerator WaitForAnimation(string animationName)
    {
        
        yield return null;
    }

}