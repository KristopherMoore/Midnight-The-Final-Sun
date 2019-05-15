//Program Information///////////////////////////////////////////////////////////
/*
 * @file AnimateDsBody.cs
 *
 *
 * @game-version 0.72 
 *          Kristopher Moore (14 May 2019)
 *          Modifications to Animations for interaction with 3rd person changes
 *          
 *          Class responsible for handling the animation states of our player
 *          dependent on the sets that occur within the PlayerCharacterController.
 *          
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateDsBody : MonoBehaviour {

    private Animator animator;
    private PlayerCharacterController pController;


    // Use this for initialization
    void Start()
    {
        animator = transform.GetComponent<Animator>();

        pController = PlayerCharacterController.Instance;

        //ensure these have a starting value
        resetAllAnimations();
    }

    //update each frame we are active
    private void Update()
    {
        setMotionAxes();
        setAiming();
        setJumping();
    }

    //Outside methods can set our motion Axes, IE the player character controller. Limit the times we get Axis Input in each script.
    public void setMotionAxes()
    {
        float XSet = pController.xAxis;
        float YSet = pController.yAxis;
        
        if (animator == null)
        {
            animator = transform.GetComponent<Animator>();
            Debug.Log("ANIMATOR RESET");
        }

        animator.SetFloat("MotionXAxis", XSet);
        animator.SetFloat("MotionYAxis", YSet);
    }

    //see above, for outside methods to set jumping anim state
    public void setAiming()
    {
        animator.SetBool("isAiming", pController.isAiming);
    }

    //see above, for outside methods to set jumping anim state
    public void setJumping()
    {
        animator.SetBool("isJumping", pController.isJumping);
    }

    //see above, for outside methods to set falling anim state
    public void setFalling()
    {
        animator.SetBool("isFalling", pController.isGliding);
    }


    public void resetAllAnimations()
    {
        //NOTE: do NOT reset our MotionAxes here, it is handled by a seperate call and never needs to be overriden.
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
    }
}
