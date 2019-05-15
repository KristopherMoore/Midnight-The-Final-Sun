//Program Information///////////////////////////////////////////////////////////
/*
 * @file MainMenu.cs
 *
 *
 * @game-version 0.72 
 *          Kristopher Moore (14 May 2019)
 *          Initial Build of MainMenuSelection static class
 *          
 *          Responsible for the holding of selections from the main menu,
 *          to be passed through to the loaded scene. Whether we wanted to load
 *          a character, or create a new one, etc.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainMenuSelection
{

    public static string modeSelection { get; set; }
    public static string nameSelection { get; set; }

}
