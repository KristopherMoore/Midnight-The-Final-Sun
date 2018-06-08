using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_PlayerHeadFollowAnchor : MonoBehaviour {

    //very simple script to ensure the player "rotates" by looking at the focus point, only on the X and Y axis, can be further developed to include z for his head only, so the head tracks cursor like in GoW

    public GameObject playerFocus;

    // Update is called once per frame
    void LateUpdate()
    {

        Vector3 positionOfFocus = playerFocus.transform.position;
        positionOfFocus = new Vector3(playerFocus.transform.position.x , playerFocus.transform.position.y , playerFocus.transform.position.z); //zero out y to ensure we dont mess with vertical axis (IMPORTANT as movement is based on world space, we dont want him to fly

        transform.LookAt(positionOfFocus); //have our player look at the new vector

        Debug.DrawLine(transform.position, playerFocus.transform.position, Color.green);
    }
}