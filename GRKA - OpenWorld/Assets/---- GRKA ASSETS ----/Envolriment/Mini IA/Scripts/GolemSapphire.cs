using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemSapphire : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    public int life;
    bool isShootin;
    public GameObject pedra;
    public Transform shootPoint;

    void Start()
    {
       player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        life = 3;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        navMeshAgent.transform.LookAt(player.transform.position);

        if (distanceToPlayer <= 50)
        {
            if (!isShootin)
            {
                animator.SetBool("jogando", true);
                navMeshAgent.isStopped = true;
                InvokeRepeating("Shoot", 0f, 1.5f);
                isShootin = true;
            }
            
        }
        else
        {
            CancelInvoke("Shoot");
            isShootin = false;
        }

        if (distanceToPlayer > 50)
        {
            navMeshAgent.isStopped = false;
            animator.SetBool("jogando", false);
            navMeshAgent.SetDestination(player.transform.position);
            navMeshAgent.speed = 30;
        }

        if (life <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void Shoot()
    {
        Instantiate(pedra, shootPoint.position, shootPoint.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            life--;
        }
    }
}