using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to keep the Player arms rotating along with the camera's movement. This allows us to avoid using animations to make the player turn with camera.
//This not only makes the system more resource efficient but cuts down on code required.
public class PlayerArmsRotationAndPosition : MonoBehaviour {

    public GameObject cameraFocusPoint;

    private float smoothSpeed = 5;

    //arms local position offsets
    private Vector3 aimDownSightsCamOffset = new Vector3(0.128f, -3.015f, .35f);
    private Vector3 sideAimCamOffset = new Vector3(0.256f, -3.15f, .35f);

    //initialization
    private void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
    }


    // update after each frame
    void LateUpdate()
    {
        //get our new rotation by utilizing look rotation functionality
        Quaternion targetRotation = Quaternion.LookRotation(cameraFocusPoint.transform.position - transform.position);

        //rotate our transform to this rot in a smooth manner
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

        //REMEMBER ALL OFFSETS of parent controlled positions NEED to be done through local, otherwise we will keep reseting its world position and not its offset from that
        //check if we should be aiming or not, if so we need to LERP our way to that offset, if not then LERP back the otherway
        if(PlayerCharacterController.Instance.isAiming)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimDownSightsCamOffset, smoothSpeed * Time.deltaTime);
        }
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, sideAimCamOffset, smoothSpeed * Time.deltaTime);
    }
}
