using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateArms : MonoBehaviour
{
    private Animator animator;
    private PlayerCharacterController pController;

    //public Instance so our Controllers can access this script.
    public static AnimateArms Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        animator = transform.GetComponent<Animator>();

        pController = PlayerCharacterController.Instance;

        //ensure starting values
        animator.SetBool("isFired", false);
        animator.SetBool("isReloading", false);
    }

    void Update()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isJogging", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdling", false);
        animator.SetBool("isAiming", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isDoubleJumping", false);
        animator.SetBool("isGliding", false);
        animator.SetBool("isFalling", false);
        animator.SetFloat("MotionState", 0);

        //REFACTOR THIS AWAY, be like the bottom methods. should be exterior calls, not everying in update again and again.
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Idling)
        {
            animator.SetBool("isIdling", true);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Walking)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("MotionState", 0.333f);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Jogging)
        {
            animator.SetBool("isJogging", true);
            animator.SetFloat("MotionState", 0.666f);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Running)
        {
            animator.SetBool("isRunning", true);
            animator.SetFloat("MotionState", 1);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Aiming)
        {
            animator.SetBool("isAiming", true);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Gliding)
        {
            animator.SetBool("isGliding", true);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Jumping)
        {
            animator.SetBool("isJumping", true);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.DoubleJumping)
        {
            animator.SetBool("isDoubleJumping", true);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Falling)
        {
            animator.SetBool("isFalling", true);
        }

    }

    //Outside methods can set our reloading anim, IE the player character controller. Limit the times we get Axis Input in each script.
    public void setReloading(bool toSet)
    {
        animator.SetBool("isReloading", toSet);
    }

    //see above, for outside methods to set fired anim state
    public void setFired(bool toSet)
    {
        animator.SetBool("isFired", toSet);
    }
}
