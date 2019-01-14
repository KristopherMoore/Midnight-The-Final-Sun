using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponHitboxCollider : MonoBehaviour {

    private Collider hitbox;

	// Use this for initialization
	void Awake ()
    {
        hitbox = transform.GetComponent<BoxCollider>();
        hitbox.isTrigger = true;
	}

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.transform.name);
        if(collision.transform.root.GetComponent<Unit>() != null)
        {
            collision.transform.root.GetComponent<Unit>().modifyHP(-5f);
        }
    }


}
