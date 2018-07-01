using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsMasterList {

    //These indexed lists will be used similarly to multiDimensionsal Arrays, but with these indexed lists
    //The idea being that after these are populated on load, we can be given an Item's Name. Find its index, then
    // have all other information available for our factory to create the apporiate item within the game.
    public List<string> itemListNames = new List<string>();
    public List<string> itemListTypes = new List<string>();
    public List<float> itemListWeights = new List<float>();
    public List<float> itemListPrices = new List<float>();

    public ItemsMasterList()
    {
        populateItemLists();
    }

    //TODO: modify this to pull from a file of items.
    public void populateItemLists()
    {
        //instantiation of lists
        itemListNames = new List<string>();
        itemListTypes = new List<string>();
        itemListWeights = new List<float>();
        itemListPrices = new List<float>();

        //now build up every item, for now it will be manual. TODO: Modify this to pull from a file containing the data, and have a coroutine loop fill
        itemListNames.Add("Bastard Sword");
        itemListTypes.Add("Weapon");
        itemListWeights.Add(5.0f);
        itemListPrices.Add(10000f);

        itemListNames.Add("Katana");
        itemListTypes.Add("Weapon");
        itemListWeights.Add(2.0f);
        itemListPrices.Add(8000f);

        itemListNames.Add("Dagger");
        itemListTypes.Add("Weapon");
        itemListWeights.Add(0.5f);
        itemListPrices.Add(2000f);
    }
}
