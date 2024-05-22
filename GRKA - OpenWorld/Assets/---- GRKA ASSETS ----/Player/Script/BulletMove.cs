using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public Rigidbody rb;
    WeaponManager manager;


    private void Awake()
    {
        manager = FindAnyObjectByType<WeaponManager>();
    }

    void Start()
    {
        rb.AddForce(manager.aaaaa * speed, ForceMode.Impulse);
        Destroy(gameObject, lifetime);
    }

    
}
