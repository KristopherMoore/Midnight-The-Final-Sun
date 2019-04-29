using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraMovement : MonoBehaviour {

    GameObject mainCam;

    Vector3 cameraPosition1pos;
    Quaternion cameraPosition1rot;
    Vector3 cameraPosition2pos;
    Quaternion cameraPosition2rot;

    private int desiredPositionSet;
    private float smoothSpeed = 1;

    // Use this for initialization
    void Start ()
    {
        desiredPositionSet = 1;

        mainCam = GameObject.Find("Main Camera");

        cameraPosition1pos = mainCam.transform.position;
        cameraPosition1rot = mainCam.transform.rotation;

        cameraPosition2pos = GameObject.Find("Camera2").transform.position;
        cameraPosition2rot = GameObject.Find("Camera2").transform.rotation;
    }

    private void Update()
    {
        if (desiredPositionSet == 1)
        {
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, cameraPosition1rot, smoothSpeed * Time.deltaTime);
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraPosition1pos, smoothSpeed * Time.deltaTime);
        }

        else if (desiredPositionSet == 2)
        {
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, cameraPosition2rot, smoothSpeed * Time.deltaTime);
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraPosition2pos, smoothSpeed * Time.deltaTime);
        }
    }

    //when our mouse enters
    private void OnMouseEnter()
    {
        desiredPositionSet = 2;
    }

    //when our mouse exits the object
    private void OnMouseExit()
    {
        desiredPositionSet = 1;
    }
}
