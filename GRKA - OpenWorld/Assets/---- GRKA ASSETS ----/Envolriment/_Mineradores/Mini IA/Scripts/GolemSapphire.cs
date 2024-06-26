using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemSapphire : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isShooting;
    public GameObject pedra;
    public Transform shootPoint;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public BarraDeVida barraDeVida;

    [Header("Particle Settings")]
    public GameObject destructionParticles; // Sistema de partículas para a destruição
    public float particleLifetime = 3f; // Tempo que a partícula fica ativa antes de ser destruída

    private bool hasSpawnedParticle = false; // Controle para garantir que as partículas sejam geradas apenas uma vez

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        barraDeVida.gameObject.SetActive(false); // Desativa a barra de vida inicialmente
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        navMeshAgent.transform.LookAt(player.transform.position);

        if (distanceToPlayer <= 50)
        {
            if (!isShooting)
            {
                animator.SetBool("jogando", true);
                navMeshAgent.isStopped = true;
                InvokeRepeating("Shoot", 0f, 1.5f);
                isShooting = true;
            }
        }
        else
        {
            CancelInvoke("Shoot");
            isShooting = false;
        }

        if (distanceToPlayer > 50)
        {
            navMeshAgent.isStopped = false;
            animator.SetBool("jogando", false);
            navMeshAgent.SetDestination(player.transform.position);
            navMeshAgent.speed = 30;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Shoot()
    {
        Instantiate(pedra, shootPoint.position, shootPoint.rotation);
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

        Debug.Log("Golem Sapphire took damage! Current health: " + currentHealth);

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

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            // Aqui você pode definir o valor de dano que o Golem Sapphire deve tomar
            TakeDamage(5);
        }
    }
}
