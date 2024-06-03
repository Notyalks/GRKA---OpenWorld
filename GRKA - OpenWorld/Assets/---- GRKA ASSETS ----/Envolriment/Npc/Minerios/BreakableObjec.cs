using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public GameObject itemToDrop; // Objeto que será dropado
    public int maxHealth = 100; // Saúde máxima do objeto
    private int currentHealth; // Saúde atual do objeto
    [SerializeField] private BarraDeVida barraDeVida; // Referência à barra de vida

    void Start()
    {
        currentHealth = maxHealth;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        Debug.Log("Objeto inicializado com saúde: " + currentHealth);
    }

    // Método para causar dano ao objeto
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        Debug.Log("Objeto recebeu dano de: " + damage + ". Saúde atual: " + currentHealth);

        // Verificar se a saúde do objeto é menor ou igual a zero
        if (currentHealth <= 0)
        {
            Debug.Log("Objeto destruído.");
            DestroyObject();
        }
    }

    // Método para destruir o objeto
    private void DestroyObject()
    {
        if (itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
            Debug.Log("Item dropado: " + itemToDrop.name);
        }

        Destroy(gameObject);
    }

    // Detectar colisões com outros objetos
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
