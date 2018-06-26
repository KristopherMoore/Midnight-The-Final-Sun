using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{

    public static CharacterController CharacterController;
    public static PlayerCharacterController Instance; //hold reference to current instance of itself

    public enum animationState
    {
        Idling, Walking, Jogging, Running, Rolling, Stagger, Stunned, L1Attack, L1AttackC2
    }
    public animationState playerAnimationState { get; set; }

    private int currentAttackChain = 0;
    private int maxAttackChain = 3;

    public enum direction
    {
        Stationary, Forward, Left, Backward, Right, LeftFwd, RightFwd, LeftBck, RightBck
    }
    public direction MoveDirection { get; set; }

    public float deadZone = 0.01f;

    public bool isMoving;
    public bool isAnimationLocked;

    void Awake() //run on game load (before start)
    {
        CharacterController = GetComponent("CharacterController") as CharacterController;
        Instance = this;  
    }



    //
    void Update()
    {
        if (Camera.main == null) //check for camera, if no main camera, stop.
            return;

        //handle actions before locomotion, since actions will at times lock the player into place
        HandleActionInput();

        //if we arent locked in an animation, continue with locomotion.
        if(!this.isAnimationLocked)
        {
            GetLocomotionInput();
        }

        //update the motor even if we didnt have locomotion, as the motor also calculates things like gravity.
        PlayerCharacterMotor.Instance.UpdateMotor();

    }

    void GetLocomotionInput()
    {
        Instance.isMoving = false;
        Instance.isAnimationLocked = false;

        PlayerCharacterMotor.Instance.verticalVelocity = PlayerCharacterMotor.Instance.MoveVector.y;

        PlayerCharacterMotor.Instance.MoveVector = Vector3.zero; //reclacuate, prevents it from being additive. Basically restart on every update

        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        //if we are animation locked, we will ignore axis data
        if(!Instance.isAnimationLocked)
        {
            //check if input exceeds the deadZone (check for a minimal amount of input before moving)
            if (yAxis > deadZone || yAxis < -deadZone)
            {
                Instance.isMoving = true;
                PlayerCharacterMotor.Instance.MoveVector += new Vector3(0, 0, yAxis);
            }
            if (xAxis > deadZone || xAxis < -deadZone)
            {
                Instance.isMoving = true;
                PlayerCharacterMotor.Instance.MoveVector += new Vector3(xAxis, 0, 0);
            }
        }

    }

    void HandleActionInput()
    {
        //ESCAPE sequence, allows us to modify animationLocks manually for avoid errors
        if (Input.GetKey(KeyCode.R))
            isAnimationLocked = false;
        if (Input.GetKey(KeyCode.E))
            isAnimationLocked = true;

        //if we are anim locked, leave this function. no Actions can happen until we are no longer locked
        if (isAnimationLocked == true)
            return;

        playerAnimationState = animationState.Idling; //default state, only modified if some other action happens

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            playerAnimationState = animationState.Jogging;
            if (Input.GetKey(KeyCode.LeftShift))
                playerAnimationState = animationState.Running;
            else if (Input.GetKey(KeyCode.Z))
                playerAnimationState = animationState.Walking;
        }
        else
            playerAnimationState = animationState.Idling;

        //rolling action
        if (Input.GetKey(KeyCode.Space))
        {
            isAnimationLocked = true;
            playerAnimationState = animationState.Rolling;

            //TODO:
            //Corroutine here to wait for rolling to finish, then when done set isAnimationLocked = false; Encapsulate Handle Action
            //Inputs inside of another check for isAnimationLocked, that way any prior engaged animations are messed with until co
            //routine finishes.

            StartCoroutine(WaitForAnimation("DS roll fwd"));

            //wait for animation, and send roll movement command to Motor
            StartCoroutine(WaitXSeconds(1f));


        }

        //L1 attack action, with left click
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch(currentAttackChain)
            {
                case 0:
                    isAnimationLocked = true;
                    playerAnimationState = animationState.L1Attack;
                    currentAttackChain += 1;
                    StartCoroutine(WaitXSeconds(1f));
                    break;
                case 1:
                    isAnimationLocked = true;
                    playerAnimationState = animationState.L1AttackC2;
                    currentAttackChain += 1;
                    StartCoroutine(WaitXSeconds(1f));
                    break;
                case 2:
                    isAnimationLocked = true;
                    playerAnimationState = animationState.L1Attack;
                    currentAttackChain += 1;
                    StartCoroutine(WaitXSeconds(1f));
                    break;
                default:
                    break;
            }
        }

        //Equip item action
        if(Input.GetKey(KeyCode.Y))
        {
            attachWeaponToRightHand("Bastard Sword");
        }
    }

    IEnumerator WaitXSeconds(float waitTime)
    {
        print(Time.time);
        yield return new WaitForSeconds(waitTime);
        print(Time.time);
        isAnimationLocked = false;
        currentAttackChain = 0;
    }

    IEnumerator WaitForAnimation(string animationName)
    {
        
        yield return null;
    }

    //method that will attachWeapons to the player bone.
    private void attachWeaponToRightHand(string weaponName)
    {
        //Find the weapon object on the Player Character
        //Transform weapon = this.transform.Find(weaponName);
        Transform weapon = HelperK.FindSearchAllChildren(this.transform, weaponName);

        //Find the rWeaponBone of the player character. Searching through all children.
        Transform rWeaponBone = HelperK.FindSearchAllChildren(this.transform, "R_Weapon");

        try
        {
            //make the weapon a child of the rWeaponBone, that way it will follow it in all its animations. And place its transform at the handbone location
            weapon.transform.parent = rWeaponBone;
            weapon.transform.SetPositionAndRotation(rWeaponBone.position, rWeaponBone.rotation);

            //compensating for our model rips base rotation being 180degrees off,
            weapon.transform.Rotate(weapon.transform.rotation.x, weapon.transform.rotation.y, weapon.transform.rotation.z + 180); 
        }
        catch(MissingComponentException ex)
        {
            Debug.Log("Throwing Null Exception");
        }
    }

}