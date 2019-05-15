using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to handle the Animator for the Player Character's FIRST Person Body Parts
public class AnimateBodyParts : MonoBehaviour
{
    private Animator animator;
    private PlayerCharacterController pController;

    //public Instance so our Controllers can access this script.
    public static AnimateBodyParts Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        animator = transform.GetComponent<Animator>();

        pController = PlayerCharacterController.Instance;

        //ensure these have a starting value
        resetAllAnimations();
    }

    //Outside methods can set our motion Axes, IE the player character controller. Limit the times we get Axis Input in each script.
    public void setMotionAxes(float XSet, float YSet)
    {
        if (animator == null)
        {
            animator = transform.GetComponent<Animator>();
            Debug.Log("ANIMATOR RESET");
        }

        animator.SetFloat("MotionXAxis", XSet);
        animator.SetFloat("MotionYAxis", YSet);
    }

    //see above, for outside methods to set sneaking anim state
    public void setJumping(bool toSet)
    {
        animator.SetBool("isJumping", toSet);
    }

    //see above, for outside methods to set sneaking anim state
    public void setFalling(bool toSet)
    {
        animator.SetBool("isFalling", toSet);
    }

    //see above, for outside methods to set sneaking anim state
    public void setSneakState(bool toSet)
    {
        animator.SetBool("isSneaking", toSet);
    }

    public void resetAllAnimations()
    {
        if (!animator)
            animator = transform.GetComponent<Animator>();

        //NOTE: do NOT reset our MotionAxes here, it is handled by a seperate call and never needs to be overriden.
        animator.SetBool("isSneaking", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
    }
}
