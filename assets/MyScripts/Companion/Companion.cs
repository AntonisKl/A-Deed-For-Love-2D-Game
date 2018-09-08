using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pathfinding;
using Panda;
using System.Linq;

public class Companion : MonoBehaviour
{
    public float health, DPS, maxHealth;
    public GameObject NPC;
    public List<GameObject> otherObjects;
    protected static int targetObjectIndex;
    private float nextActionTime = 0.0f;
    public float period = 1f;
    public string[] hints;
    Animator animator;
    TextMesh text;

    [Task] float distanceFromPlayer = 999, distanceFromOther = 999;

    void Awake()
    {
        NPC = gameObject;
    }

    // Use this for initialization
    void Start()
    {
        health = GameManager.instance.companionHealth;
        maxHealth = 50;
        DPS = 3;

        animator = gameObject.GetComponent<Animator>();

        otherObjects = new List<GameObject>();
        text = gameObject.GetComponentInChildren<TextMesh>();

        if (SceneManager.GetActiveScene().name == "start")
            StartCoroutine(displayHints());
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            Destroy(gameObject);
        GameManager.instance.companionHealth = health;
    }

    IEnumerator displayHints()
    {
        yield return new WaitForSeconds(1);
        foreach (string hint in hints)
        {
            StartCoroutine(displayText(hint));
            yield return new WaitForSeconds(6);
        }
    }

    IEnumerator displayText(string txt)
    {
        text.text = txt;
        StartCoroutine(FadeTextToFullAlpha(0.9f, text));
        yield return new WaitForSeconds(4);
        StartCoroutine(FadeTextToZeroAlpha(0.9f, text));
        yield return new WaitForSeconds(1);
    }

    IEnumerator FadeTextToFullAlpha(float t, TextMesh i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeTextToZeroAlpha(float t, TextMesh i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    void getOtherObjects()
    {
        // add player and key objects to the list if they are not in it already
        if (!otherObjects.Contains(GameObject.FindGameObjectWithTag("Player")))
            otherObjects.Add(GameObject.FindGameObjectWithTag("Player"));

        if (!otherObjects.Contains(GameObject.FindGameObjectWithTag("Key")))
            otherObjects.Add(GameObject.FindGameObjectWithTag("Key"));

        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Health Pickup");

        if (pickups != null)
        {
            foreach (GameObject pickup in pickups)
            {
                if (!otherObjects.Contains(pickup))
                    otherObjects.Add(pickup);
            }
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null)
        {
            foreach (GameObject enemy in enemies)
            {
                if (!otherObjects.Contains(enemy))
                    otherObjects.Add(enemy);
            }
        }
    }

    // same as the lineOfSightBlocked of the Enemy
    bool lineOfSightBlocked(GameObject gameObj)
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(
            NPC.transform.position + (gameObj.transform.position - NPC.transform.position).normalized,
            gameObj.transform.position - NPC.transform.position,
            Vector2.Distance(NPC.transform.position, gameObj.transform.position) - 2, LayerMask.GetMask("Unwalkable"));

        return hit.collider;
    }

    [Task]
    void updateDistances()
    {
        getOtherObjects();

        if (otherObjects[0] != null)
        {
            if (!lineOfSightBlocked(otherObjects[0]))
                // update distance from player
                distanceFromPlayer =
                    Vector2.Distance(gameObject.transform.position, otherObjects[0].transform.position);
            else
                distanceFromPlayer = 999;
        }

        float dist = 999;


        for (int i = 1; i < otherObjects.Count; i++)
        {
            if (otherObjects[i] != null)
            {
                float otherDist;
                if (!lineOfSightBlocked(otherObjects[i])) // only if the target is in line of sight, update its distance
                    otherDist = Vector2.Distance(gameObject.transform.position, otherObjects[i].transform.position);
                else
                    otherDist = 999;

                if (otherDist < dist)
                {
                    dist = otherDist;
                    targetObjectIndex = i;
                }
            }
        }

        distanceFromOther = dist;
    }

    [Task]
    void Attack()
    {
        getOtherObjects();

        if (distanceFromOther > 1.5f || otherObjects[targetObjectIndex].tag != "Enemy")
        {
            if (animator)
                animator.SetTrigger("notAttacking");
            Task.current.Fail();
        }
        else
        {
            if (otherObjects[targetObjectIndex] != null && otherObjects[targetObjectIndex].tag == "Enemy")
            {
                if (Time.time > nextActionTime)
                {
                    nextActionTime = Time.time + period;

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

                    otherObjects[targetObjectIndex].GetComponent<Enemy>().health -=
                        gameObject.GetComponent<Companion>().DPS;
                }
            }
            else
                animator.SetTrigger("notAttacking");
        }
    }

    [Task]
    void Chase()
    {
        if (animator)
            animator.SetTrigger("notAttacking");
        getOtherObjects();
        if ((distanceFromPlayer < 8 && distanceFromOther > 4) ||
            (distanceFromOther < 1.5 && otherObjects[targetObjectIndex].tag == "Enemy"))
        {
            // reset previously changed parameters
            gameObject.GetComponent<AIPath>().slowdownDistance = 1;
            gameObject.GetComponent<AIPath>().whenCloseToDestination = CloseToDestinationMode.Stop;
            gameObject.GetComponent<AIPath>().endReachedDistance = 2;

            Task.current.Fail();
        }
        else
        {
            // change some parameters temporarily in order for the AI companion to go exactly at the key
            gameObject.GetComponent<AIPath>().slowdownDistance = 0.01f;
            gameObject.GetComponent<AIPath>().whenCloseToDestination =
                CloseToDestinationMode.ContinueToExactDestination;
            gameObject.GetComponent<AIPath>().endReachedDistance = 0.1f;

            gameObject.GetComponent<AIDestinationSetter>().targetObject = otherObjects[targetObjectIndex];
        }
    }

    [Task]
    void FollowPlayer()
    {
        if (animator)
            animator.SetTrigger("notAttacking");
        if (distanceFromOther < 8)
            Task.current.Fail();
        else
        {
            // target the player
            gameObject.GetComponent<AIDestinationSetter>().targetObject = otherObjects[0];
            targetObjectIndex = 0;
        }
    }

    [Task] bool True = true;
}