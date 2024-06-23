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

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public BarraDeVida barraDeVida;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        barraDeVida.gameObject.SetActive(false); // Desativa a barra de vida inicialmente
    }

    void Update()
    {
        if (player != null)
        {
            navMeshAgent.SetDestination(player.transform.position);

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= disappearDistance)
            {
                Die();
            }
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

        Debug.Log("Golem took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);

        if (!hasSpawnedParticle)
        {
            Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
            hasSpawnedParticle = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            // Aqui você pode chamar o método TakeDamage com o valor de dano desejado
            // Por exemplo, se o GolemRuby tomar 10 de dano ao entrar em colisão com o jogador:
            TakeDamage(5);
        }
    }
}
