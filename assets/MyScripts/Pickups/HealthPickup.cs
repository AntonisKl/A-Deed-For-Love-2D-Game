using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

    public float health;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "AI")
        {
            // health pickup is taken
            Player playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            Companion companionScript = GameObject.FindGameObjectWithTag("AI").GetComponent<Companion>();

            if (playerScript.health + health >= playerScript.maxHealth)
                playerScript.health = playerScript.maxHealth;
            else
                playerScript.health += health;

            if (companionScript.health + (health / 2) >= companionScript.maxHealth)
                companionScript.health = companionScript.maxHealth;
            else
                companionScript.health += (health / 2);

            Destroy(gameObject);
        }
    }
}
