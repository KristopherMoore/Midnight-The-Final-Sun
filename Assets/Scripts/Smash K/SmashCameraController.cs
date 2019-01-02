using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashCameraController : MonoBehaviour
{

    //Camera class that handles functionality such as the camera, its anchor pivot, its orientation to the player, its focus target, and occlusion checks and fixes.


    public static SmashCameraController Instance;

    //IMPORTANT, these values control where the anchor starts in ragard to the player, rotation and distance
    private float StartOffsetRotationHorizontal = 0f;  //positive shifts the camera the the "left of the player" starting orientation
    private float StartOffsetRotationVertical = 30f;  //negative shifts the camera to the "bottom of the player" (under) starting orientation
    private float StartOffsetDistance = 3.7f;        //negative shifts the distance away from the player in front fromt he starting position.

    public Transform cameraAnchorAround;
    
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

    //IMPORTANT, i found best values to be a differential of 100, so my settings has camera with -30, 70, and the camera focus with -50, 50. And they stop exactly together
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
    private float zoomSpeed = 0.05f;
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
        if (cameraAnchorAround == null) //if we arent looking at anything
            return;

        //if (GameMenu.Instance.getMenuStatus() == true)
            //return;

        //HandlePlayerInput();

        CheckCameraPoints(cameraAnchorAround.position, desiredPosition);

        CalculateDesiredPosition();
            
        UpdatePosition();

        CameraZoomController();
    }


    //TODO: Modify this to handle zoom based on character distance from each other.
    private void CameraZoomController()
    {
        //check if the player wants to zoom
        if (Input.GetMouseButton(1) == true) //if holding RMB down
        {
            if (tValue < 1)
            {
                tValue += zoomSpeed; //increment t based and how fast we want to zoom, how quickly it can add up to 1
                //Camera.main.fieldOfView = Mathf.Lerp(90, 60, tValue); //go from 90 fov to 60 fov
            }

            //Lerp values of camera position and to its right along the red axis.
            float lerpX = Mathf.Lerp(transform.position.x, transform.position.x + -transform.forward.x, tValue);
            float lerpY = Mathf.Lerp(transform.position.y, transform.position.y + -transform.forward.y, tValue);
            float lerpZ = Mathf.Lerp(transform.position.z, transform.position.z + -transform.forward.z, tValue);
            Vector3 newPosition = new Vector3(lerpX, lerpY, lerpZ);

            //set our new position on the camera object
            transform.SetPositionAndRotation(newPosition, transform.rotation);

        }

        else //if we let go of RMB
        {
            if (tValue > 0) //check if we have moved our t value at all , meaning we have zoomed in
            {
                tValue -= zoomSpeed; //increment t based and how fast we want to zoom, how quickly it can add up to 1
                //Camera.main.fieldOfView = Mathf.Lerp(90, 60, tValue);  //keep, order, the change in t going downward will account for us going from 60 to 90
            }

            //Lerp values of camera position and to its right along the red axis.
            float lerpX = Mathf.Lerp(transform.position.x, transform.position.x + -transform.forward.x, tValue);
            float lerpY = Mathf.Lerp(transform.position.y, transform.position.y + -transform.forward.y, tValue);
            float lerpZ = Mathf.Lerp(transform.position.z, transform.position.z + -transform.forward.z, tValue);
            Vector3 newPosition = new Vector3(lerpX, lerpY, lerpZ);

            //set our new position on the camera object
            transform.SetPositionAndRotation(newPosition, transform.rotation);
        }
    }

    void CalculateDesiredPosition()
    {
        //Evaluate distance
        ResetDesiredDistance();
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

        //Calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return cameraAnchorAround.position + rotation * direction;
    }


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

        if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
            nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, to + transform.forward * -GetComponent<Camera>().nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
            if (hitInfo.distance < nearestDistance || nearestDistance == -1) //if the new hit closer? or if we havent otherwise hit anything
                nearestDistance = hitInfo.distance;


        return nearestDistance;
    }

    void ResetDesiredDistance()
    {
        if (desiredDistance < preOccludedDistance) //are we closer than the starting distance
        {
            Vector3 pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

            //we are going to use this new point and test if it can be moved to without occluding
            var nearestDistance = CheckCameraPoints(cameraAnchorAround.position, pos);

            if (nearestDistance == -1 || nearestDistance > preOccludedDistance) //no collisions or its nearest distance is greater than our pre occluded distance
            {
                desiredDistance = preOccludedDistance;
            }
        }
    }

    void UpdatePosition()
    {
        //find positions individually
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);

        position = new Vector3(posX, posY, posZ);

        transform.position = position;  //move the camera to the appropriate position in world space

    }

    private void Reset()
    {
        mouseX = StartOffsetRotationHorizontal;
        mouseY = StartOffsetRotationVertical;
        Distance = StartOffsetDistance;
        desiredDistance = Distance;

        preOccludedDistance = Distance;
    }

}
