using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_AnimatorK : MonoBehaviour {

    private Animator[] playerBodyPartsToAnimate;

	// Use this for initialization
	void Start ()
    {
        Debug.Log(transform.childCount);
        //playerBodyPartsToAnimate[0] = transform.GetChild(0).gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(TP_MotorK.Instance.MoveDirection == TP_MotorK.direction.Forward)
        {
            //playerBodyPartsToAnimate[0].GetComponent<Animator>().SetBool("isWalkingFwd", true);
        }
	}
}
