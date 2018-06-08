using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_ControllerK : MonoBehaviour {

    public static CharacterController CharacterController;
    public static TP_ControllerK Instance; //hold reference to current instance of itself

    private Animator animator;

    public float deadZone = 0.01f;

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;
        animator = GetComponent<Animator>(); //grabs an instance of the Animator object   
                                             
    }

  

    //
    void Update()
    {
        if (Camera.main == null) //check for camera, if no main camera, stop.
            return;

        GetLocomotionInput();
        HandleActionInput();

        TP_MotorK.Instance.UpdateMotor();

	}

    void GetLocomotionInput()
    {

        TP_MotorK.Instance.verticalVelocity = TP_MotorK.Instance.MoveVector.y;

        TP_MotorK.Instance.MoveVector = Vector3.zero; //reclacuate, prevents it from being additive. Basically restart on every update

        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //check if input exceeds the deadZone (check for a minimal amount of input before moving)
        if (yAxis > deadZone || yAxis < -deadZone)
            TP_MotorK.Instance.MoveVector += new Vector3(0, 0, yAxis);
        if (xAxis > deadZone || xAxis < -deadZone)
            TP_MotorK.Instance.MoveVector += new Vector3(xAxis, 0, 0);

        

        //modify Animator float states, so animations follow player inputs
        //animator.SetFloat("VelocityX", xAxis);
        //animator.SetFloat("VelocityY", yAxis);
        //animator.SetFloat("MouseY", -(Camera.main.transform.rotation.x * 2));

    }

    void HandleActionInput()
    {
        if (Input.GetKey(KeyCode.Space))
            Jump();
    }

    void Jump()
    {
        TP_MotorK.Instance.Jump();
    }
}
