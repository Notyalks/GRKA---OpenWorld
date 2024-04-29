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
}
