﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtCameraFocus : MonoBehaviour {

    public GameObject cameraFocusPoint;

    private float smoothSpeed = 5;

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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation , smoothSpeed * Time.deltaTime);
        
        //OLD
        //transform.LookAt(cameraFocusPoint.transform);

        Debug.DrawRay(this.transform.position, this.transform.rotation * (Vector3.forward * 15), Color.blue);
    }
}
