using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashCharacterMotor : MonoBehaviour
{


    public static SmashCharacterMotor Instance; //hold reference to instance of itself  

    public float WalkSpeed = 1.75f;
    public float JogSpeed = 3.75f;
    public float RunSpeed = 6.0f;
    public float SlideSpeed = 3f;
    public float jumpSpeed = 4.5f;
    public float Gravity = 7f;
    public float terminalVelocity = 5.5f;
    public float glidingModifier = 19f;
    public float slideThreshold = 0.6f;
    public float maxControllableSlideMagnitude = 0.4f;


    public Vector3 MoveVector { get; set; }
    public float verticalVelocity = 0;


    public GameObject cameraFocusPoint;

    public float smoothTime = 0.3f;
    private float velocity = 0.1f;


    // 
    void Awake()
    {
        Instance = this;
    }

    // Update is called by the TP_Controller
    public void UpdateMotor()
    {
        ProcessMotion();
    }

    void ProcessMotion()
    {
 
        //Normalize MoveVector, which is now relative to cam rotation
        //MoveVector = Vector3.Normalize(MoveVector);

        //Multiply MoveVector by MoveSpeed (units per frame)
        MoveVector = MoveVector * MoveSpeed();

        //Reapply verticalVelocity
        MoveVector = new Vector3(MoveVector.x, verticalVelocity, MoveVector.z);

        //Apply Gravity / gliding Gravity depending on state
        //if (SmashCharacterController.Instance.isGliding)
           // applyGravityGliding();
        //else
        applyGravity();

        //Move the Character in World Space,if not animation locked
        if (!SmashCharacterController.Instance.isAnimationLocked)
        {
            SmashCharacterController.CharacterController.Move(MoveVector * Time.deltaTime); //Multiply MoveVector by DeltaTime (converts to units per second) right before we move

            //Modify Rotation to direction in which we moved
            if (SmashCharacterController.Instance.isMoving)
            {
                //old rot
                //Vector3 toRot = new Vector3(MoveVector.x, 0f, MoveVector.z);
                //transform.rotation = Quaternion.LookRotation(toRot);
            }
        }
    }

    //method for rolling, will propel the character in their rolled direction, over a set duration.
    public void Roll()
    {

    }

    void applyGravity()
    {
        if (MoveVector.y > -terminalVelocity)
        {
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * Time.deltaTime), MoveVector.z);
        }
        if (SmashCharacterController.CharacterController.isGrounded && MoveVector.y < -1)
        {
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
        }
    }

    //instead of messing with terminal velocity settings and changing grav and them, I decided the gravity for gliding would be cleaner
    void applyGravityGliding()
    {
        if (MoveVector.y < -1)
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);

        //divide our terminal velocity check by our gliding modifier, we reach a much lower terminal velocity and stay there
        //We could have removed a check entirely and just set our vector to just encorporate a fixed speed.
        //But I feel this method is more safe from error
        if (MoveVector.y > -(terminalVelocity / glidingModifier))
        {
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * Time.deltaTime), MoveVector.z);
        }

        if (SmashCharacterController.CharacterController.isGrounded && MoveVector.y < -1)
        {
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
        }
    }

    public void Jump()
    {
        if (SmashCharacterController.CharacterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }

        else
            verticalVelocity = jumpSpeed;
    }

    float MoveSpeed()
    {
        float moveSpeed = JogSpeed;

        if (SmashCharacterController.Instance.playerAnimationState == SmashCharacterController.animationState.Walking)
            moveSpeed = WalkSpeed;

        if (SmashCharacterController.Instance.playerAnimationState == SmashCharacterController.animationState.Running)
            moveSpeed = RunSpeed;

        return moveSpeed;
    }

}

