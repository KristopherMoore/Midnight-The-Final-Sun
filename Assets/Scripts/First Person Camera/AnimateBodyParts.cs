using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        animator.SetFloat("MotionXAxis", 0);
        animator.SetFloat("MotionYAxis", 0);
        animator.SetBool("isSneaking", false);
    }

    //Outside methods can set our motion Axes, IE the player character controller. Limit the times we get Axis Input in each script.
    public void setMotionAxes(float XSet, float YSet)
    {
        animator.SetFloat("MotionXAxis", XSet);
        animator.SetFloat("MotionYAxis", YSet);
    }

    //see above, for outside methods to set sneaking anim state
    public void setSneakState(bool toSet)
    {
        animator.SetBool("isSneaking", toSet);
    }
}
