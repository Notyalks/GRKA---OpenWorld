using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowArrow : MonoBehaviour
{
    public GameObject player;
    private Rigidbody rb;
    public float force;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Head");
        transform.LookAt(player.transform.position);
        Vector3 direction = player.transform.position - transform.position;
        transform.LookAt(player.transform.position);
        rb.velocity = new Vector3(direction.x, direction.y, direction.z).normalized * force;
        transform.LookAt(player.transform.position);
        Destroy(gameObject, 3f);
    }

    
}
