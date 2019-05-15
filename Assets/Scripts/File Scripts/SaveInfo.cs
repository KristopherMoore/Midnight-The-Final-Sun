//Program Information///////////////////////////////////////////////////////////
/*
 * @file SaveInfo.cs
 *
 *
 * @game-version 0.72 
 *          Kristopher Moore (14 May 2019)
 *          Initial Build of SaveInfo
 *          
 *          Responsible for holding copies of the player object
 *          and their spawn location
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInfo
{
    public Player playerObject { get; set; }
    public Vector3 spawnLocation { get; set; }

}
