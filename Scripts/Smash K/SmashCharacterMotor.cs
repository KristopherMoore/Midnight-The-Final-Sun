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
    public float Gravity = 8f;
    public float terminalVelocity = 5.5f;
    public float glidingModifier = 19f;
    public float slideThreshold = 0.6f;
    public float maxControllableSlideMagnitude = 0.4f;


    public Vector3 MoveVector { get; set; }
    public float verticalVelocity = 0;

    private float horizontalVelocity = 0;
    private float currentMaxHVel = 0;
    private float minimumBound = 2;
    private float jumpHVelForceMultiplier = 4f;


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
        //Determine whether to deal with Air or Ground Motor Mechanics
        if (SmashCharacterController.CharacterController.isGrounded)
            ProcessMotion();
        else
            ProcessAirMotion();
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
        applyGravity();

        //Move the Character in World Space,if not animation locked
        if (!SmashCharacterController.Instance.isAnimationLocked)
        {
            SmashCharacterController.CharacterController.Move(MoveVector * Time.deltaTime); //Multiply MoveVector by DeltaTime (converts to units per second) right before we move
        }
    }

    private void ProcessAirMotion()
    {
        //Check if we started falling without jumping. if so we when off a ledge. In that case, check what the last movement differential was (last time input was above
        //a deadzone. and use it to seed the horiontal velocity. This should similuate running off edges)
        if (SmashCharacterController.Instance.isFalling && !SmashCharacterController.Instance.isJumping)
            horizontalVelocity = SmashCharacterController.Instance.lastAxisInput * jumpHVelForceMultiplier;


        //Debug.Log(horizontalVelocity + " start Hvel");

        //Account for Influence of horizontal Velocity, and clamp the number to the current max in either positive or neg direction. Must use logic to check
        horizontalVelocity += (SmashCharacterController.Instance.xAxis * .25f);

        //Debug.Log(horizontalVelocity + " after xAxis DI");

        float negBound = 0;
        float posBound = 0;
        if (currentMaxHVel > 0)
        {
            posBound = currentMaxHVel;
            negBound = -currentMaxHVel;
        }
        else if (currentMaxHVel < 0)
        {
            posBound = -currentMaxHVel;
            negBound = currentMaxHVel;
        }
        else //must be zero, allow space for DI
        {
            posBound = minimumBound;
            negBound = -minimumBound;
        }
        

        //double check for minimum bound. This ensure small axis angles dont cause slow vertical jumps
        if (posBound < minimumBound)
            posBound = minimumBound;

        if (negBound > -minimumBound)
            negBound = -minimumBound;

        //bound hVel
        if (horizontalVelocity > posBound)
            horizontalVelocity = posBound;
        if (horizontalVelocity < negBound)
            horizontalVelocity = negBound;



        //modify moveVector by applying both horizontal and vertical velocity
        MoveVector = new Vector3(horizontalVelocity, verticalVelocity, MoveVector.z);
        applyGravity();

        //multiply by DletaTime
        SmashCharacterController.CharacterController.Move(MoveVector * Time.deltaTime);

        //Debug.Log(horizontalVelocity + " hVel");
        //Debug.Log(currentMaxHVel + " max");
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

    //force here is a "vector" in the traditional sense, a force and a direction based on positive or negative
    public void Jump(bool isShorthop, float force)
    {
        //set vert vel based on jumpspeed of short/full hops
        if (isShorthop)
            verticalVelocity = jumpSpeed / 2;
        else
            verticalVelocity = jumpSpeed;

        //set up our velocity horizontally and our max Vel speed
        horizontalVelocity = force * jumpHVelForceMultiplier;
        currentMaxHVel = horizontalVelocity;

    }

    //similar to the jump with vector, although this time our vertical force will be determined by player percentages
    //hitForces will be in pos/neg axes.
    public void Knockback(float horizontalHitForce, float verticalHitForce)
    {

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

