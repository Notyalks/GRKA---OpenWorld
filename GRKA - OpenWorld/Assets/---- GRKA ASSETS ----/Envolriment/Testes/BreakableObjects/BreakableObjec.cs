using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject itemToDrop; // Objeto que será dropado
    public int maxHealth = 100; // Saúde máxima do objeto
    private int currentHealth; // Saúde atual do objeto
    [SerializeField] private BarraDeVida barraDeVida; // Referência à barra de vida

    [Header("Drop Settings")]
    public bool shouldDropItem = true; // Toggle para definir se o objeto deve dropar itens

    [Header("Particle Settings")]
    public GameObject destructionParticles; // Sistema de partículas para a destruição
    public float particleLifetime = 5f; // Tempo de vida das partículas

    private bool isHealthBarActive = false; // Indica se a barra de vida está ativa

    private MissionDestroyObjects missionDestroyObjects; // Referência ao script da missão

    void Start()
    {
        currentHealth = maxHealth;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        barraDeVida.gameObject.SetActive(false); // Desativa a barra de vida inicialmente

        // Procura o objeto que contém o script MissionDestroyObjects na cena
        missionDestroyObjects = FindObjectOfType<MissionDestroyObjects>();
    }

    // Método para causar dano ao objeto
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        if (!isHealthBarActive)
        {
            barraDeVida.gameObject.SetActive(true); // Ativa a barra de vida após o primeiro dano
            isHealthBarActive = true;
        }

        currentHealth -= damage;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);

        // Verificar se a saúde do objeto é menor ou igual a zero
        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }

    // Método para destruir o objeto
    private void DestroyObject()
    {
        if (shouldDropItem && itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }

        if (destructionParticles != null)
        {
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(particles, particleLifetime); // Destrói a partícula após particleLifetime segundos
        }

        // Notifica o sistema de missão que este objeto foi destruído
        if (missionDestroyObjects != null)
        {
            missionDestroyObjects.ObjectDestroyed(gameObject);
        }

        Destroy(gameObject);
    }

    // Detectar colisões com outros objetos
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            TakeDamage(5); // Exemplo de dano
        }
    }

    // Detectar entrada de trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            TakeDamage(5); // Exemplo de dano
        }
    }
}
