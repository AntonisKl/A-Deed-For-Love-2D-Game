using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour {

    Animator animator;
    public float health, DPS, maxHealth;
    public bool canDealDamage, inputEnabled;

    // Use this for initialization
    void Start () {
        health = GameManager.playerHealth;
        maxHealth = 100;
        animator = GetComponent<Animator>();
        canDealDamage = false;
        inputEnabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
        {
            GameObject.FindGameObjectWithTag("MainCamera").transform.parent = null;
            Destroy(gameObject);
        }

        GameManager.playerHealth = health;

        if (inputEnabled)
        {
            float horizontal, vertical;
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(horizontal, vertical, 0);

            // code for animation of player
            if (horizontal != 0)
            {
                if ((horizontal * 2 * Time.deltaTime) < 0)
                    animator.SetTrigger("walkingLeft");
                else if ((horizontal * 2 * Time.deltaTime) > 0)
                    animator.SetTrigger("walkingRight");
            }
            else if (vertical != 0)
            {
                if ((vertical * 2 * Time.deltaTime) < 0)
                    animator.SetTrigger("walkingDown");
                else if ((vertical * 2 * Time.deltaTime) > 0)
                    animator.SetTrigger("walkingUp");
            }
            else
                animator.SetTrigger("idle");

            // move player
            transform.position += move * 2 * Time.deltaTime;
        }
    }


}
