using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    public Rigidbody rb;

    [Header("Health Settings")]
    public HealthBar healthBar;
    public float vida = 100f;
    public float vidaMaxima = 100f;

    [Header("Health Regeneration Settings")]
    [Tooltip("Intervalo de tempo para regeneração (segundos)")]
    public float regenerationInterval = 1f; // Intervalo de tempo para regeneração (segundos)
    [Tooltip("Quantidade de vida regenerada por intervalo")]
    public float regenerationAmount = 1f; // Quantidade de vida regenerada por intervalo
    [Tooltip("Controle de ativação da regeneração")]
    [SerializeField]
    private bool enableHealthRegeneration = true; // Controle de ativação da regeneração

    private Coroutine regenerationCoroutine;

    [Header("Shield Settings")]
    public float shieldVida = 50f;
    public float shieldVidaMaxima = 50f;
    public GameObject Shield;
    public bool isShieldActive = false;
    public float shieldRechargeTime = 5f;

    [Header("Damage Settings")]
    public float launchUpForce = 10f;
    public float launchBackForce = 5f;

    [Header("Player Flags")]
    public bool isInteracting;
    public bool isAiming;
    public bool isDead = false;
    public bool Dead = false;
    public bool canMoveCam = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        vida = vidaMaxima;
        healthBar.ColocarVidaMaxima(vidaMaxima);
        Cursor.lockState = CursorLockMode.Locked;

        // Ativar o escudo no início se a Shire3 estiver completa
        if (PlayerPrefs.GetInt("Shire3Finishe", 0) == 1)
        {
            ShieldOn();
        }
        else
        {
            Shield.SetActive(false);
            isShieldActive = false;
        }

        // Iniciar regeneração de vida se Shire4 estiver completa e a regeneração estiver habilitada
        if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 1 && enableHealthRegeneration)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        isAiming = animator.GetBool("isAiming");

        if (vida <= 0 && !isDead)
        {
            Debug.Log("Não morreu");
            rb.constraints = RigidbodyConstraints.FreezePosition;
            inputManager.OnDisable();
            isDead = true;
            animator.SetBool("isDead", true);

            // Parar regeneração de vida se o jogador estiver morto
            if (regenerationCoroutine != null)
            {
                StopCoroutine(regenerationCoroutine);
            }
        }

        // Verificar se Shire3 foi completada e ativar ou desativar o escudo conforme necessário
        if (PlayerPrefs.GetInt("Shire3Finishe", 0) == 1 && !isShieldActive)
        {
            ShieldOn();
        }
        else if (PlayerPrefs.GetInt("Shire3Finishe", 0) == 0 && isShieldActive)
        {
            // Desativar o escudo sem iniciar a recarga
            Shield.SetActive(false);
            isShieldActive = false;
            Debug.Log("Escudo desativado devido à Shire3 não estar completa.");
        }

        // Verificar se Shire4 foi completada e iniciar ou parar a regeneração de vida conforme necessário
        if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 1 && enableHealthRegeneration && regenerationCoroutine == null)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealth());
        }
        else if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 0 && regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
            Debug.Log("Regeneração de vida desativada devido à Shire4 não estar completa.");
        }
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        if (canMoveCam == true)
        {
            cameraManager.HandleAllCameraMovement();
        }
        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dano"))
        {
            TomarDano(10f);
        }

        if (other.CompareTag("Dano1"))
        {
            TomarDano(10f);
            rb.velocity = Vector3.zero;
            Vector3 launchDirection = -other.transform.forward * launchBackForce + Vector3.up * launchUpForce;
            rb.AddForce(launchDirection, ForceMode.Impulse);
        }

        if (other.CompareTag("Vida"))
        {
            RestaurarVida(30f);
        }

        if (other.CompareTag("Lava"))
        {
            TomarDano(200f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dano"))
        {
            Debug.Log("Tomou dano");
            TomarDano(10f);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("bafo"))
        {
            TomarDano(1f);
        }
    }

    public void ShieldOn()
    {
        if (!isShieldActive)
        {
            Shield.SetActive(true);
            isShieldActive = true;
            shieldVida = shieldVidaMaxima;
            Debug.Log("Escudo ativado. Vida do escudo: " + shieldVida);
        }
    }

    public void ShieldOff()
    {
        if (isShieldActive)
        {
            Shield.SetActive(false);
            isShieldActive = false;
            Debug.Log("Escudo desativado.");
            StartCoroutine(RechargeShieldAfterDelay(shieldRechargeTime));
        }
    }

    private IEnumerator RechargeShieldAfterDelay(float delay)
    {
        Debug.Log("Recarregando escudo em " + delay + " segundos.");
        yield return new WaitForSeconds(delay);
        ShieldOn();
    }

    public void DeadAnimationComplete()
    {
        Debug.Log("Entrou aqui");
        animator.SetBool("isDead", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestaurarVida(float quantidade)
    {
        vida += quantidade;
        if (vida > vidaMaxima)
        {
            vida = vidaMaxima;
        }
        healthBar.AlterarVida(vida);
        Debug.Log("Vida restaurada: " + vida);
    }

    public void TomarDano(float dano)
    {
        if (isShieldActive)
        {
            shieldVida -= dano;
            Debug.Log("Dano absorvido pelo escudo: " + dano + ". Vida do escudo restante: " + shieldVida);
            if (shieldVida <= 0)
            {
                ShieldOff();
            }
        }
        else
        {
            vida -= dano;
            healthBar.AlterarVida(vida);
            Debug.Log("Dano tomado: " + dano + ". Vida restante: " + vida);
            if (vida <= 0 && !isDead)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
                inputManager.OnDisable();
                isDead = true;
                animator.SetBool("isDead", true);
            }
        }
    }

    private IEnumerator RegenerateHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenerationInterval);
            if (vida < vidaMaxima && !isDead && enableHealthRegeneration)
            {
                vida += regenerationAmount;
                if (vida > vidaMaxima)
                {
                    vida = vidaMaxima;
                }
                healthBar.AlterarVida(vida);
                Debug.Log("Vida regenerada: " + vida);
            }
        }
    }
}

