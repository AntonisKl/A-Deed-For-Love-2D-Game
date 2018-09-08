using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "AI")
        {
            // key is taken
            GameManager.instance.addKey();
            Destroy(gameObject);
        }
    }
}
