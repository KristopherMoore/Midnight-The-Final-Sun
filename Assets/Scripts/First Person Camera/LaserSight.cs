using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour {

    public GameObject cameraFocusPoint;
    private LineRenderer laserLine;
    private TrailRenderer laserTrail;

    private int maxLaserDistance = 1000000;
    private bool laserCollided = false;

    //initialization
    private void Start()
    {
        cameraFocusPoint = GameObject.FindWithTag("CameraFocusPoint");
        laserLine = gameObject.GetComponent<LineRenderer>();
        laserTrail = gameObject.GetComponent<TrailRenderer>();
        
        
        laserLine.enabled = true;
        laserTrail.enabled = true;
    }

    // update after each frame
    void Update()
    {
        RaycastHit hit;

        //get Direction from the two points in space
        Vector3 laserDirection = (cameraFocusPoint.transform.position - transform.position);

        Ray ray = new Ray(transform.position, laserDirection);
        laserLine.SetPosition(0, transform.position);
        laserTrail.AddPosition(transform.position);


        if (Physics.Raycast(laserLine.transform.position, laserDirection, out hit, maxLaserDistance))
        {
            laserCollided = true;
        }

        if (laserCollided)
        {
            laserTrail.enabled = true;
            laserLine.SetPosition(1, hit.point);
            laserTrail.AddPosition(hit.point);
        }
        else
        {
            laserTrail.enabled = false;
        }



        //IN PROGRESS. When not moving, we need a quick line renderer to replace the missing trail
        if(laserTrail.GetPosition(0) != Vector3.zero)
        {
            laserLine.enabled = false;
        }
        else
        {
            laserLine.enabled = true;
        }

        laserCollided = false;
        
    }

}
