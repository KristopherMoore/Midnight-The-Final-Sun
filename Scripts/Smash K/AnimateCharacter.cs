using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCharacter : MonoBehaviour
{
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SmashCharacterController pController = SmashCharacterController.Instance;

        animator.SetBool("isWalking", false);
        animator.SetBool("isJogging", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isRolling", false);
        animator.SetBool("isIdling", false);
        animator.SetBool("isL1Attack", false);
        animator.SetBool("isAiming", false);
        animator.SetBool("isFired", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isDoubleJumping", false);
        animator.SetBool("isGliding", false);
        animator.SetBool("isFalling", false);
        animator.SetFloat("MotionState", 0);


        if (pController.playerAnimationState == SmashCharacterController.animationState.Idling)
        {
            animator.SetBool("isIdling", true);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Walking)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("MotionState", 0.333f);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Jogging)
        {
            animator.SetBool("isJogging", true);
            animator.SetFloat("MotionState", 0.666f);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Running)
        {
            animator.SetBool("isRunning", true);
            animator.SetFloat("MotionState", 1);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Rolling)
        {
            animator.SetBool("isRolling", true);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.L1Attack)
        {
            animator.SetBool("isL1Attack", true);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Aiming)
        {
            animator.SetBool("isAiming", true);
        }
        if (pController.firedBow)
            animator.SetBool("isFired", true);

        if (pController.playerAnimationState == SmashCharacterController.animationState.Gliding)
        {
            animator.SetBool("isGliding", true);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Jumping)
        {
            animator.SetBool("isJumping", true);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.DoubleJumping)
        {
            animator.SetBool("isDoubleJumping", true);
        }
        if (pController.playerAnimationState == SmashCharacterController.animationState.Falling)
        {
            animator.SetBool("isFalling", true);
        }

    }
}