using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu: MonoBehaviour {

    public static GameMenu Instance;
    private GameObject[] panelGameObjects;
    private Inventory inventory;



    //InventoryPanel Children
    private GameObject armorGrid;
    private GameObject weaponsGrid;
    private GameObject itemsGrid;
    private GameObject previewPanel;
    private GameObject itemDescriptionPanel;
    private GameObject inventoryCategoriesPanel;
    

    private bool status;
    private int currentPanel;

    //Initialization of Game Menu states, lots of loading to do here, as we need access to alot of the different panels for functions
    //Note: This will be the only Menu base script, so all children that have buttons that call should access the gameMenu go that has this script attached
	void Awake()
    {
        //create instance, setup default states
        Instance = this;
        status = false;
        currentPanel = 0;
        this.gameObject.SetActive(false);

        //find our menu sub-panels and establish references to them
        panelGameObjects = new GameObject[4];
        panelGameObjects[0] = this.transform.Find("StatisticsPanel").transform.gameObject;
        panelGameObjects[1] = this.transform.Find("InventoryPanel").transform.gameObject;
        panelGameObjects[2] = this.transform.Find("SettingsPanel").transform.gameObject;
        panelGameObjects[3] = this.transform.Find("QuitPanel").transform.gameObject;

        //find InventoryPanel sub-panels
        armorGrid = panelGameObjects[1].transform.Find("ArmorGrid").transform.gameObject;
        weaponsGrid = panelGameObjects[1].transform.Find("WeaponsGrid").transform.gameObject;
        itemsGrid = panelGameObjects[1].transform.Find("ItemsGrid").transform.gameObject;
        previewPanel = panelGameObjects[1].transform.Find("PreviewPanel").transform.gameObject;
        itemDescriptionPanel = panelGameObjects[1].transform.Find("ItemDescriptionPanel").transform.gameObject;
        inventoryCategoriesPanel = panelGameObjects[1].transform.Find("InventoryCategoriesPanel").transform.gameObject;

        loadMenuPanel();

        loadPlayerInventory();
    }

    //when called by the GameHandler, this will turn the menu ui on and off
    public void changeMenuStatus()
    {
        //invert our current status, and set the gameObjects active state to match
        status = !status;
        this.gameObject.SetActive(status);

    }

    //method to load an updated copy of the Player Object's inventory
    public void loadPlayerInventory()
    {
        string iconsToAdd;
        Transform childToDestroy;

        inventory = PlayerObject.Instance.getInventoryObject();

        for (int i = 0; i < armorGrid.transform.childCount; i++)
            Destroy(armorGrid.transform.GetChild(i).gameObject);
    
        //TODO: Have a loading check here that populates all items in the inventory, so we know what to load icons for
        for (int i = 0; i < inventory.getLength(); i++)
        {
            //Debug.Log("for loop in loadPlayerInventory: " + inventory.findItemAtIndex(i).getName());

            //grab the name of the item at our current index of the inventory, then load it from our resources
            iconsToAdd =  inventory.findItemAtIndex(i).getName();
            Object toAdd = (Object)Resources.Load("Menu/Weapons/" + iconsToAdd + " Clickable");

            //if the object is null, move on. Otherwsie instantiate it
            if (toAdd == null)
                continue;
            Instantiate(toAdd, armorGrid.transform);
        }

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
        if (currentPanel >= 2)
            currentPanel = 0;
        else
            currentPanel++;

        loadMenuPanel();
    }

    //method to shift the panel focused on to the leftmost, if we are already at far left, then move across.
    public void moveMenuLeft()
    {
        if (currentPanel <= 0)
            currentPanel = 2;
        else
            currentPanel--;

        loadMenuPanel();
    }

    public bool getMenuStatus()
    {
        return status;
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

    ////////////////InventoryPanel Sub-functions:////////////////////////////////
    public void onClickEquipItem(string itemName)
    {
        PlayerObject.Instance.equipPlayer(itemName);
    }

    //Simple debugging function, to test buttons calls at runtime
    public void DebuggingTest(string toPrint)
    {
        Debug.Log("Debugging Test in Game Menu: " + toPrint);
    }
}
