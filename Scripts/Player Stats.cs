using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : UnitStats {

    private UnitStats player;

    protected float Vigor;
    protected float Endurance;
    protected float Vitality;
    protected float Strength;
    protected float Dexterity;
    protected float Intellect;
    protected float Faith;

    // run on game load
    void Awake ()
    {
        player = new UnitStats();
        this.setStats(0,0,0,0,0,0,0,0,0,0);
	}

    //method to be called when loading a game state, takes in the players saved stats
    public void setStats(float HP, float stam, float mana, float Vig, float End, float Vit, float Str, float Dex, float Int, float Fai)
    {

    }

    public bool checkIfDead()
    {
        if (this.player.getHP() > 0)
            return false;
        else
            return true;
    }
	
}
