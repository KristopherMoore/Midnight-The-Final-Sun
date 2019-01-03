using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateArms : MonoBehaviour
{
    private Animator animator;
    private PlayerCharacterController pController;

    // Use this for initialization
    void Start()
    {
        animator = transform.GetComponent<Animator>();

        pController = PlayerCharacterController.Instance;
    }

    void Update()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isJogging", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdling", false);
        animator.SetBool("isAiming", false);
        animator.SetBool("isFired", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isDoubleJumping", false);
        animator.SetBool("isGliding", false);
        animator.SetBool("isFalling", false);
        animator.SetFloat("MotionState", 0);


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
        if (pController.firedBow)
            animator.SetBool("isFired", true);

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
}
