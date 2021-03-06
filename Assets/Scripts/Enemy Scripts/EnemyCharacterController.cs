﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//class responsible for handling the control of AI enemies, handles attacks, vision states, movement across a Navigation Mesh, etc.
public class EnemyCharacterController : MonoBehaviour {

    public CharacterController CharacterController;
    public EnemyCharacterController Instance; //hold reference to current instance of itself
    private Animator animator;
    private NavMeshAgent NavAgent;

    //get the Hearing Collider, vision will be handled by Angle calculations
    private Collider hearingCollider;
    private bool visionStay;
    private float visionStayCount;
    private bool hearingStay;
    private float hearingStayCount;

    //aggro state, the most important trigger to whether our enemy has a target, and who they are persuing.
    private bool isAggroed = false;
    private Transform aggroTargetTransform;

    //character states, mostly for animation and ensuring we stay in certain states for a period of time. Like Reloading and Firing.
    private bool isReloading = false;
    private bool isFiring = false;
    private bool isAnimationLocked = false;
    private bool isMelee = true;

    //run on game load (before start)
    void Awake()
    {
        //grab relevant objects to our controller.
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;
        animator = transform.GetComponent<Animator>();
        NavAgent = GetComponent<NavMeshAgent>();

        //grab hearing collider instance
        hearingCollider = transform.GetComponent<SphereCollider>();
    }

    //Update called on every frame
    void Update()
    {
        //hearing collider is handled when the triggers activate.
        //vision is done via angle, so we need to check that on Update
        checkVisionCone();

        //handleMotion
        navMeshMovement();

        //execute actions and send off any animations.
        if(isMelee)
            checkMeleeRangeOfTarget();
        else
        {
            checkRangeOfTarget();
        }

        //since we will not be doing animations seperately for the arms and body, (like the player controller does), we need to lock the enemy in place for reloading,
        //otherwise the animations would look strange.
        if (isReloading)
            return;

    }

    //method to check the vision radius of the enemy and if a player is within it
    private void checkVisionCone()
    {
        //find the angle between the target position and our enemies position
        Vector3 targetDirection = GameObject.Find("PlayerCharacter").transform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);

        //leaving these Rays for debugging visualization going forward. Mostly if I want to modify and test the bounds of the Vision Cones.
        Debug.DrawRay(transform.position, transform.forward * 15, Color.yellow);
        Debug.DrawRay(transform.position, targetDirection * 15, Color.green);
        Debug.DrawRay(transform.position + transform.up, targetDirection * 15, Color.white);

        //if that angle is less than 60deg, this will be applied to both sides so we have a 120deg triangle of vision
        if (angle < 60.0f)
        {
            //now we need to verify that the target is in fact visible, IE not blocked by a structure. We can do that with a raycast towards the player and check if it hits
            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up, targetDirection, out hit, Mathf.Infinity))
            {
                if (hit.transform.root.tag == "Player")
                    isAggroed = true;
            }
        }
    }

    //if a Weapon is Triggered within the hearingCollider, check the audio source, see if it is playing and if so aggro to the player
    private void OnTriggerStay(Collider hearingCollider)
    {
        if(hearingCollider.transform.tag == "Weapon")
        {
            AudioSource weaponSound;
            weaponSound = hearingCollider.transform.GetComponent<AudioSource>();

            //check if the shot happened recently, since the clip plays over two seconds and can be repeated. We only care if the gunshot just happedened and its in our hearing collider.
            if(weaponSound.time < .35f && weaponSound.time != 0)
            {
                isAggroed = true;
            }
        }
        /*if(visionStay)
        {
            visionStayCount = visionStayCount + Time.deltaTime;
        }
        Debug.Log(visionCollider.name);
        */
    }

    //if the trigger state is exited
    private void OnTriggerExit(Collider collider)
    {
        //if we escape the aggro ranges. we stop being chased. Would work fine, need to determine seperating the trigger states.
        //isAggroed = false;
    }

    //check if the enemy has entered within melee range of its target, for now it only handles the Player Character, should be extended for other targets
    private void checkMeleeRangeOfTarget()
    {
        //TODO: extend to other targets not just PC.
        Vector3 targetDirection = GameObject.Find("PlayerCharacter").transform.position - transform.position;

        Debug.DrawRay(transform.position + transform.up, targetDirection, Color.red);

        //cast a ray to see if the target is close enough to start a melee attack
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, targetDirection, out hit, 2f))
        {
            //if that hit was on the player
            if (hit.transform.root.tag == "Player")
            {
                animator.SetBool("isMelee", true);
            }
        }
        else
        {
            animator.SetBool("isMelee", false);
        }
    }

    //TODO: implement checking the Range of our target, to see what the best course of AI action is
    private void checkRangeOfTarget()
    {

    }

    //utilize navMeshAgent to move our Enemy
    private void navMeshMovement()
    {
        //if we are aggroed, then make progress towards target
        if(isAggroed)
        {
            //testing of our navmesh agent. by having it attempt to go to the camera. with a small Z offset to be in front not in camera.
            NavAgent.destination = Camera.main.transform.position + (Camera.main.transform.forward * 2);

            //now make our agent face toward the target
            Quaternion storeOldRotation = transform.rotation;
            transform.LookAt(Camera.main.transform.position);
            transform.SetPositionAndRotation(transform.position, new Quaternion(storeOldRotation.x, transform.rotation.y, storeOldRotation.z, transform.rotation.w));
        }
        else //otherwise, continue with idle movement
        {
            
        }
        
    }

    //change the aggro state of our AI unit to be aggroed to the given target 
    private void setAggro(bool aggroToSet, Transform targetTransformToSet)
    {
        isAggroed = aggroToSet;
        aggroTargetTransform = targetTransformToSet;
    }

    //enumerator to delay the AI for reload anims, set it in a locked anim state
    IEnumerator WaitXSecondsForReload(float waitTime)
    {
        //wait the time out
        yield return new WaitForSeconds(waitTime);
        isAnimationLocked = false;
        isReloading = false;

        //end anims
        AnimateArms.Instance.setReloading(false);
        AnimateWeapon.Instance.setReloading(false);
    }

    //enumerator to handle delay in weapon based on the fireRate, fast guns shoot fast, slow guns shoot slow.
    IEnumerator FireWeapon(float waitTime)
    {
        //PRE-WAIT ACTIONS
        Instance.isFiring = true;
        //play our fire sound
        SoundManager.Instance.PlaySound("Fire");
        //send off animation
        AnimateArms.Instance.setFired(true);
        AnimateWeapon.Instance.setFired(true);

        //wait the time out, in frames
        yield return new WaitForSeconds(waitTime);

        //POST-WAIT ACTIONS
        isAnimationLocked = false;
        isFiring = false;

        //end anims
        AnimateArms.Instance.setFired(false);
        AnimateWeapon.Instance.setFired(false);
    }

    IEnumerator WaitForAnimation(string animationName)
    {

        yield return null;
    }
}
