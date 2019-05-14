using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to handle a HitScan based raycast system. Extremely simple, but necessry for HitScan based shooting
public class HitScanRayCast : MonoBehaviour {

    public GameObject cameraFocusPoint;
    public GameObject mainCamera;

    private bool rayCollided = false;
    private float maxRayDistance = 1000000;

    //initialization
    private void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
        mainCamera = GameObject.FindWithTag("MainCamera");
    }

    // update after each frame
    void LateUpdate()
    {
        RaycastHit hit;

        //get Direction from the two points in space
        Vector3 laserDirection = (cameraFocusPoint.transform.position - mainCamera.transform.position);

        //setup ray, and first point for each Renderer; Since they will always start at the laser sights model.
        Ray ray = new Ray(mainCamera.transform.position, laserDirection);

        //shoot our ray towards the camera focus point. check for collision, to stop the laser where we collided.
        if (Physics.Raycast(mainCamera.transform.position, laserDirection, out hit, maxRayDistance))
        {
            rayCollided = true;
        }

        //if the ray collided with some object
        if (rayCollided)
        {
            
        }
        else
        {

        }

        rayCollided = false;

    }
}
