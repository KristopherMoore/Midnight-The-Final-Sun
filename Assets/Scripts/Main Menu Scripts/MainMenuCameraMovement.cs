//Program Information///////////////////////////////////////////////////////////
/*
 * @file MainMenuCameraMovement.cs
 *
 *
 * @game-version 0.71 
 *          Kristopher Moore (13 May 2019)
 *          Modification of Cam movement to incorporate the new characters, allows shifting of
 *          camera upon highlighting each character selection
 *          
 *          
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraMovement : MonoBehaviour {

    public static MainMenuCameraMovement Instance;

    GameObject mainCam;

    Vector3 cameraPosition1pos;
    Quaternion cameraPosition1rot;
    Vector3 cameraPosition2pos;
    Quaternion cameraPosition2rot;

    Vector3 knightCamPos;
    Quaternion knightCamRot;
    Vector3 darkKnightCamPos;
    Quaternion darkKnightCamRot;
    Vector3 dragonKnightCamPos;
    Quaternion dragonKnightCamRot;
    Vector3 sentinelCamPos;
    Quaternion sentinelCamRot;
    Vector3 clericCamPos;
    Quaternion clericCamRot;
    Vector3 ninjaCamPos;
    Quaternion ninjaCamRot;
    Vector3 sorceressCamPos;
    Quaternion sorceressCamRot;
    Vector3 pyromancerCamPos;
    Quaternion pyromancerCamRot;

    private string desiredPosition;
    private int desiredPositionSet;
    private float smoothSpeed = 2f;

    // Use this for initialization
    void Start ()
    {
        Instance = this;

        desiredPosition = "DEFAULT";
        desiredPositionSet = 1;

        mainCam = GameObject.Find("Main Camera");

        //grab all of our camera's positions. So we can rotate to them easily.
        cameraPosition1pos = mainCam.transform.position;
        cameraPosition1rot = mainCam.transform.rotation;

        cameraPosition2pos = GameObject.Find("Camera2").transform.position;
        cameraPosition2rot = GameObject.Find("Camera2").transform.rotation;

        knightCamPos = GameObject.Find("KnightCamera").transform.position;
        knightCamRot = GameObject.Find("KnightCamera").transform.rotation;

        darkKnightCamPos = GameObject.Find("DarkKnightCamera").transform.position;
        darkKnightCamRot = GameObject.Find("DarkKnightCamera").transform.rotation;

        dragonKnightCamPos = GameObject.Find("DragonKnightCamera").transform.position;
        dragonKnightCamRot = GameObject.Find("DragonKnightCamera").transform.rotation;

        sentinelCamPos = GameObject.Find("SentinelCamera").transform.position;
        sentinelCamRot = GameObject.Find("SentinelCamera").transform.rotation;

        clericCamPos = GameObject.Find("ClericCamera").transform.position;
        clericCamRot = GameObject.Find("ClericCamera").transform.rotation;

        ninjaCamPos = GameObject.Find("NinjaCamera").transform.position;
        ninjaCamRot = GameObject.Find("NinjaCamera").transform.rotation;

        sorceressCamPos = GameObject.Find("SorceressCamera").transform.position;
        sorceressCamRot = GameObject.Find("SorceressCamera").transform.rotation;

        pyromancerCamPos = GameObject.Find("PyromancerCamera").transform.position;
        pyromancerCamRot = GameObject.Find("PyromancerCamera").transform.rotation;
    }

    private void Update()
    {
        //use switch on text to determine where to move the camera.
        switch (desiredPosition)
        {
            case "Knight":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, knightCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, knightCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "DarkKnight":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, darkKnightCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, darkKnightCamPos, smoothSpeed * Time.deltaTime);
                break;
            
            case "DragonKnight":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, dragonKnightCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, dragonKnightCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "Sentinel":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, sentinelCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, sentinelCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "Cleric":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, clericCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, clericCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "Sorceress":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, sorceressCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, sorceressCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "Ninja":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, ninjaCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, ninjaCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "Pyromancer":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, pyromancerCamRot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, pyromancerCamPos, smoothSpeed * Time.deltaTime);
                break;

            case "DEFAULT":
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, cameraPosition1rot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraPosition1pos, smoothSpeed * Time.deltaTime);
                break;

            default:
                mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, cameraPosition1rot, smoothSpeed * Time.deltaTime);
                mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, cameraPosition1pos, smoothSpeed * Time.deltaTime);
                break;
        }

    }

    //allow Exterior scripts to modify the camera position. This will be done by the hoverable class names
    public void ChangeCameraPosition(string toSet)
    {
        desiredPosition = toSet; 
    }

}
