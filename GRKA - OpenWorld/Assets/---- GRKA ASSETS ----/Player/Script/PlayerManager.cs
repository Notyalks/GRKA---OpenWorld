using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float vidaMaxima = 100f;  // Adicionei uma variável para vida máxima
    public float launchUpForce = 10f;
    public float launchBackForce = 5f;

    public GameObject Shield;
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
        vida = vidaMaxima;  // Define a vida inicial como a vida máxima
        healthBar.ColocarVidaMaxima(vidaMaxima);
        Cursor.lockState = CursorLockMode.Locked;
        //PlayerPrefs.DeleteAll();
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
            vida -= 10f;
            healthBar.AlterarVida(vida);
        }

        if (other.CompareTag("Dano1"))
        {
            vida -= 10f;
            healthBar.AlterarVida(vida);
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
            vida -= 200f;
            healthBar.AlterarVida(vida);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dano"))
        {
            Debug.Log("tomou dano");
            vida -= 10f;
            healthBar.AlterarVida(vida);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("bafo"))
        {
            vida -= 1f;
            healthBar.AlterarVida(vida);
        }
    }

    public void ShieldOn()
    {
        Shield.SetActive(true);
    }

    public void ShieldOff()
    {
        Shield.SetActive(false);
    }

    public void DeadAnimationComplete()
    {
        Debug.Log("entrou aqui");
        animator.SetBool("isDead", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // restartMenu.SetActive(true);
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
}
