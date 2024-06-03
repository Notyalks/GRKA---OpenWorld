using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject itemToDrop; // Objeto que ser� dropado
    public int maxHealth = 100; // Sa�de m�xima do objeto
    private int currentHealth; // Sa�de atual do objeto
    [SerializeField] private BarraDeVida barraDeVida; // Refer�ncia � barra de vida

    void Start()
    {
        currentHealth = maxHealth;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        Debug.Log("Objeto inicializado com sa�de: " + currentHealth);
    }

    // M�todo para causar dano ao objeto
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        Debug.Log("Objeto recebeu dano de: " + damage + ". Sa�de atual: " + currentHealth);

        // Verificar se a sa�de do objeto � menor ou igual a zero
        if (currentHealth <= 0)
        {
            Debug.Log("Objeto destru�do.");
            DestroyObject();
        }
    }

    // M�todo para destruir o objeto
    private void DestroyObject()
    {
        if (itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
            Debug.Log("Item dropado: " + itemToDrop.name);
        }

        Destroy(gameObject);
    }

    // Detectar colis�es com outros objetos
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fire"))
        {
            Debug.Log("Colidiu com fogo. Aplicando dano.");
            TakeDamage(5); // Exemplo de dano
        }
    }

    // Detectar entrada de trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fire"))
        {
            Debug.Log("Entrou em contato com fogo. Aplicando dano.");
            TakeDamage(5); // Exemplo de dano
        }
    }
}
