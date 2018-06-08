using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_AnimateBodyPart : MonoBehaviour {

    //TODO: This class is good and functional, though A rework of the movement system is in order.
    //For this game project worldspace Vertical and Horizontal movement are no longer desirable
    //See the written specs for that write up whats important here is we will be rebinding
    //The inputs and actions to a Input and Action manager class (Input will translate to the Action)
    //What we need here is a way for the Action manager to call to a AnimatePlayer function and
    //send it a parameter based on what Action happened. (Rolling, Walking Fwd, Running Back, Getting hit, ETC.)
    //then we will do what we are doing now set values based on which animations we want to play.



    private Animator animator;

	// Use this for initialization
	void Start ()
    {
        animator = transform.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        TP_MotorK.Instance.determineCurrentMoveDirection();
        if (TP_MotorK.Instance.MoveDirection == TP_MotorK.direction.Forward)
        {
            animator.SetBool("isWalkingFwd", true);
        }
        else if (TP_MotorK.Instance.MoveDirection == TP_MotorK.direction.Stationary)
        {
            animator.SetBool("isWalkingFwd", false);
        }
    }
}
