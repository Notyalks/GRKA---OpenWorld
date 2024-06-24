using UnityEngine;
using System.Collections;

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

    // Referência ao AudioSource
    private AudioSource audioSource;
    public AudioClip destructionSound; // Som de destruição

    void Start()
    {
        currentHealth = maxHealth;
        barraDeVida.AlterarBarraDeVida(currentHealth, maxHealth);
        barraDeVida.gameObject.SetActive(false); // Desativa a barra de vida inicialmente

        // Procura o objeto que contém o script MissionDestroyObjects na cena
        missionDestroyObjects = FindObjectOfType<MissionDestroyObjects>();

        // Obter o componente AudioSource
        audioSource = GetComponent<AudioSource>();

        // Atribuir o clipe de som de destruição ao AudioSource
        if (audioSource != null && destructionSound != null)
        {
            audioSource.clip = destructionSound;
        }
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

        // Tocar som de destruição
        if (audioSource != null && destructionSound != null)
        {
            // Criar um objeto temporário para tocar o som
            GameObject tempAudioSource = new GameObject("TempAudio");
            AudioSource tempSource = tempAudioSource.AddComponent<AudioSource>();
            tempSource.clip = destructionSound;
            tempSource.Play();
            Destroy(tempAudioSource, destructionSound.length); // Destruir o objeto temporário após o som terminar
        }

        // Notifica o sistema de missão que este objeto foi destruído
        if (missionDestroyObjects != null)
        {
            missionDestroyObjects.ObjectDestroyed(gameObject);
        }

        // Destruir o objeto imediatamente
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
