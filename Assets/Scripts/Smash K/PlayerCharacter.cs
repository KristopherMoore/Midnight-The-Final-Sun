using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    //This class will serve as a base for the Player objects. It will hold important references to their stats and stocks.

    public string playerName;

    private int maxStocks;
    private int stocks;
    private int percent;



	// Use this for initialization
	void Start ()
    {
        playerName = "DefaultInstance";
        percent = 50;
	}

    public void setName(string toSet)
    {
        playerName = toSet;
    }

    public int getCurrentStocks()
    {
        return stocks;
    }

    public void addStocks(int stocksToAdd)
    {
        stocks += stocksToAdd;

        if (stocks > maxStocks)
            stocks = maxStocks;
    }

    //to be called by GameHandler upon a stockLoss (going off screen)
    public void subtractStock()
    {
        stocks--;
        if (stocks < 0)
            stocks = 0;

        //Also reset our percentage to zero, since we lost a stock
    }

    //to be called by Game Handler to check if the player is alive or dead.
    public bool isAlive()
    {
        if (stocks > 0)
            return true;
        else
            return false;
    }
	
    //to be called by game creation, and set the players stock max.
    public void setMaxStocks(int toSet)
    {
        maxStocks = toSet;
        addStocks(maxStocks);
    }

    public int getPercentage()
    {
        return percent;
    }

    public void addPercentage(int damageAmount) //take Damage
    {
        percent += damageAmount;

        //bound to 999% maximum
        if (percent > 999)
            percent = 999;
    }

    public void subtractPercentage(int healAmount) //heal Damage
    {
        percent -= healAmount;

        //bound to 0% maximum
        if (percent < 0)
            percent = 0;
    }

}
