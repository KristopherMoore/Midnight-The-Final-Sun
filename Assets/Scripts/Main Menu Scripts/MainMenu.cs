//Program Information///////////////////////////////////////////////////////////
/*
 * @file MainMenu.cs
 *
 *
 * @game-version 0.71 
 *          Kristopher Moore (13 May 2019)
 *          Initial Build of MainMenu class
 *          
 *          Responsible as a base for the MenuClickables Scripts, allows those objects to call into functions
 *          from their own instance.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public static MainMenu Instance;
    private GameObject MainButtonsPanel;
    private GameObject ClassButtonsPanel;

    //Initialization of Main Menu states
    void Awake()
    {
        //create instance, setup default states
        Instance = this;

        MainButtonsPanel = HelperK.FindSearchAllChildren(this.transform, "MainButtonsPanel").gameObject;
        ClassButtonsPanel = HelperK.FindSearchAllChildren(this.transform, "ClassButtonsPanel").gameObject;
    }

    //Menu Clickables will send this click actions with their processName to be parsed here
    public void MenuClickable(string selection, string startingClass)
    {
        //determine which action to take based on selection Code.
        switch (selection)
        {
            case "PLAY":
                StartCoroutine(LoadAsyncScene());
                break;
            case "NEW GAME":
                ClassButtonsPanel.SetActive(true);
                MainButtonsPanel.SetActive(false);
                break;
            case "LOAD GAME":
                //TODO: implement loading features and saving process of game
                break;
            case "BACK":
                ClassButtonsPanel.SetActive(false);
                MainButtonsPanel.SetActive(true);
                break;
            case "EXIT":
                ExitGame();
                break;
        }
    }


    //method to end the game upon call
    private void ExitGame()
    {
        //quits the game. This command is ignored in the unity editor
        Application.Quit();
    }


    //new game starting, with startingClass string, to be used to select proper character
    public void NewGame(string startingClass)
    {
        Debug.Log("Starting Load Co-routine");
        StartCoroutine(LoadAsyncScene());
    }

    //Enumerator to load our scene after a given amount of time
    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TestingLevelNew");
        while (asyncLoad.isDone == false)
        {
            yield return null;
        }
    }

    //Simple debugging function, to test buttons calls at runtime
    public void DebuggingTest(string toPrint)
    {
        Debug.Log("Debugging Test in Game Menu: " + toPrint);
    }

}
