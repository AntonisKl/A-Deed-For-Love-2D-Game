using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Door : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider)
    {
        // if the player goes through the door, change the scene/level
        if (collider.gameObject.tag == "Player")
            GameManager.changeLevel();
    }
}
