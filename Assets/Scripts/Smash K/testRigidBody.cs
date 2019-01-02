using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRigidBody : MonoBehaviour {

    private static Rigidbody rb;

    private Vector3 moveVect;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent("Rigidbody") as Rigidbody;
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, maxDistance: .3f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
        }

        float xAxis = Input.GetAxis("Horizontal");

        //check if input exceeds the deadZone (check for a minimal amount of input before moving)
        if (xAxis > .01 || xAxis < -.01)
        {
            moveVect += new Vector3(xAxis, 0, 0);
        }

    }

    private void LateUpdate()
    {
        rb.MovePosition(moveVect);
    }
}
