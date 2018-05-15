using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    private float nextActionTime = 0.0f;
    public float period = 1f;
    bool canDealDamage = false;
    Collider2D enemyCollider = null;
    Animator animator;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            animator = gameObject.GetComponentInParent<Animator>();
            canDealDamage = true;
            enemyCollider = collider;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            animator.SetTrigger("notAttacking");
            canDealDamage = false;
            enemyCollider = null;
        }
    }

    void Update()
    {
        if (canDealDamage && enemyCollider != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // deal damage every 1 second
                if (Time.time > nextActionTime)
                {
                    nextActionTime = Time.time + period;

                    Debug.Log("Player Attack");
                    GameObject otherObj = enemyCollider.gameObject;
                    if (otherObj.transform.position.x - gameObject.transform.parent.position.x != 0)
                    {
                        if (otherObj.transform.position.x - gameObject.transform.parent.position.x < 0)
                            animator.SetTrigger("attackingLeft");
                        else if (otherObj.transform.position.x - gameObject.transform.parent.position.x > 0)
                            animator.SetTrigger("attackingRight");
                    }
                    else if (otherObj.transform.position.y - gameObject.transform.parent.position.y != 0)
                    {
                        if (otherObj.transform.position.y - gameObject.transform.parent.position.y < 0)
                            animator.SetTrigger("attackingDown");
                        else if (otherObj.transform.position.y - gameObject.transform.parent.position.y > 0)
                            animator.SetTrigger("attackingUp");
                    }
                    else
                        animator.SetTrigger("notAttacking");

                    enemyCollider.gameObject.GetComponent<Enemy>().health -= gameObject.GetComponentInParent<Player>().DPS;
                }
            }
        }
        if (animator != null)
        {
            if (!canDealDamage && !animator.GetBool("notAttacking"))
                animator.SetTrigger("notAttacking");
        }
    }
}
