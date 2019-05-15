//Program Information///////////////////////////////////////////////////////////
/*
 * @file CameraController.cs
 *
 *
 * @game-version 0.72 
 *          Kristopher Moore (14 May 2019)
 *          Modifications to Player Motor for interaction with 3rd person changes
 *          
 *          Camera class that handles functionality such as the camera, its anchor pivot, its orientation to the player, 
 *          its focus target, and occlusion checks and fixes.
 *          Also Allows for switching between 3rd and 1st Person Camera States
 *          
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController Instance;

    //IMPORTANT, these values control where the anchor starts in ragard to the player, rotation and distance
    private float StartOffsetRotationHorizontal = 0f;  //positive shifts the camera the the "left of the player" starting orientation
    private float StartOffsetRotationVertical = 0f;  //negative shifts the camera to the "bottom of the player" (under) starting orientation 
                                                     //NOTE: keep this zero, and modify the body anchor points for Vertical offset, this value will unalign the camera / focus point degree rotations
    private float StartOffsetDistance = 3f;        //negative shifts the distance away from the player in front fromt he starting position.

    //anchor points transforms.
    public Transform cameraAnchorAround;
    public Transform cameraFocusPoint;
    public Transform cameraAnchorHead;
    public Transform cameraAnchorBody;

    private float Distance = 2f;
    private float DistanceMin = 1f;
    private float DistanceMax = 25f;
    private float DistanceSmooth = 0.05f;
    private float DistanceResumeSmooth = 1f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.15f;
    public float Y_Smooth = 0.15f;

    //IMPORTANT, to have the camera focus point camera stay aligned they MUST have the same limit. AND it must be aligned with The Camera Focus Control's
    private float Y_MinLimit = -80f;   //for the camera focus, this limits how high upwards we can aim (towards the sky)  the higher negatives = higher aim
    private float Y_MaxLimit = 80f;    //inverse, controls how low we can aim, higher numbers = lower we can aim

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

    private float smoothSpeed = 5;

    //values for the cameraZoom
    private float zoomSpeed = 0.1f;
    float tValue = 0f;  //will be used in a lerp, 0 is at min, .5 is go halfway, 1 is go to the new point //we want to start at 0 as we want to start at 90 and lerp toward 60
    private float deadZone = 0.19f;

    void Awake()
    {
        Instance = this;
    }

    //initializations on script load
    void Start()
    {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;
        Reset();

        //find anchor points / set based on our current camera perspective
        cameraAnchorHead = GameObject.Find("AnchorPointHead").transform;
        cameraAnchorBody = GameObject.Find("AnchorPointBody").transform;
        this.setCameraAnchor();

    }

    //using LateUpdate to have these camera calcs happen after Game Logic
    void LateUpdate()
    {
        Debug.DrawRay(this.transform.position, this.transform.rotation * (Vector3.forward * 100), Color.red);

        if (cameraAnchorAround == null) //if we arent looking at anything
            return;
        //if we are, then set the anchor based on perspective.
        this.setCameraAnchor();

        HandlePlayerInput();

        //move camera position with player
        transform.position = cameraAnchorAround.position;

        //check camera state, if 3rd person camera, then apply positon with offset.
        if (PlayerCharacterController.Instance.is1stPersonCamera == false)
        {
            CheckCameraPoints(cameraAnchorAround.position, desiredPosition);

            int count = 0;
            do
            {
                CalculateDesiredPosition();
                count++;
            } while (CheckIfOccluded(count));

            UpdatePosition();
        }

        //have camera focus on the focus point (Dont need it smoothed in this case.)
        transform.LookAt(cameraFocusPoint);

    }

    //updates the camera physical posiition in world space, Smoothly based on our current/desired posititons
    void UpdatePosition()
    {
        //find positions individually
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);

        position = new Vector3(posX, posY, posZ);

        //move the camera to the appropriate position in world space
        transform.position = position;

        //have camera focus on the focus point
        transform.LookAt(cameraFocusPoint);
    }

    //Handle the players mouse inputs, and respond accordingly. Handles Controller input as well.
    void HandlePlayerInput()
    {

        //get Axis input
        mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;

        //check for controller input, and replace mouse values with controller inputs
        if (Input.GetAxis("RightJoyHorizontal") > deadZone || Input.GetAxis("RightJoyHorizontal") < -deadZone)
            mouseX += Input.GetAxis("RightJoyHorizontal") * X_MouseSensitivity;
        if (Input.GetAxis("RightJoyVertical") > deadZone || Input.GetAxis("RightJoyVertical") < -deadZone)
            mouseY -= Input.GetAxis("RightJoyVertical") * Y_MouseSensitivity;


        // Limit mouse Y rotation here, so we can't go over the top of our own head. X is not required and we can spin in circles as much as we want.
        mouseY = HelperK.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

    }

    //calcuates our DesiredPosition vector based on our distance and axis values
    void CalculateDesiredPosition()
    {
        //Evaluate distance
        ResetDesiredDistance();
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

        //Calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }

    //give a rotation and a distance, returns a vector our our new Camera position with rotational offsets
    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return cameraAnchorAround.position + rotation * direction;
    }

    //Main Camera Occlusion method, check if we are occluded and our count of occluded states
    //Occlusion is the process of the camera being blocked from its ideal target. This process will allow us to step closer as occlusions happen
    bool CheckIfOccluded(int count)
    {
        bool isOccluded = false;

        var nearestDistance = CheckCameraPoints(cameraAnchorAround.position, desiredPosition);

        //something was hit, ie: we are being Occluded by an object
        if (nearestDistance != -1) 
        {
            //take steps towards the player, if occlusion still occurs check again.
            if (count < maxOcclusionChecks)
            {
                isOccluded = true;
                Distance -= OcculsionDistanceStep;

                if (Distance < 0.25f)
                    Distance = 0.25f;
            }
            //else we have exceeded max occlusion checks. Move camera NOW, or the player wont be able to see
            else 
                Distance = nearestDistance - Camera.main.nearClipPlane;

            desiredDistance = Distance;
            distanceSmooth = DistanceResumeSmooth;

        }

        return isOccluded;
    }

    //reset of all primary values, necessary in some cases when modifying cameraStates
    private void Reset()
    {
        mouseX = StartOffsetRotationHorizontal;
        mouseY = StartOffsetRotationVertical;
        Distance = StartOffsetDistance;
        desiredDistance = Distance;

        preOccludedDistance = Distance;
    }

    //resets our desired distance to the preOccluded Distance, this way we can smoothly go back to original state
    void ResetDesiredDistance()
    {
        //if we are closer than the starting distance
        if (desiredDistance < preOccludedDistance) 
        {
            Vector3 pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

            //we are going to use this new point and test if it can be moved to without occluding
            var nearestDistance = CheckCameraPoints(cameraAnchorAround.position, pos);

            //no collisions or its nearest distance is greater than our pre occluded distance, set back to preOccluded
            if (nearestDistance == -1 || nearestDistance > preOccludedDistance) 
            {
                desiredDistance = preOccludedDistance;
            }
        }
    }

    //helper function for the Occlusion system, given two vectors it checks the Clip Plane points and returns the nearest distance we can reach without occluding
    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
        var nearestDistance = -1f;
        RaycastHit hitInfo;
        HelperK.ClipPlanePoints clipPlanePoints = HelperK.ClipPlaneAtNear(to);

        //debugging lines creation
        Debug.DrawLine(from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.UpperLeft);
        Debug.DrawLine(from, clipPlanePoints.LowerLeft);
        Debug.DrawLine(from, clipPlanePoints.UpperRight);
        Debug.DrawLine(from, clipPlanePoints.LowerRight);

        Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
        Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
        Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
        Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

        //ensure at each step we do not hit irrelevant colliders (Player (Ourselves), Enemies, or Objects specifically marked to not have CameraCollisions)
        if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "IgnoreCameraCollision" && hitInfo.collider.tag != "Enemy")
            nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "IgnoreCameraCollision" && hitInfo.collider.tag != "Enemy")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "IgnoreCameraCollision" && hitInfo.collider.tag != "Enemy")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "IgnoreCameraCollision" && hitInfo.collider.tag != "Enemy")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "IgnoreCameraCollision" && hitInfo.collider.tag != "Enemy")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, out hitInfo))
        {
            Debug.Log(hitInfo.collider.tag);
            Debug.Log(hitInfo.collider.name);
        }

        return nearestDistance;
    }

    //Checks for existence of a MainCamera object, if none exists, then create our own //TODO: Further upon this for cleanliness of object creations
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

    //method to check our camera perspective and set the anchor accordingly
    private void setCameraAnchor()
    {
        if (PlayerCharacterController.Instance.is1stPersonCamera)
            cameraAnchorAround = cameraAnchorHead;
        else
            cameraAnchorAround = cameraAnchorBody;
    }
}
