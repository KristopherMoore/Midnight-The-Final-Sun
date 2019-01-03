using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRayGreen : MonoBehaviour {

    public GameObject cameraFocusPoint;

    //initialization
    private void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
    }

    // update after each frame
    void LateUpdate()
    {
        Debug.DrawLine(this.transform.position, cameraFocusPoint.transform.position, Color.green);

    }
}
