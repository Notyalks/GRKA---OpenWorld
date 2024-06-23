using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemEmerald : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    private Animator anim;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public BarraDeVida barraDeVida;

    [Header("Particle Settings")]
    public GameObject destructionParticles; // Sistema de partículas para a destruição
    public float particleLifetime = 3f; // Tempo que a partícula fica ativa antes de ser destruída

    private bool hasSpawnedParticle = false; // Controle para garantir que as partículas sejam geradas apenas uma vez

    [Header("Attack Settings")]
    public float attackDistance = 2f;
    public GameObject coll;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        barraDeVida.gameObject.SetActive(false); // Desativa a barra de vida inicialmente
    }

    void Update()
    {
        if (player == null) return;

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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        if (!barraDeVida.gameObject.activeSelf)
        {
            barraDeVida.gameObject.SetActive(true); // Ativa a barra de vida após o primeiro dano
        }

        currentHealth -= damage;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);

        Debug.Log("Golem Emerald took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!hasSpawnedParticle)
        {
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(particles, particleLifetime);
            hasSpawnedParticle = true;
        }

        navMeshAgent.isStopped = true;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            // Aqui você pode definir o valor de dano que o Golem Emerald deve tomar
            TakeDamage(5);
        }
    }
}
