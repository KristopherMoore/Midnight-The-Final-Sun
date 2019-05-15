//Program Information///////////////////////////////////////////////////////////
/*
 * @file MainMenuHoverable.cs
 *
 *
 * @game-version 0.72
 *          Kristopher Moore (14 May 2019)
 *          Modified to allow click and data send to MainMenuSelection
 *          
 *          Responsible for the scripting actions of the character hover,
 *          the player will hover these colliders on the UI, and this will
 *          call to move the camera towards the relevant character based on
 *          the gameObject name
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuHoverable : MonoBehaviour {

    string goName;
    Image image;
    EventTrigger eventTrigger;
    GameObject inputField;

    void Awake()
    {
        //grab the icon's name, will be used later for sending to equip scripts
        goName = this.transform.name;

        //get image of the Clickable running this script, and its event trigger
        image = this.transform.GetComponent<Image>();
        eventTrigger = this.transform.GetComponent<EventTrigger>();

        //get our InputField
        inputField = GameObject.Find("InputField");

        //create our PointerEnter trigger, establish what it should call, and add it to the triggers
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { onPointerEnterDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);

        //create our PointerExit trigger
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { onPointerExitDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);

        //create our PointerClick trigger
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { onPointerClickDelegate((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);

    }

    public void onPointerClickDelegate(PointerEventData data)
    {
        MainMenuSelection.nameSelection = inputField.transform.Find("Text").GetComponent<Text>().text;
        MainMenuSelection.modeSelection = "NEW GAME";

        //start game by loading the scene.
        StartCoroutine(LoadAsyncScene());
    }

    public void onPointerEnterDelegate(PointerEventData data)
    {
        MainMenuCameraMovement.Instance.ChangeCameraPosition(this.goName);

        //give black hue to item being hovered
        image.color = new Color(0, 0, 1, 0.95f);
    }

    public void onPointerExitDelegate(PointerEventData data)
    {
        MainMenuCameraMovement.Instance.ChangeCameraPosition("DEFAULT");

        //remove hue from the item, as it is no longer being hovered
        image.color = Color.white;
    }

    //allow us to load the scene
    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TestingLevelNew");
        while (asyncLoad.isDone == false)
        {
            yield return null;
        }
    }
}
