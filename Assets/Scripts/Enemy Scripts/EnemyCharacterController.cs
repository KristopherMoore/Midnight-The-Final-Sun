using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacterController : MonoBehaviour {

    //IMPORTANT NOTE: I have offloaded redundancy into this Controller, primarily because the Animation scripts were very redundant in state checking
    //since this controller already has to check these states we can drastically reduce overhead by simply adding in statement load. Furthermore,
    //this controller already has exit conditions for optimization, so that is expontential in our overall statement execution reduction.

    public CharacterController CharacterController;
    public EnemyCharacterController Instance; //hold reference to current instance of itself
    private Animator animator;
    private NavMeshAgent NavAgent;

    //get the Hearing Collider and Vision Collider
    private Collider hearingCollider;
    private Collider visionCollider;
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

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;
        animator = transform.GetComponent<Animator>();
        NavAgent = GetComponent<NavMeshAgent>();

        //grab collider instances
        hearingCollider = transform.GetComponent<SphereCollider>();
        visionCollider = transform.GetComponent<BoxCollider>();
    }


    void Update()
    {
        
        //vision and hearing are handled when the triggers activate.

        //Decide what to do now,

        //handleMotion
        navMeshMovement();

        //execute actions and send off any animations.

        //since we will not be doing animations seperately for the arms and body, (like the player controller does), we need to lock the enemy in place for reloading,
        //otherwise the animations would look strange.
        if (isReloading)
            return;

    }

    private void CheckVisionAndHearing()
    {
        
    }

    private void OnTriggerEnter(Collider visionCollider)
    {
        if(visionCollider.transform.root.tag == "Player")
        {
            Debug.Log(visionCollider);
            isAggroed = true;
        }
    }

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
        //if(visionStay)
        //{
           // visionStayCount = visionStayCount + Time.deltaTime;
        //}
        //Debug.Log(visionCollider.name);
    }

    private void OnTriggerExit(Collider visionCollider)
    {
        //if we escape the aggro ranges. we stop being chased. Would work fine, need to determine seperating the trigger states.
        //isAggroed = false;
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

    private void setAggro(bool aggroToSet, Transform targetTransformToSet)
    {
        isAggroed = aggroToSet;
        aggroTargetTransform = targetTransformToSet;
    }


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
