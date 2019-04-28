using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//extremely simple script for flashlight control. will be modified by the PlayerCharacter Controllers.
public class FlashlightController : MonoBehaviour {
	
    //set our flashlight based on a boolean
    public void setFlashlight(bool toSet)
    {
        this.gameObject.SetActive(toSet);
    }
}
