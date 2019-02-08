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
        Idling, Walking, Jogging, Running, Sneaking, Stagger, Stunned, Jumping, Gliding, Aiming, DoubleJumping, Falling
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
    private bool flashLightOn = true;

    //cameraState 1st Person, 0 is 3rd Person, 1 is 1st
    public bool is1stPersonCamera = false;

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

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;

        //grab Camera's anchor point, the Head
        cameraAnchorPoint = HelperK.FindSearchAllChildren(this.transform, "AnchorPointHead");

        //grab the Camera focus point, (ie what we are aiming at), and the laserStartPoint for our weapon
        cameraFocusPoint = GameObject.Find("Camera Focus Point");
        laserStartPoint = GameObject.Find("LaserStartPoint");

        //grab the flashlight
        flashlight = HelperK.FindSearchAllChildren(this.transform.parent.transform, "FlashLight").GetComponent<FlashlightController>();
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
        //if we are anim locked, leave this function. no Actions can happen until we are no longer locked
        if (isAnimationLocked == true)
            return;

        playerAnimationState = animationState.Idling; //default state, only modified if some other action happens

        //movemetn checks for the animators, always check these first then other action inputs.
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            playerAnimationState = animationState.Jogging;
            if (Input.GetKey(KeyCode.LeftShift))
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
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
        }

        //on frame aiming has stopped
        if(Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

        //reloading action
        if (Input.GetKey(KeyCode.R))
        {
            //if were are not currently reloading, do not use negation here, because we send off coroutines, hard checks necessary
            if (!isReloading)
            {
                Instance.isReloading = true;

                //send off animation.
                AnimateArms.Instance.setReloading(true);
                AnimateWeapon.Instance.setReloading(true);

                //wait for Animation to play. based on weapon being handled.
                StartCoroutine(WaitXSecondsForReload(4.25f));
            }

        }

        //Weapon fire action, with left click
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //run the FireWeapon coroutine, if we arent currently in a isFiring state. or reloading
            if(!isFiring && !isReloading)
                StartCoroutine(FireWeapon(.3f));
        }

        //Flashlight boolean check.
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashLightOn = !flashLightOn; //negate whatever we had before and send that to the flashlight controller
            flashlight.setFlashlight(flashLightOn); 
        }
        if (Input.GetKey(KeyCode.E))
        {

        }

        //check Camera changes, if so, invert.
        if(Input.GetKeyDown(KeyCode.U))
        {
            is1stPersonCamera = !is1stPersonCamera;
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

    //send off any necessary animationStates to the animators
    private void updateAnimations()
    {
        //reset Animations
        AnimateArms.Instance.resetAllAnimations();
        AnimateBodyParts.Instance.resetAllAnimations();
        AnimateWeapon.Instance.resetAllAnimations();

        //utilizing a switch here for C's overall more efficient execution time rather than if conditionals
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

        //now we need to check if we actually hit anything. Due to us using hitscan, we can just do this with raycasting, If we move into a projectile system,
        //replace this hitscan check with a projectile Instantiation. (on the bullet would use a projectile script to move it in worldspace and handle collisions)
        Vector3 targetDirection = cameraFocusPoint.transform.position - laserStartPoint.transform.position;

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

    IEnumerator WaitForAnimation(string animationName)
    {
        
        yield return null;
    }

}