using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject itemToDrop; // Objeto que ser� dropado
    public int maxHealth = 100; // Sa�de m�xima do objeto
    private int currentHealth; // Sa�de atual do objeto
    [SerializeField] private BarraDeVida barraDeVida; // Refer�ncia � barra de vida

    [Header("Drop Settings")]
    public bool shouldDropItem = true; // Toggle para definir se o objeto deve dropar itens

    [Header("Particle Settings")]
    public GameObject destructionParticles; // Sistema de part�culas para a destrui��o
    public float particleLifetime = 5f; // Tempo de vida das part�culas

    private bool isHealthBarActive = false; // Indica se a barra de vida est� ativa

    private MissionDestroyObjects missionDestroyObjects; // Refer�ncia ao script da miss�o

    void Start()
    {
        currentHealth = maxHealth;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        barraDeVida.gameObject.SetActive(false); // Desativa a barra de vida inicialmente

        // Procura o objeto que cont�m o script MissionDestroyObjects na cena
        missionDestroyObjects = FindObjectOfType<MissionDestroyObjects>();
    }

    // M�todo para causar dano ao objeto
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        if (!isHealthBarActive)
        {
            barraDeVida.gameObject.SetActive(true); // Ativa a barra de vida ap�s o primeiro dano
            isHealthBarActive = true;
        }

        currentHealth -= damage;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);

        // Verificar se a sa�de do objeto � menor ou igual a zero
        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }

    // M�todo para destruir o objeto
    private void DestroyObject()
    {
        if (shouldDropItem && itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }

        if (destructionParticles != null)
        {
            GameObject particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            Destroy(particles, particleLifetime); // Destr�i a part�cula ap�s particleLifetime segundos
        }

        // Notifica o sistema de miss�o que este objeto foi destru�do
        if (missionDestroyObjects != null)
        {
            missionDestroyObjects.ObjectDestroyed(gameObject);
        }

        Destroy(gameObject);
    }

    // Detectar colis�es com outros objetos
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
