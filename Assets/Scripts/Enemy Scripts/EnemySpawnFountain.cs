using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to handle a spawning set of enemies
//VERY inefficient script, made for TESTING purposes
public class EnemySpawnFountain : MonoBehaviour {

    private List<GameObject> enemyList;
    private int maxSpawnLimit;
    private GameObject obj;

    // Use this for initialization
    void Start ()
    {
        enemyList = new List<GameObject>();
        maxSpawnLimit = 10;

        //load object from resources folder and instantiate it
        obj = (GameObject)Resources.Load("Prefabs/Enemies/enemyFem2RagDoll");
        GameObject clone;

        for (int i = 0; i < maxSpawnLimit; i++)
        {
            clone = (GameObject)Instantiate(obj, this.transform);
            enemyList.Add(clone);

        }

        this.transform.DetachChildren();
    
    }
	
	// Update is called once per frame
	void Update ()
    {
        //check if any objects were destroyed, if so replace them
        foreach (GameObject go in enemyList)
        {
            if (go == null)
            {
                enemyList.Remove(go);
            }
        }

        GameObject clone;
        while (enemyList.Count < maxSpawnLimit)
        {
            clone = (GameObject)Instantiate(obj, this.transform);
            enemyList.Add(clone);
        }

	}
}
