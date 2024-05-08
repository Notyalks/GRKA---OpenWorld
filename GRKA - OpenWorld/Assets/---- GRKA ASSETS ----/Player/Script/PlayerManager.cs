using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;
    public HealthBar healthBar;
    private float vida = 100f;

    public GameObject Shield;
    public bool isInteracting;

    [Header("Player Flags")]
    public bool isAiming;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        
    }

    private void Start()
    {
        vida = 100f;
        healthBar.ColocarVidaMaxima(vida);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        isAiming = animator.GetBool("isAiming");
 
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

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

        if (other.CompareTag("Vida"))
        {
            vida += 30f;
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
}
