using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharAndBodyRotator : MonoBehaviour {

    public GameObject mainCam;

    //initialization
    private void Start()
    {
        mainCam = GameObject.FindWithTag("MainCamera");
    }

    // update after each frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, mainCam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
