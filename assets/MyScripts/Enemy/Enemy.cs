using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Panda;

public class Enemy : MonoBehaviour {

    public float health, DPS;
    public GameObject NPC;
    public List<GameObject> otherObjects;
    protected static int targetObjectIndex;
    float distance = 999, nextActionTime;
    public float period = 1f;
    public float tendency_to_chase_player = 0.5f;
    public GameObject projectileObj;

    public bool ranged = false;
    public float maxRange = 1.5f; // for ranged enemies maxRange is bigger

    void Awake()
    {
        NPC = gameObject;
        health = 70 + (GameManager.getLevel() * Random.Range(5, 10));
        DPS = GameManager.getLevel() + Random.Range(1, 4);
    }

    // Use this for initialization
    void Start () {        
        // add player and key objects to the list if they are not in it already
        if (!otherObjects.Contains(GameObject.FindGameObjectWithTag("Player")))
            otherObjects.Add(GameObject.FindGameObjectWithTag("Player"));

        if (!otherObjects.Contains(GameObject.FindGameObjectWithTag("AI")))
            otherObjects.Add(GameObject.FindGameObjectWithTag("AI"));
    }
	
	// Update is called once per frame
	void Update () {
        if (health <= 0)
            Destroy(gameObject);
    }

    // shoots a projectile towards the target and destroys it after 3 seconds
    void rangedFire()
    {
        GameObject projectile = Instantiate(projectileObj, gameObject.transform.position, Quaternion.identity);
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, projectile.transform.rotation.z));

        projectile.GetComponent<Projectile>().target = otherObjects[targetObjectIndex].transform;
        projectile.GetComponent<Projectile>().damage = DPS;

        // Destroy the bullet after 3 seconds
        Destroy(projectile, 3);
    }

    // raycasts between the position of the enemy and the position of gameObj and detects walls (if they exist) that are between them
    bool lineOfSightBlocked(GameObject gameObj)
    {
        RaycastHit2D hit;
        
        hit = Physics2D.Raycast(NPC.transform.position + (gameObj.transform.position - NPC.transform.position).normalized, gameObj.transform.position - NPC.transform.position, Vector2.Distance(NPC.transform.position, gameObj.transform.position) - 2, LayerMask.GetMask("Unwalkable"));

        return hit.collider;
    }

    [Task]
    void updateDistance()
    {
        float dist = 999;
        float distPlayer;

        if (otherObjects[0] != null)
        {


            if (!lineOfSightBlocked(otherObjects[0]))
                distPlayer = Vector2.Distance(NPC.transform.position, otherObjects[0].transform.position);
            else
                distPlayer = 999;

            dist = distPlayer;
            targetObjectIndex = 0;
        }
        else
            return;

        if (otherObjects[1] != null)
        {
            float distCompanion;
            if (!lineOfSightBlocked(otherObjects[1]))
                distCompanion = Vector2.Distance(NPC.transform.position, otherObjects[1].transform.position);
            else
                distCompanion = 999;

            if (distCompanion < distPlayer)
            {
                dist = distCompanion;
                targetObjectIndex = 1;
            }
        }

        distance = dist;
    }

    [Task]
    void Attack()
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (distance > maxRange)
        {
            animator.SetTrigger("notAttacking");
            NPC.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            Task.current.Fail();
        }
        else
        {
            if (ranged && !(NPC.GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.FreezeAll))
                NPC.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            if (otherObjects[targetObjectIndex] != null)
            {
                if (Time.time > nextActionTime)
                {
                    nextActionTime = Time.time + period;

                    // code for the animation of companion
                    GameObject otherObj = otherObjects[targetObjectIndex];
                    if (otherObj.transform.position.x - NPC.transform.position.x != 0)
                    {
                        if (otherObj.transform.position.x - NPC.transform.position.x < 0)
                            animator.SetTrigger("attackingLeft");
                        else if (otherObj.transform.position.x - NPC.transform.position.x > 0)
                            animator.SetTrigger("attackingRight");
                    }
                    else if (otherObj.transform.position.y - NPC.transform.position.y != 0)
                    {
                        if (otherObj.transform.position.y - NPC.transform.position.y < 0)
                            animator.SetTrigger("attackingDown");
                        else if (otherObj.transform.position.y - NPC.transform.position.y > 0)
                            animator.SetTrigger("attackingUp");
                    }
                    else
                        animator.SetTrigger("notAttacking");

                    if (!ranged)
                    {
                        if (targetObjectIndex == 0)
                            otherObjects[targetObjectIndex].GetComponent<Player>().health -= NPC.GetComponent<Enemy>().DPS;
                        else
                            otherObjects[targetObjectIndex].GetComponent<Companion>().health -= NPC.GetComponent<Enemy>().DPS;
                    }
                    else
                        rangedFire();
                }
            }
            else
                animator.SetTrigger("notAttacking");
        }
    }

    [Task]
    void Chase()
    {
        if (distance > maxRange + 5 || distance < maxRange)
            Task.current.Fail();
        else
        {
            float distPlayer = 999, distCompanion = 999;
            if (otherObjects[0] != null)
                distPlayer = Vector2.Distance(NPC.transform.position, otherObjects[0].transform.position);

            if (otherObjects[1] != null)
                distCompanion = Vector2.Distance(NPC.transform.position, otherObjects[1].transform.position);

            if (distCompanion < distPlayer)
            {
                if (distPlayer - distCompanion <= tendency_to_chase_player)
                    targetObjectIndex = 0;
                else
                    targetObjectIndex = 1;
            }
            else
                targetObjectIndex = 0;

            NPC.GetComponent<AIDestinationSetter>().targetObject = otherObjects[targetObjectIndex];
        }
    }

    [Task]
    void Idle()
    {
        if (distance < maxRange + 5)
            Task.current.Fail();
    }
}
