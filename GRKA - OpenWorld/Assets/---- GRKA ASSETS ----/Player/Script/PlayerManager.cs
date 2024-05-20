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
    HitBoss boss;
    Rigidbody rb;
    public HealthBar healthBar;
    public float vida = 100f;
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
        boss = FindObjectOfType<HitBoss>();
    }

    private void Start()
    {
        vida = 100f;
        healthBar.ColocarVidaMaxima(vida);
        Cursor.lockState = CursorLockMode.Locked;
        // PlayerPrefs.DeleteAll();
        
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        isAiming = animator.GetBool("isAiming");

        if (vida <= 0 && !isDead)
        {
            Debug.Log("Nãomorreu");
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
            vida += 30f;
            healthBar.AlterarVida(vida);
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
            Debug.Log("tomoudano");
            vida -= 10f;
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
        Debug.Log("entrouaqui");
        animator.SetBool("isDead", false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // restartMenu.SetActive(true);
    }
}
