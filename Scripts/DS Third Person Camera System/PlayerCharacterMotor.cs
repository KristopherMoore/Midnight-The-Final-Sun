﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterMotor : MonoBehaviour
{


    public static PlayerCharacterMotor Instance; //hold reference to instance of itself  

    public float WalkSpeed = 1.75f;
    public float JogSpeed = 3.75f;
    public float RunSpeed = 6.0f;
    public float SlideSpeed = 3f;
    public float jumpSpeed = 6f;
    public float Gravity = 21f;
    public float terminalVelocity = 20f;
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
                //TO BE REMOVED no longer useful for our DS style movement, could be useful when doing lock on system
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

        //Apply Gravity
        applyGravity();

        //Move the Character in World Space,if not animation locked
        if(!PlayerCharacterController.Instance.isAnimationLocked)
        {
            PlayerCharacterController.CharacterController.Move(MoveVector * Time.deltaTime); //Multiply MoveVector by DeltaTime (converts to units per second) right before we move

            //Kris -- Modify Rotation to direction in which we moved
            if (PlayerCharacterController.Instance.isMoving)
            {
                Vector3 toRot = new Vector3(MoveVector.x, 0f, MoveVector.z);
                transform.rotation = Quaternion.LookRotation(toRot);
            }
        }
    }

    void applyGravity()
    {
        if (MoveVector.y > -terminalVelocity)
        {
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * Time.deltaTime), MoveVector.z);
        }
        if (PlayerCharacterController.CharacterController.isGrounded && MoveVector.y < -1)
        {
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
        }
    }

    public void Jump()
    {
        if (PlayerCharacterController.CharacterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }
    }

    float MoveSpeed()
    {
        float moveSpeed = JogSpeed;

        if (PlayerCharacterController.Instance.playerAnimationState == PlayerCharacterController.animationState.Walking)
            moveSpeed = WalkSpeed;
      
        if (PlayerCharacterController.Instance.playerAnimationState == PlayerCharacterController.animationState.Running)
            moveSpeed = RunSpeed;

        return moveSpeed;
    }

}


