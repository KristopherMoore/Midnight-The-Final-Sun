using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarTestingCounter : MonoBehaviour
{
    public static GameObject textFieldObject;
    public static Text text;
    public static HealthBarTestingCounter Instance;
    private float healthCount = 100f;
    private static PlayerStats player = new PlayerStats();

    // Use this for initialization
    void Start()
    {
        //self instanciation
        Instance = this;

        //find our Counter object
        textFieldObject = GameObject.Find("HealthBar Testing Counter");

        //grab its text component
        text = textFieldObject.GetComponent<Text>();

        //call first instantiation, of the counter
        updateHealthCounter();
    }

    //base method
    public void updateHealthCounter()
    {
        healthCount = player.getHP();
        text.text = healthCount.ToString();
        text.text = text.text + "%";
    }

    //overload to allow for public subtracting and adding to the counter
    public void updateHealthCounter(float hpCount)
    {
        healthCount += hpCount;
        this.updateHealthCounter();
    }
}
