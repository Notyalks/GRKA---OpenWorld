using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemAmethyst : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public BarraDeVida barraDeVida;

    [Header("Particle Settings")]
    public GameObject destructionParticles; // Sistema de part�culas para a destrui��o
    public float particleLifetime = 3f; // Tempo que a part�cula fica ativa antes de ser destru�da

    private bool hasSpawnedParticle = false; // Controle para garantir que as part�culas sejam geradas apenas uma vez

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

        if (distanceToPlayer <= 40)
        {
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("rolando", true);
            navMeshAgent.speed = 15;
        }
        else if (distanceToPlayer > 50)
        {
            navMeshAgent.SetDestination(player.transform.position);
            animator.SetBool("rolando", false);
            navMeshAgent.speed = 30;
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
            barraDeVida.gameObject.SetActive(true); // Ativa a barra de vida ap�s o primeiro dano
        }

        currentHealth -= damage;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);

        Debug.Log("Golem Amethyst took damage! Current health: " + currentHealth);

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
            // Aqui voc� pode definir o valor de dano que o Golem Amethyst deve tomar
            TakeDamage(5);
        }
    }
}
