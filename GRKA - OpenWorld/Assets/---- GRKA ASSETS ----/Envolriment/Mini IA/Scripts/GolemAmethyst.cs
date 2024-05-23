using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemAmethyst : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public int life;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        life = 5;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= 40)
        {
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("rolando", true);
            navMeshAgent.speed = 15;
        }

        if (distanceToPlayer > 50)
        {
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("rolando", false);
            navMeshAgent.speed = 30;
        }

        if(life <= 0)
        {
            Destroy(this.gameObject);
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
