using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPointAtFocus : MonoBehaviour {

    public GameObject cameraFocusPoint;

    private float smoothSpeed = 5;

    //initialization
    void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
    }

    
    // update after each frame
    void LateUpdate()
    {
        //if we are in a state where the weapon shouldnt be focused but instead follows the arms it is attached to
        if (PlayerCharacterController.Instance.isReloading)
            return;
        if (PlayerCharacterController.Instance.isRunning)
            return;

        //get our new rotation by utilizing look rotation functionality
        Quaternion targetRotation = Quaternion.LookRotation(cameraFocusPoint.transform.position - transform.position);

        //rotate our transform to this rot in a smooth manner
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

        Debug.DrawRay(this.transform.position, this.transform.rotation * (Vector3.forward * 15), Color.blue);
    }
        
}
