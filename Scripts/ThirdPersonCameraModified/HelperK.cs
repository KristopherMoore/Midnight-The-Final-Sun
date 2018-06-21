﻿using UnityEngine;

public static class HelperK
{

    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        do
        {

            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;

        } while (angle < -360 || angle > 360);

        return Mathf.Clamp(angle, min, max);
    }

    public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos) //find the 4 points of the nearClipPlane and returns them
    {
        ClipPlanePoints clipPlanePoints = new ClipPlanePoints();

        if (Camera.main == null)
            return clipPlanePoints;

        var transform = Camera.main.transform;
        var halfFoV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;
        var aspect = Camera.main.aspect;
        var distance = Camera.main.nearClipPlane; //dist from cam to NCP
        var height = distance * Mathf.Tan(halfFoV);
        var width = height * aspect;

        clipPlanePoints.LowerRight = pos + transform.right * width;
        clipPlanePoints.LowerRight -= transform.up * height; //moving down by height, - to invert the up
        clipPlanePoints.LowerRight += transform.forward * distance;

        clipPlanePoints.LowerLeft = pos - transform.right * width;
        clipPlanePoints.LowerLeft -= transform.up * height; //moving down by height, - to invert the up
        clipPlanePoints.LowerLeft += transform.forward * distance;

        clipPlanePoints.UpperRight = pos + transform.right * width;
        clipPlanePoints.UpperRight += transform.up * height; //moving down by height, - to invert the up
        clipPlanePoints.UpperRight += transform.forward * distance;

        clipPlanePoints.UpperLeft = pos - transform.right * width;
        clipPlanePoints.UpperLeft += transform.up * height; //moving down by height, - to invert the up
        clipPlanePoints.UpperLeft += transform.forward * distance;


        return clipPlanePoints;
    }
}