using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public Rigidbody rb;


    void Start()
    {
        Destroy(gameObject, lifetime);
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Boss"))
        {
            coll.GetComponent<Boss>().HP_Min -= 50;
        }
    }
}
