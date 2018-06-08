using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_FitPlayerToScreen : MonoBehaviour {

    public GameObject anchorTo;
    private float xOffset = 0;
    private float yOffset = -1;
    private float zOffset = 0;

    private Vector3 scale = new Vector3(.8f, .8f, .8f);

    // Use this for initialization
    void Start ()
    {
        Vector3 anchorPos = anchorTo.transform.position;
        Vector3 offsets = new Vector3(xOffset, yOffset, zOffset);

        Quaternion temp = new Quaternion();

        anchorPos += offsets; // add offsets to the anchors pos

        transform.localScale = scale;

        transform.SetPositionAndRotation(anchorPos, temp);
	}
	
}
