using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu: MonoBehaviour {

    public static GameMenu Instance;
    private GameObject[] panelGameObjects;

    private bool status;
    private int currentPanel;

	void Awake()
    {
        Instance = this;
        status = false;
        currentPanel = 0;
        this.gameObject.SetActive(false);

        panelGameObjects = new GameObject[4];
        panelGameObjects[0] = this.transform.Find("StatisticsPanel").transform.gameObject;
        panelGameObjects[1] = this.transform.Find("InventoryPanel").transform.gameObject;
        panelGameObjects[2] = this.transform.Find("SettingsPanel").transform.gameObject;
        panelGameObjects[3] = this.transform.Find("QuitPanel").transform.gameObject;

        loadMenuPanel();
    }

    //when called by the GameHandler, this will turn the menu ui on and off
    public void changeMenuStatus()
    {
        //invert our current status, and set the gameObjects active state to match
        status = !status;
        this.gameObject.SetActive(status);
    }

    //method to trigger the menu to load the UI elements, will utilize the currentPanel int to determine which ui elements to turn on and off
    public void loadMenuPanel()
    {
        //reset all panels, then set our current to true
        panelGameObjects[0].SetActive(false);
        panelGameObjects[1].SetActive(false);
        panelGameObjects[2].SetActive(false);
        panelGameObjects[3].SetActive(false);

        panelGameObjects[currentPanel].SetActive(true);
    }

    //method to shift the panel focused on to the rightmost, if we are already at far right, then move across.
    public void moveMenuRight()
    {

    }

    //method to shift the panel focused on to the leftmost, if we are already at far left, then move across.
    public void moveMenuLeft()
    {

    }

    public void onClickStats()
    {
        currentPanel = 0;
        loadMenuPanel();
    }

    public void onClickInventory()
    {
        currentPanel = 1;
        loadMenuPanel();
    }

    public void onClickSettings()
    {
        currentPanel = 2;
        loadMenuPanel();
    }

    //method to end the game upon call
    public void QuitGame()
    {
        //quits the game. This command is ignored in the unity editor
        Application.Quit();
    }
}
