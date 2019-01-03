using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSight : MonoBehaviour {

    public GameObject cameraFocusPoint;
    private LineRenderer laserLine;
    private TrailRenderer laserTrail;

    private int maxLaserDistance = 1000000;
    private bool laserCollided = false;
    private bool laserStatus = true;

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
    void LateUpdate()
    {
        //reset status of lasers to default true values, will be changed later by other statements, but for code efficiency lets just reset here.
        laserTrail.enabled = true;
        laserLine.enabled = true;

        //We need to check if the Trail renderer will work, this is based on player movement / cursor movement. If they arent moving, we need to use a Line Renderer as a standstill replacement.
        if(InputHandler.Instance.isCursorMoving() || PlayerCharacterController.Instance.isMoving)
        {
            laserTrail.enabled = true;
            laserLine.enabled = false;
        }
        else
        {
            laserLine.enabled = true;
            laserTrail.enabled = false;
        }

        //if the Laser if off overall, IE reloading and other anims.
        if(!laserStatus)
        {
            laserLine.enabled = false;
            laserTrail.enabled = false;
            return;
        }

        RaycastHit hit;

        //get Direction from the two points in space
        Vector3 laserDirection = (cameraFocusPoint.transform.position - transform.position);

        //setup ray, and first point for each Renderer; Since they will always start at the laser sights model.
        Ray ray = new Ray(transform.position, laserDirection);
        laserLine.SetPosition(0, transform.position);
        laserTrail.AddPosition(transform.position);

        //shoot our ray towards the camera focus point. check for collision, to stop the laser where we collided.
        if (Physics.Raycast(laserLine.transform.position, laserDirection, out hit, maxLaserDistance))
        {
            laserCollided = true;
        }

        //if we collided, add the collision hitpoint, else add the camera focus point's position
        if (laserCollided)
        {
            laserLine.SetPosition(1, hit.point);
            laserTrail.AddPosition(hit.point);
        }
        else
        {
            laserLine.SetPosition(1, cameraFocusPoint.transform.position);
            laserTrail.AddPosition(cameraFocusPoint.transform.position);
        }

        laserCollided = false;
        
    }

    public void setLaserStatus(bool toSet)
    {
        laserStatus = toSet;

        laserTrail.enabled = toSet;
    }

}
