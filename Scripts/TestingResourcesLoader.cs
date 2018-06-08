using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingResourcesLoader : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        //create gameObject for testing, NOTE: In the armor changer we will just pull use the script on the base GameObject transform.
        GameObject go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder);
        go.name = "Test Instance GameObj";

        //load object from resources folder and instantiate it Instantiate(our loaded obj, parent's transform)
        Object obj = Resources.Load("Prefabs/Elite Knight Body");
        Instantiate(obj, go.transform);

        //Also instantiate it to our main transform. Testing for when the Player Character Gobj will use this type of script to load/delete
        this.transform.SetPositionAndRotation(new Vector3(1,1,1), Quaternion.Euler(0,0,0)); //testing where obj gets instanced, should be trans pos
        Instantiate(obj, this.transform);
        
    }
	
}
