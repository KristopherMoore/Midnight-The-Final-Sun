using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWeapon : MonoBehaviour {

    private Animator animator;
    private PlayerCharacterController pController;

    //public Instance so our Controllers can access this script.
    public static AnimateWeapon Instance;

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
        animator.SetBool("isFired", false);
        animator.SetBool("isReloading", false);
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
