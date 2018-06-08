using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusControl : MonoBehaviour
{

    public static CameraFocusControl Instance;


    //public edit values, will use this to refine the rotation focus, then take away the control
    public Transform anchorAround;


    //IMPORTANT, these values control where the anchor starts in ragard to the player, rotation and distance
    private float StartOffsetRotationHorizontal = 10f;  //positive shifts the focus point the the "right of the player" starting orientation
    private float StartOffsetRotationVertical = 0f;  //negative shifts the focus point to the "top of the player" (upwards) starting orientation
    private float StartOffsetDistance = -10f;        //negative shifts the distance away from the player in front fromt he starting position.

    public float Distance = -25f;
    public float DistanceMin = -100f;
    public float DistanceMax = 100f;
    public float DistanceSmooth = 0.05f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;

    //IMPORTANT, i found best values to be a differential of 100, so my settings has camera with -30, 70, and the camera focus with -50, 50. And they stop exactly together
    public float Y_MinLimit = -50f;   //for the camera focus, this limits how high upwards we can aim (towards the sky)  the higher negatives = higher aim
    public float Y_MaxLimit = 50f;    //inverse, controls how low we can aim, higher numbers = lower we can aim

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    //private float startDistance = 50f;
    private Vector3 position = Vector3.zero;
    private Vector3 desiredPosition = Vector3.zero;
    private float desiredDistance = 0f;

    void Awake()
    {
        Instance = this;
    }


    // Use this for initialization
    void Start()
    {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax); //set up our distance based on the min and max values, and where we are currently
        //startDistance = Distance;
        Reset();
    }

    //after update frams
    void LateUpdate()
    {
        if (anchorAround == null) //if we arent anchoring around anything (for our focus point we will want to anchor around the player so we roate around them)
            return;

        HandlePlayerInput();

        CalculateDesiredPosition();

        UpdatePosition();
    }

    void HandlePlayerInput() //function to get and handle mouse input
    {

        //get Axis input
        mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;

        //Limit mouse Y rotation here
        mouseY = HelperK.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

    }

    void CalculateDesiredPosition() //find out where we want to be
    {
        //Evaluate distance
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, DistanceSmooth); //find out where we want to be distance wise

        //Calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance); //based on the given changes to the vertical and horizontal planes (may need to invert here for consistency with camera), and our distance
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance) //takes in our (mouseY, mouseX, Distance) variables and finds our new position from our desired position
    {
        Vector3 direction = new Vector3(0, 0, -distance); //calc direction to rotate
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0); //find out how we can rotate
        return anchorAround.position + rotation * direction; //use the anchor around position then rotate by rotation and direction
    }

    void UpdatePosition()
    {
        //find positions individually
        var posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);

        position = new Vector3(posX, posY, posZ); //convert to a Vector3

        transform.position = position;  //change the position of our focus control point

        //transform.LookAt(TargetLookAt);  //shouldnt need to focus on anything, just rotate around anchor
    }

    public void Reset() //public incase we ever need to reset the camera externally
    {
        mouseX = StartOffsetRotationHorizontal;
        mouseY = StartOffsetRotationVertical;
        Distance = StartOffsetDistance;
        desiredDistance = Distance;
    }

}
