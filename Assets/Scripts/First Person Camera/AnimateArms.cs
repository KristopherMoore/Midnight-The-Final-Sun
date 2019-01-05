using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateArms : MonoBehaviour
{

    //IMPORTANT NOTES: I have refactored these behaviors to no longer include updating function runs. Instead we pass off this redundancy to the 
    //player Character Controller class, where we are going to have some redundant statements, but Drastically reduce the times we Check the same steps in seperate updates.
    //Overall this change further optimizes runtimes by centralizing logic and statements in the character controller.

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

    }

    //so we can reset our animation states
    public void resetAllAnimations()
    {
        animator.SetBool("isFired", false);
        animator.SetBool("isIdling", false);
        animator.SetBool("isJogging", false);
        animator.SetBool("isReloading", false);
        animator.SetBool("isRunning", false);
    }

    public void setIdling(bool toSet)
    {
        animator.SetBool("isIdling", toSet);
    }

    public void setJogging(bool toSet)
    {
        animator.SetBool("isJogging", toSet);
    }

    //Outside methods can set our reloading anim, IE the player character controller. Limit the times we get Axis Input in each script.
    public void setReloading(bool toSet)
    {
        animator.SetBool("isReloading", toSet);
    }

    //see above, outside access to running anim state
    public void setRunning(bool toSet)
    {
        animator.SetBool("isRunning", toSet);
    }

    //see above, for outside methods to set fired anim state
    public void setFired(bool toSet)
    {
        animator.SetBool("isFired", toSet);
    }
}
