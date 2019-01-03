using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPointAtFocus : MonoBehaviour {

    public GameObject cameraFocusPoint;

    //initialization
    private void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
    }

    // update after each frame
    void LateUpdate()
    {
        Debug.DrawRay(this.transform.position, this.transform.rotation * (Vector3.forward * 15), Color.blue);

        transform.LookAt(cameraFocusPoint.transform);
    }
}
