//Program Information///////////////////////////////////////////////////////////
/*
 * @file MainMenuClickable.cs
 *
 *
 * @game-version 0.71
 *          Kristopher Moore (13 May 2019)
 *          Initial Build of MainMenuClickable class
 *          
 *          Responsible for the scripting actions of Buttons of the main menu,
 *          changing them to transparent as they are hovered, and allowing them
 *          to call to the MainMenu for game actions.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuClickable : MonoBehaviour {

    string iconName;
    Image image;
    EventTrigger eventTrigger;

    void Awake()
    {
        //grab the icon's name, will be used later for sending to equip scripts
        iconName = this.transform.name;

        //get image of the Clickable running this script, and its event trigger
        image = this.transform.GetComponent<Image>();
        eventTrigger = this.transform.GetComponent<EventTrigger>();

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
        //remove the clickable from the iconName, and pass along to the onClickEquipItem function
        MainMenu.Instance.MenuClickable(this.iconName, "DEFAULT");
    }

    public void onPointerEnterDelegate(PointerEventData data)
    {
        //give black hue to item being hovered
        image.color = new Color(0, 0, 1, 0.95f);
    }

    public void onPointerExitDelegate(PointerEventData data)
    {
        //remove hue from the item, as it is no longer being hovered
        image.color = Color.white;
    }
}
