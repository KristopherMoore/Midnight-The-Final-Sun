using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance;


    //variables for the players Weapon
    private GameObject playerWeapon;
    private AudioSource weaponAudioSource;

	// Use this for initialization
	void Start ()
    {
        Instance = this;

        findEquippedWeapon();

        weaponAudioSource = playerWeapon.GetComponent<AudioSource>();
	}

    public void PlaySound(string soundName)
    {
        if(soundName == "Fire")
        {
            weaponAudioSource.Play();
        }
    }

    private void findEquippedWeapon()
    {
        //find our Main Camera gameObject
        playerWeapon = GameObject.Find("Main Camera");
        //grab our weapon bone
        playerWeapon = HelperK.FindSearchAllChildren(playerWeapon.transform, "WEAPON").gameObject;

        //find the name of the object current the child of our WEAPON bone, this way we dont need to know the name of the weapon currently equipped. We just know where it will be
        playerWeapon = playerWeapon.transform.GetChild(0).gameObject;
    }

}
