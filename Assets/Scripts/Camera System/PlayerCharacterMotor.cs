using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class responsible for handling the Motion behaviour of our player character. Moving in world space, calculating vertical positional offsets (jumping, falling, gliding)
public class PlayerCharacterMotor : MonoBehaviour
{
    public static PlayerCharacterMotor Instance; //hold reference to instance of itself  

    private float WalkSpeed = 1.5f;
    private float JogSpeed = 2.5f;
    private float RunSpeed = 5f;
    private float SneakSpeed = 3f;
    public float SlideSpeed = 3f;
    public float jumpSpeed = 8f;
    public float Gravity = 21f;
    public float terminalVelocity = 20f;
    public float glidingModifier = 19f;
    public float slideThreshold = 0.6f;
    public float maxControllableSlideMagnitude = 0.4f;

    public Vector3 MoveVector { get; set; }
    public float verticalVelocity = 0;

    public GameObject cameraFocusPoint;

    public float smoothTime = 0.3f;
    private float velocity = 0.1f;

    void Awake()
    {
        Instance = this;
    }

    // This Update is called by the PlayerCharacterController, ensuring it always runs after the Controller
    public void UpdateMotor()
    {
        ProcessMotion();
    }

    //Process our Motion based on updates performed by the PlayerCharacterController
    void ProcessMotion()
    {
                //TO BE REMOVED no longer useful for our style movement, could be useful when doing lock on system
                //Transform MoveVector to WorldSpace relative to our characters location
                //MoveVector = transform.TransformDirection(MoveVector);

        //Modify MoveVector relative to our cameras rotation
        Vector3 cameraDirection = Camera.main.transform.TransformDirection(MoveVector.x, MoveVector.y, MoveVector.z);
        cameraDirection.y = 0.0f;
        MoveVector = cameraDirection;

        //Normalize MoveVector, which is now relative to cam rotation
        MoveVector = Vector3.Normalize(MoveVector);

        //Multiply MoveVector by MoveSpeed (units per frame)
        MoveVector = MoveVector * MoveSpeed();

        //Reapply verticalVelocity
        MoveVector = new Vector3(MoveVector.x, verticalVelocity, MoveVector.z);

        //Apply Gravity / gliding Gravity depending on state
        if (PlayerCharacterController.Instance.isGliding)
            applyGravityGliding();
        else
            applyGravity();

        //Move the Character in World Space,if not animation locked
        if(!PlayerCharacterController.Instance.isAnimationLocked)
        {
            PlayerCharacterController.CharacterController.Move(MoveVector * Time.deltaTime); //Multiply MoveVector by DeltaTime (converts to units per second) right before we move

            //rotate our character model towards our camera focus point
            Vector3 focusP = cameraFocusPoint.transform.position;
            //focusP.y = 0f;
            //transform.LookAt(cameraFocusPoint.transform);
            //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            //transform.rotation = Quaternion.LookRotation(focusP);
        }
    }

    //method for rolling, will propel the character in their rolled direction, over a set duration.
    //TODO: Reimplement roll status and actions
    public void Roll()
    {

    }

    //Apply Graivity on the player
    void applyGravity()
    {
        //if we have reached terminalVelocity (maximum possible fall speed), negative because we will always be falling downward (neg direction)
        if (MoveVector.y > -terminalVelocity)
        {
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * Time.deltaTime), MoveVector.z);
        }
        //if we are grounded still, ensure we remain consistent
        if (PlayerCharacterController.CharacterController.isGrounded && MoveVector.y < -1)
        {
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
        }
    }

    //instead of messing with terminal velocity settings and changing grav and them, I decided a gravity function for gliding would be cleaner
    void applyGravityGliding()
    {
        if(MoveVector.y < -1)
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);

        //divide our terminal velocity check by our gliding modifier, we reach a much lower terminal velocity and stay there
        //We could have removed a check entirely and just set our vector to just encorporate a fixed speed.
        //But I feel this method is more safe from error
        if (MoveVector.y > -(terminalVelocity / glidingModifier))
        {
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * Time.deltaTime), MoveVector.z);
        }

        if (PlayerCharacterController.CharacterController.isGrounded && MoveVector.y < -1)
        {
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
        }
    }

    //handles jump actions, only if we are still grounded, can we apply changes to the verticalVelocity vector
    public void Jump()
    {
        if (PlayerCharacterController.CharacterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }
    }

    //handles the modifications to movementSpeed based on our current movement state (Walking, Running, Sneaking, etc)
    float MoveSpeed()
    {
        float moveSpeed = JogSpeed;

        if (PlayerCharacterController.Instance.isWalking)
            moveSpeed = WalkSpeed;
      
        if (PlayerCharacterController.Instance.isRunning)
            moveSpeed = RunSpeed;

        if (PlayerCharacterController.Instance.isSneaking)
            moveSpeed = SneakSpeed;

        return moveSpeed;
    }

}


