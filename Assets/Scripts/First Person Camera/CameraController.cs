using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //Camera class that handles functionality such as the camera, its anchor pivot, its orientation to the player, its focus target, and occlusion checks and fixes.


    public static CameraController Instance;

    //IMPORTANT, these values control where the anchor starts in ragard to the player, rotation and distance
    private float StartOffsetRotationHorizontal = 0;  //positive shifts the camera the the "left of the player" starting orientation
    private float StartOffsetRotationVertical = 0;  //negative shifts the camera to the "bottom of the player" (under) starting orientation
    private float StartOffsetDistance = 0;        //negative shifts the distance away from the player in front fromt he starting position.

    public Transform cameraAnchorAround;
    public Transform cameraFocusPoint;
    public float Distance = 1f;
    public float DistanceMin = 1f;
    public float DistanceMax = 25f;
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 1f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.15f;
    public float Y_Smooth = 0.15f;

    //IMPORTANT, I found best values to be a differential of 100, so my settings has camera with -30, 70, and the camera focus with -50, 50. And they stop exactly together
    public float Y_MinLimit = -30f;
    public float Y_MaxLimit = 70f;    //controls how far our camera can rotate on its axis, needs to be tuned along with CameraFocusControl's values (NOT THE SAME), so they work well together

    public float OcculsionDistanceStep = 0.5f;
    public int maxOcclusionChecks = 10;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    private float startDistance = 0f;
    private Vector3 position = Vector3.zero;
    private Vector3 desiredPosition = Vector3.zero;
    private float desiredDistance = 0f;
    private float distanceSmooth = 0f;
    private float preOccludedDistance = 0;


    //values for the cameraZoom
    private float zoomSpeed = 0.1f;
    float tValue = 0f;  //will be used in a lerp, 0 is at min, .5 is go halfway, 1 is go to the new point //we want to start at 0 as we want to start at 90 and lerp toward 60
    private float deadZone = 0.19f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;
        Reset();
    }

    void LateUpdate()
    {
        Debug.DrawRay(this.transform.position, this.transform.rotation * (Vector3.forward * 100), Color.red);

        if (cameraAnchorAround == null) //if we arent looking at anything
            return;

        HandlePlayerInput();

        //move camera position with player
        transform.position = cameraAnchorAround.position;

        //have camera focus on the focus point (Dont need it smoothed in this case.)
        transform.LookAt(cameraFocusPoint);

    }


    void HandlePlayerInput()
    {

        //get Axis input
        mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;

        //check for controller input
        if (Input.GetAxis("RightJoyHorizontal") > deadZone || Input.GetAxis("RightJoyHorizontal") < -deadZone)
            mouseX += Input.GetAxis("RightJoyHorizontal") * X_MouseSensitivity;
        if (Input.GetAxis("RightJoyVertical") > deadZone || Input.GetAxis("RightJoyVertical") < -deadZone)
            mouseY -= Input.GetAxis("RightJoyVertical") * Y_MouseSensitivity;


        // Limit mouse Y rotation here
        mouseY = HelperK.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

    }



    private void Reset()
    {
        mouseX = StartOffsetRotationHorizontal;
        mouseY = StartOffsetRotationVertical;
        Distance = StartOffsetDistance;
        desiredDistance = Distance;

        preOccludedDistance = Distance;
    }


    public static void UseExistingOrCreateNewMainCamera()
    {
        GameObject tempCamera;
        GameObject cameraAnchorAround;
        CameraController myCamera;

        if (Camera.main != null) //if it exists
        {
            tempCamera = Camera.main.gameObject;
        }
        else //didnt exist
        {
            tempCamera = new GameObject("Main Camera");
            tempCamera.AddComponent<Camera>();
            tempCamera.tag = "MainCamera";
        }

        tempCamera.AddComponent<CameraController>();
        myCamera = tempCamera.GetComponent<CameraController>() as CameraController;


        //now we deal with the Target Look at
        cameraAnchorAround = GameObject.Find("cameraAnchorAround") as GameObject;

        if (cameraAnchorAround == null) //we didnt find anything
        {
            cameraAnchorAround = new GameObject("cameraAnchorAround");
            cameraAnchorAround.transform.position = Vector3.zero; //position target focus point at world origin
        }

        myCamera.cameraAnchorAround = cameraAnchorAround.transform;
    }
}
