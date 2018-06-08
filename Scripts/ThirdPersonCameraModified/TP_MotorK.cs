using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_MotorK : MonoBehaviour {


    public static TP_MotorK Instance; //hold reference to instance of itself  

    public float WalkSpeed = 2f;
    public float JogSpeed = 3f;
    public float RunSpeed = 6f;
    public float SlideSpeed = 3f;
    public float jumpSpeed = 6f;
    public float Gravity = 21f;
    public float terminalVelocity = 20f;
    public float slideThreshold = 0.6f;
    public float maxControllableSlideMagnitude = 0.4f;

    public enum direction
    {
        Stationary, Forward, Left, Backward, Right, LeftFwd, RightFwd, LeftBck, RightBck
    }

    public direction MoveDirection { get; set; }

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
        //SnapAlignCharacterWithCamera();
        ProcessMotion();
    }

    void ProcessMotion()
    {
        //Transform MoveVector to WorldSpace relative to our characters location
        MoveVector = transform.TransformDirection(MoveVector);

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

        //Move the Character in World Space
        TP_ControllerK.CharacterController.Move(MoveVector * Time.deltaTime); //Multiply MoveVector by DeltaTime (converts to units per second) right before we move

    }

    void applyGravity()
    {
        if(MoveVector.y > -terminalVelocity)
        {
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - (Gravity * Time.deltaTime), MoveVector.z);
        }
        if(TP_ControllerK.CharacterController.isGrounded && MoveVector.y < - 1)
        {
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
        }
    }

    void applySlide()
    {
        if (!TP_ControllerK.CharacterController.isGrounded)
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
        if(TP_ControllerK.CharacterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }
    }

    void SnapAlignCharacterWithCamera()
    {
        if(MoveVector.x != 0  || MoveVector.z != 0)
        {
            //here we define the rotation as the x and z values (vertical and hori movement) from the character itself, and the y changes from the camera.
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, transform.eulerAngles.z); 
        }
    }

    float MoveSpeed()
    {
        float moveSpeed = 0;
        determineCurrentMoveDirection();

        switch(Instance.MoveDirection)
        {
            case direction.Stationary:
                    moveSpeed = 0;
                    break;

            case direction.Forward:
                moveSpeed = JogSpeed;
                break;

            case direction.Backward:
                moveSpeed = WalkSpeed;
                break;

            case direction.Left:
                moveSpeed = JogSpeed;
                break;

            case direction.LeftBck:
                moveSpeed = WalkSpeed;
                break;

            case direction.LeftFwd:
                moveSpeed = JogSpeed;
                break;

            case direction.Right:
                moveSpeed = JogSpeed;
                break;

            case direction.RightBck:
                moveSpeed = WalkSpeed;
                break;

            case direction.RightFwd:
                moveSpeed = JogSpeed;
                break;
        }

        if (slideDirection.magnitude > 0)
            moveSpeed = SlideSpeed;

        return moveSpeed;
    }

    public void determineCurrentMoveDirection()
    {
        bool forward = false;
        bool backward = false;
        bool left = false;
        bool right = false;

        if (Instance.MoveVector.z > 0) //are we moving forward
            forward = true;
        if (Instance.MoveVector.z < 0) //are we moving backward
            backward = true;
        //if (Instance.MoveVector.x < 0) //are we moving left
            //left = true;
        //if (Instance.MoveVector.x > 0) //are we moving right
           // right = true;

        
        if (forward)
        {
            if (left)
            {
                MoveDirection = direction.LeftFwd;
            }
            else if (right)
            {
                Debug.Log(Instance.MoveVector.x);

                MoveDirection = direction.RightFwd;
            }
            else
            {
                MoveDirection = direction.Forward;
                //animator.SetBool("isWalkingFwd", true);
                //PlayerModelPartsToAnimate[0].GetComponent<Animator>().SetBool("isWalking", true);
            }
        }
        else if (backward)
        {
            if (left)
                MoveDirection = direction.LeftBck;
            else if (right)
                MoveDirection = direction.RightBck;
            else
                MoveDirection = direction.Backward;
        }

        else if (left)
            MoveDirection = direction.Left;

        else if (right)
            MoveDirection = direction.Right;

        else //not moving
            MoveDirection = direction.Stationary;

    }
}


