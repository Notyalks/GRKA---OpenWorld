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
    public HealthBar healthBar;
    public float vida = 100f;
    public float vidaMaxima = 100f; // Adicionei uma variável para vida máxima

    public float shieldVida = 50f; // Vida do escudo
    public float shieldVidaMaxima = 50f; // Vida máxima do escudo
    public GameObject Shield;
    public bool isShieldActive = false;

    public float shieldRechargeTime = 5f; // Tempo de recarga do escudo

    public float launchUpForce = 10f;
    public float launchBackForce = 5f;

    public bool isInteracting;

    [Header("Player Flags")]
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
        vida = vidaMaxima; // Define a vida inicial como a vida máxima
        healthBar.ColocarVidaMaxima(vidaMaxima);
        Cursor.lockState = CursorLockMode.Locked;

        // Ativar o escudo no início
        ShieldOn();
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
        if (!isShieldActive) // Verifica se o escudo já não está ativo
        {
            Shield.SetActive(true);
            isShieldActive = true;
            shieldVida = shieldVidaMaxima; // Reseta a vida do escudo para a vida máxima
            Debug.Log("Escudo ativado. Vida do escudo: " + shieldVida);
        }
    }

    public void ShieldOff()
    {
        Shield.SetActive(false);
        isShieldActive = false;
        Debug.Log("Escudo desativado.");

        // Inicia a corrotina para recarregar o escudo após o tempo de recarga
        StartCoroutine(RechargeShieldAfterDelay(shieldRechargeTime));
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

    private void TomarDano(float dano)
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
}
