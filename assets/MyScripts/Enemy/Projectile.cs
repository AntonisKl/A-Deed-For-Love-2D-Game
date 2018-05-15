using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed;
    public Transform target;
    public float damage;
    Vector3 normalizedDirection;

	// Use this for initialization
	void Start () {
        normalizedDirection = (target.position - transform.position).normalized;
    
        int sign = 1;
        if (target.position.y - transform.position.y < 0)
            sign = -1;
        // make projectile facing towards the target
        transform.Rotate(new Vector3(0, 0, sign * Vector3.Angle(normalizedDirection, transform.right)));
    }
	
	// Update is called once per frame
	void Update () {
        float step = speed * Time.deltaTime;// The step size is equal to speed times frame time.

        // move projectile smoothly towards the initial position of target
        transform.position += normalizedDirection * step;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // deal damage
        if (collider.gameObject.tag == "Player")
            collider.gameObject.GetComponent<Player>().health -= damage;
        else if (collider.gameObject.tag == "AI")
            collider.gameObject.GetComponent<Companion>().health -= damage;
    }
}
