//Program Information///////////////////////////////////////////////////////////
/*
 * @file AnimateDsBody.cs
 *
 *
 * @game-version 0.77 
 *          Kristopher Moore (19 May 2019)
 *          Modifications to Animations to support new animations, two handed states, rolls, attacks
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
        //call out to each of the animator updates.
        setMotionAxes();
        setAttacking();
        setAttackLock();
        setAiming();
        setJumping();
        setRolling();
        set2hStatus();
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

    //see above, for outside methods to set aiming anim state
    public void setAiming()
    {
        animator.SetBool("isAiming", pController.isAiming);
    }

    //see above, for outside methods to set attacking anim state
    public void setAttacking()
    {
        animator.SetBool("isAttackLocked", pController.isAttackLocked);
    }

    //see above, for outside methods to set attacking anim state
    public void setAttackLock()
    {
        animator.SetInteger("r1chain", pController.R1attackChain);
        animator.SetInteger("r2chain", pController.R2attackChain);
    }

    //see above, for outside methods to set jumping anim state
    public void setJumping()
    {
        animator.SetBool("isJumping", pController.isJumping);
    }

    //see above, for outside methods to set rolling anim state
    public void setRolling()
    {
        animator.SetBool("isRolling", pController.isRolling);
    }

    //see above, for outside methods to set 2h anim state
    public void set2hStatus()
    {
        animator.SetBool("is2h", pController.is2h);
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
