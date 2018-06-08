using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterMotor : MonoBehaviour
{


    public static PlayerCharacterMotor Instance; //hold reference to instance of itself  

    public float WalkSpeed = 2f;
    public float JogSpeed = 4f;
    public float RunSpeed = 6f;
    public float SlideSpeed = 3f;
    public float jumpSpeed = 6f;
    public float Gravity = 21f;
    public float terminalVelocity = 20f;
    public float slideThreshold = 0.6f;
    public float maxControllableSlideMagnitude = 0.4f;


    private Vector3 slideDirection;


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

        //Kris -- Modify MoveVector relative to our cameras rotation
        Vector3 cameraDirection = Camera.main.transform.TransformDirection(MoveVector.x, MoveVector.y, MoveVector.z);
        cameraDirection.y = 0.0f;
        MoveVector = cameraDirection;

        //Normalize MoveVector if Magnitude > 1
        if (MoveVector.magnitude > 1)
            MoveVector = Vector3.Normalize(MoveVector);

        //apply slide, if needed (checks if we will slide, or not then does if so)
        applySlide();

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

    void applySlide()
    {
        if (!PlayerCharacterController.CharacterController.isGrounded)
            return; //cant slide in the air
        slideDirection = Vector3.zero;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))   //raycast from the character postion + 1 unti upwards., and shoot downwards, save the info into HitInfo
        {
            if (hitInfo.normal.y < slideThreshold) //if the y axis is at a steeper angle than our slide threshold
                slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z); //inverted y as we want to go downward
        }

        if (slideDirection.magnitude < maxControllableSlideMagnitude)
            MoveVector += slideDirection;
        else
        {
            MoveVector = slideDirection;
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
        float moveSpeed = 0;

        moveSpeed = JogSpeed;
        //HERE, is where we can check if sprinting or walking

        if (slideDirection.magnitude > 0)
            moveSpeed = SlideSpeed;

        return moveSpeed;
    }

}


