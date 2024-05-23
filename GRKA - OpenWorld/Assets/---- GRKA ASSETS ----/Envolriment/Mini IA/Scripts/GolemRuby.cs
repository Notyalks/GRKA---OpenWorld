using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemRuby : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    public GameObject particleSystemPrefab;
    public float disappearDistance = 2f;
    private bool hasSpawnedParticle = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            navMeshAgent.SetDestination(player.transform.position);

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= disappearDistance)
            {
                gameObject.SetActive(false);

                if (!hasSpawnedParticle)
                {
                    Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
                    hasSpawnedParticle = true;
                }
          
            }
        }
    }
}
