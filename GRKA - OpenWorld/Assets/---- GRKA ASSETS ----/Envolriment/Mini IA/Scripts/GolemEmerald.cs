using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemEmerald : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    Animator anim;
    public float attackDistance = 2f;
    public GameObject coll;
    public int life;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        life = 3;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackDistance)
        {
            anim.SetBool("bate", true);
            coll.SetActive(true);
            navMeshAgent.isStopped = true;
        }
        else
        {
            anim.SetBool("bate", false);
            coll.SetActive(false);
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.transform.position);
        }

        if (life <= 0)
        {
            navMeshAgent.isStopped = true;
            Destroy(gameObject, 2f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            life--;
        }
    }
}
