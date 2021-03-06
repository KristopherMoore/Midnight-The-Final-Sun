﻿//Program Information///////////////////////////////////////////////////////////
/*
 * @file PlayerCharacterController.cs
 *
 *
 * @game-version 0.77 
 *          Kristopher Moore (19 May 2019)
 *          Modifications to Player Controller to support new animations, two handed states, rolls.
 *          
 *          The PlayerCharacterController is responsible for key functionalities, it serves as a base for a players interactions with their character. 
 *          It validates player inputs, and sends off any information needed by the Player Motor and the Animation controllers.
 *          
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public static CharacterController CharacterController;
    public static PlayerCharacterController Instance; //hold reference to current instance of itself

    //using enumerator to judge current anim state, benefit is that since we can only ever be in a single state for anims, enums reduce calls needed to change our value.
    public enum animationState
    {
        Idling, Walking, Jogging, Running, Twohanding, Sneaking, Stagger, Stunned, Jumping, Gliding, Aiming, DoubleJumping, Falling
    }
    public animationState playerAnimationState { get; set; }

    //attackChain calculators, from an older more meleee focused build of game, will be useful later in dev
    private int currentAttackChain = 0;
    private int maxAttackChain = 3;

    //deadZone to determine minium player input for actions
    public float deadZone = 0.01f;


    private int frameCounter = 0;


    //player states, used for checks on the controller, especially for animations. Used to use an enumerator, but multiple calls from exterior methods were required for checks,
    //whereas if we have some redundancy here we avoid exterior redundancy. I feel like this is the best choice.
    //TODO: add encapsulation to these values.
    public bool isMoving;
    public bool isWalking = false;
    public bool isRunning = false;
    public bool isAnimationLocked = false;
    public bool isAttackLocked = false;
    public bool isRolling = false;
    public bool isJumping = false;
    public bool isGliding = false;
    public bool isSneaking = false;
    public bool isAiming = false;
    public bool isFiring = false;
    public bool isReloading = false;
    public bool is2h = false;
    private bool flashLightOn = true;

    //axis values, we will hold for the motor, since the controller sends off for inputs.
    public float xAxis;
    public float yAxis;

    //attack chain ints, for keeping our current state location.
    public int R1attackChain = 0;
    private int maxR1 = 3;
    public int R2attackChain = 0;
    private int maxR2 = 2;

    //cameraState 1st Person, 0 is 3rd Person, 1 is 1st.   //Also need to grab Transform of models, so we can hide/unhide them
    public bool is1stPersonCamera = true;
    private Transform firstPersonModel;
    private Transform firstPersonArms;
    private Transform thirdPersonModel;

    //cameraAnchorPoint, used for moving the camera during crouch, etc.
    private Transform cameraAnchorPoint;
    Vector3 crouchCamOffset = new Vector3(0f, 2f, 0f);
    Vector3 standCamOffset = new Vector3(0f, 3f, 0f);
    float smoothSpeed = 5f;

    //hold our cameraFocusPoint, and laser start point, for firing the weapon
    private GameObject cameraFocusPoint;
    private GameObject laserStartPoint;

    //hold our flashlight object
    private FlashlightController flashlight;


    //----------------------------------------------------------------------------BEGIN FUNCTIONS--------------------------------------------------------------------------------------//
    //run on game load (before start)
    void Awake()
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;

        //grab Camera's anchor point, the Head
        cameraAnchorPoint = HelperK.FindSearchAllChildren(this.transform, "AnchorPointHead");

        //grab the Camera focus point, (ie what we are aiming at), and the laserStartPoint for our weapon
        cameraFocusPoint = GameObject.Find("Camera Focus Point");
        laserStartPoint = GameObject.Find("LaserStartPoint");

        //grab the player models for 1st / 3rd person
        //TODO: Update this to work within all player model status, check model for Player Instance. For now its just testing code
        firstPersonModel = HelperK.FindSearchAllChildren(this.transform, "1stPerson");
        firstPersonArms = GameObject.Find("1stPersonArms").transform;
        thirdPersonModel = HelperK.FindSearchAllChildren(this.transform, "3rdPerson");
        changeCameraState();

        //grab the flashlight
        flashlight = HelperK.FindSearchAllChildren(this.transform.parent.transform, "FlashLight").GetComponent<FlashlightController>();
    }

    
    //updates actions, run on every frame
    void Update()
    {
        //check for main camera or we are in menu, if none exists, exit.
        if (Camera.main == null || GameMenu.Instance.getMenuStatus()) 
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

    //responsible for getting input values and if above our deadZone, update our Motor's MoveVector
    void GetLocomotionInput()
    {
        Instance.isMoving = false;
        Instance.isAnimationLocked = false;

        PlayerCharacterMotor.Instance.verticalVelocity = PlayerCharacterMotor.Instance.MoveVector.y;

        PlayerCharacterMotor.Instance.MoveVector = Vector3.zero; //recalcuate, prevents it from being additive. Basically restart on every update

        //ask the Input manager for our Horizontal / vertical inputs. (PC this is WASD, Controller this is Left Stick)
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

        
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

        //Since we are not animation locked, animate our body based on X and Y Axes
        if (AnimateBodyParts.Instance && is1stPersonCamera)
            AnimateBodyParts.Instance.setMotionAxes(xAxis, yAxis);

    }

    //handles players inputs for actions, things like moving, attacking, etc. see above each keyCode check for more info
    void HandleActionInput()
    {
        //if we are anim locked, leave this function. no Actions can happen until we are no longer locked
        if (isAnimationLocked == true)
            return;

        //default state, only modified if some other action happens
        playerAnimationState = animationState.Idling;

        //movement checks for the animators, always check these first then other action inputs.
        if (isMoving)
        {
            playerAnimationState = animationState.Jogging;
            if (InputHandler.Instance.checkAction(InputHandler.actions.RUN))
            {
                //cant run if we are sneaking, so check that and proceed accordingly
                if (isSneaking == false)
                {
                    isRunning = true;
                    playerAnimationState = animationState.Running;
                }
            }
            else if (Input.GetKey(KeyCode.Z))
                isWalking = true;
            else
            {
                isWalking = false;
                isRunning = false;
            }
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
                playerAnimationState = animationState.Sneaking;
            }
            else
            {
                isSneaking = false;
            }

            //set our Animation state
            AnimateBodyParts.Instance.setSneakState(isSneaking);
        }
        //continuation of crouch, but need to run every frame, LERP our way to the positon
        if (PlayerCharacterController.Instance.isSneaking)
        {
            cameraAnchorPoint.localPosition = Vector3.Lerp(cameraAnchorPoint.localPosition, crouchCamOffset, smoothSpeed * Time.deltaTime);
        }
        else
            cameraAnchorPoint.localPosition = Vector3.Lerp(cameraAnchorPoint.localPosition, standCamOffset, smoothSpeed * Time.deltaTime);

        //check for aiming
        if (InputHandler.Instance.checkAction(InputHandler.actions.AIM))
            isAiming = true;
        else
            isAiming = false;

        //player roll check
        if (InputHandler.Instance.checkAction(InputHandler.actions.ROLL))
        {
            //run the rolling coroutine, set our animationLock
            isAnimationLocked = true;
            StartCoroutine(RollAction(1f));
        }

        //light attack action
        if (InputHandler.Instance.checkAction(InputHandler.actions.LIGHTATTACK))
        {
            //if not locked, start R1 chain
            if(!isAttackLocked)
                StartCoroutine(playerAttack("R1", .5f));

            //run the FireWeapon coroutine, if we arent currently in a isFiring state. or reloading
            //if(!isFiring && !isReloading)
            //StartCoroutine(FireWeapon(.3f));
        }

        //heavy attack action
        if (InputHandler.Instance.checkAction(InputHandler.actions.HEAVYATTACK))
        {
            //if not locked, start R2 chain
            if (!isAttackLocked)
                StartCoroutine(playerAttack("R2", 1f));
        }

        //Flashlight boolean check.
        if (Input.GetKeyDown(KeyCode.F))
        {
            //negate whatever we were set to, and send to the flashlight controller
            flashLightOn = !flashLightOn;
            flashlight.setFlashlight(flashLightOn); 
        }

        //TODO: Use item command
        if (InputHandler.Instance.checkAction(InputHandler.actions.USEITEM))
        {

        }

        //change camera state, If 1Person -> 3Person, else 3P -> 1P
        if(InputHandler.Instance.checkAction(InputHandler.actions.CHANGECAM))
        {
            changeCameraState();
        }

        //switch player stance (2h vs 1 hand)
        if (InputHandler.Instance.checkAction(InputHandler.actions.SWITCHSTANCE))
        {
            is2h = !is2h;
        }
    }

    //Handles specifically jump inputs, seperated for readability
    void HandleJumpInput()
    {
        //when our character hits the ground, reset our states
        if(CharacterController.isGrounded)
        {
            isJumping = false;
            isGliding = false;
        }

        //jump action
        if (InputHandler.Instance.checkAction(InputHandler.actions.JUMP))
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

            //else we are still in jumping state, and space was triggered. We can glide
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

    //send off any necessary animationStates to the animators
    private void updateAnimations()
    {
        //none of these apply in ThirdPerson mode, so return
        if (!is1stPersonCamera)
            return;

        //reset Animations
        if(AnimateArms.Instance)
            AnimateArms.Instance.resetAllAnimations();
        if(AnimateBodyParts.Instance)
            AnimateBodyParts.Instance.resetAllAnimations();
        if(AnimateWeapon.Instance)
            AnimateWeapon.Instance.resetAllAnimations();

        //use switch conditional on our animation state, and set our relevant animation controller's values
        switch (playerAnimationState)
        {
            case animationState.Idling:
                AnimateArms.Instance.setIdling(true);
                break;
            case animationState.Jogging:
                AnimateArms.Instance.setJogging(true);
                break;
            case animationState.Running:
                AnimateArms.Instance.setRunning(true);
                break;
            case animationState.Jumping:
                AnimateBodyParts.Instance.setJumping(true);
                break;
            case animationState.Falling:
                AnimateBodyParts.Instance.setFalling(true);
                break;
            default:
                break;

        }

        //some animations (like sneak/reload/fire) can happen during other animation states, but as an enumerator gets overriden, we need to check it seperately
        if(isSneaking)
        {
            AnimateBodyParts.Instance.setSneakState(true);
        }
        if(isReloading)
        {
            AnimateArms.Instance.setReloading(true);
            AnimateWeapon.Instance.setReloading(true);
        }
        if(isFiring)
        {
            AnimateArms.Instance.setFired(true);
            AnimateWeapon.Instance.setFired(true);
        }
    }

    //changes our camera state, invert our boolean, and hide models as needed
    private void changeCameraState()
    {
        //invert our Camera state
        is1stPersonCamera = !is1stPersonCamera;

        //adjust model's visibility as necessary, using inverse logic for thirdPersonModel. Cleaner code, doesnt need conditional checks
        firstPersonModel.gameObject.SetActive(is1stPersonCamera);
        firstPersonArms.gameObject.SetActive(is1stPersonCamera);
        thirdPersonModel.gameObject.SetActive(!is1stPersonCamera);
    }

    //reloading enumerator, allows us to send off the reload anim, and only end reloading state when the time is done.
    IEnumerator WaitXSecondsForReload(float waitTime)
    {
        //wait the time out
        yield return new WaitForSeconds(waitTime);

        //end our animation lock, and our reloading state, to allow other anims, including reload to be activated again
        isAnimationLocked = false;
        isReloading = false;

        //end anims
        AnimateArms.Instance.setReloading(false);
        AnimateWeapon.Instance.setReloading(false);
    }

    //action enumerator, allows us to perform an action based on the enum we send in
    IEnumerator playerAttack(string attackCode, float waitTime)
    {
        //PRE-WAIT ACTIONS
        isAttackLocked = true;

        switch (attackCode)
        {
            case "R1":
                //increase our chain
                R1attackChain++;
                break;

            case "R2":
                //increase our chain
                R2attackChain++;
                break;
        }

        //wait the time out, in frames, just waiting here so the slide can cock back and "rechamber" a round
        yield return new WaitForSeconds(waitTime);

        //POST-WAIT ACTIONS
        isAttackLocked = false;

        //bound our chains, if necessary
        if (R1attackChain >= maxR1)
            R1attackChain = 0;
        if (R2attackChain >= maxR2)
            R2attackChain = 0;
    }

    //rolling enumerator, allows us to wait for a roll, before ending our animation lock
    IEnumerator RollAction(float waitTime)
    {
        //PRE-WAIT ACTIONS
        isAnimationLocked = true;
        //play our fire sound
        SoundManager.Instance.PlaySound("Fire");
        //set roll state, for animator
        isRolling = true;

        //wait the time out, in frames, just waiting here so the slide can cock back and "rechamber" a round
        yield return new WaitForSeconds(waitTime);

        //POST-WAIT ACTIONS
        isAnimationLocked = false;
        isRolling = false;

    }

    //fire weapon enumerator, this allows us to delay fire times based on the weapon's fire rate (Fast guns shoot fast, slow guns shoot slow)
    //this doubles as the raycast calculator, to see if our shot actually hit anything.
    //TODO: Offset this into a WEAPON controller, at this point its only here for testing purposes
    IEnumerator FireWeapon(float waitTime)
    {
        //PRE-WAIT ACTIONS
        Instance.isFiring = true;
        //play our fire sound
        SoundManager.Instance.PlaySound("Fire");
        //send off animation
        AnimateArms.Instance.setFired(true);
        AnimateWeapon.Instance.setFired(true);

        //now we need to check if we actually hit anything. Due to us using hitscan, we can just do this with raycasting, If we move into a projectile system,
        //replace this hitscan check with a projectile Instantiation. (on the bullet would use a projectile script to move it in worldspace and handle collisions)
        Vector3 targetDirection = cameraFocusPoint.transform.position - laserStartPoint.transform.position;

        //---DEBUG---
        Debug.DrawRay(laserStartPoint.transform.position, targetDirection, Color.magenta);

        //cast a ray to see if the target is close enough to start a melee attack
        RaycastHit hit;
        if (Physics.Raycast(laserStartPoint.transform.position, targetDirection, out hit, Mathf.Infinity))
        {
            Debug.Log(hit.transform.root.tag);
            Debug.Log(hit.transform.name);
            if (hit.transform.root.tag == "Enemy")
            {
                hit.transform.root.GetComponent<Unit>().modifyHP(-100f);
                Debug.Log(hit.transform.root.GetComponent<Unit>().getHP());
            }
        }

        //wait the time out, in frames, just waiting here so the slide can cock back and "rechamber" a round
        yield return new WaitForSeconds(waitTime);

        //POST-WAIT ACTIONS
        isAnimationLocked = false;
        isFiring = false;

        //end anims
        AnimateArms.Instance.setFired(false);
        AnimateWeapon.Instance.setFired(false);
    }

    //enumerator to wait for animation to finish
    //TODO: Replace with more relevant actions based on above
    IEnumerator WaitForAnimation(string animationName)
    {
        
        yield return null;
    }

}