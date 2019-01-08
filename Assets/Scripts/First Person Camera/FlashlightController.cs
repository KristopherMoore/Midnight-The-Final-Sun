using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour {
	
    //extremely simple script for flashlight controll. will be modified by the PlayerCharacter Controllers.

    //so our controller can set the flashlight on and off
    public void setFlashlight(bool toSet)
    {
        this.gameObject.SetActive(toSet);
    }
}
