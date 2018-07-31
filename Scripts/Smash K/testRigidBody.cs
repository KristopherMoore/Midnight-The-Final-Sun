using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRigidBody : MonoBehaviour {

    private bool isGrounded;
    private static Rigidbody rigidbody;

    private Vector3 moveVect;

	// Use this for initialization
	void Start ()
    {
        rigidbody = GetComponent("Rigidbody") as Rigidbody;
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, maxDistance: .3f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            isGrounded = true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            isGrounded = false;
        }

        float xAxis = Input.GetAxis("Horizontal");

        //check if input exceeds the deadZone (check for a minimal amount of input before moving)
        if (xAxis > .01 || xAxis < -.01)
        {
            moveVect += new Vector3(xAxis, 0, 0);
        }

        //rigidbody.MovePosition(moveVect);

    }

    private void LateUpdate()
    {
        rigidbody.MovePosition(moveVect);
    }
}
