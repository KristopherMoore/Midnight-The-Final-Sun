﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBodyParts : MonoBehaviour
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
        PlayerCharacterController pController = PlayerCharacterController.Instance;
 
        animator.SetBool("isWalking", false);
        animator.SetBool("isJogging", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isRolling", false);
        animator.SetBool("isIdling", false);
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
            //Mathf.Lerp()
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Running)
        {
            animator.SetBool("isRunning", true);
            animator.SetFloat("MotionState", 1);
        }
        if (pController.playerAnimationState == PlayerCharacterController.animationState.Rolling)
        {
            animator.SetBool("isRolling", true);
        }

    }
}