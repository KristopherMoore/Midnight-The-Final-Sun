using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRayForwardBlue : MonoBehaviour {

    public GameObject cameraFocusPoint;

    //initialization
    private void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
    }

    // update after each frame
    void LateUpdate()
    {
        Debug.DrawLine(this.transform.position, Vector3.forward * 15, Color.blue);

    }
}
