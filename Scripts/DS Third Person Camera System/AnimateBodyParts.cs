using System.Collections;
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

        if (pController.playerAnimationState == PlayerCharacterController.animationState.Walking)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (pController.playerAnimationState == PlayerCharacterController.animationState.Jogging)
        {
            animator.SetBool("isJogging", true);
        }
        else
        {
            animator.SetBool("isJogging", false);
        }

        if (pController.playerAnimationState == PlayerCharacterController.animationState.Running)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (pController.playerAnimationState == PlayerCharacterController.animationState.Rolling)
        {
            animator.SetBool("isRolling", true);
        }
        else
        {
            animator.SetBool("isRolling", false);
        }

    }
}
