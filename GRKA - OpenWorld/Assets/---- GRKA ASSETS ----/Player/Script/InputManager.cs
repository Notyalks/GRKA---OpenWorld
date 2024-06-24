using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputs playerControls;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;
    Animator animator;
    PlayerUiManager uiManager;
    WeaponManager weaponManager;
    PlayerManager playerManager;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool b_Input;
    public bool walk_input;
    public bool jump_input;
    public bool aiming_input;
    public bool shoot_input;
    public bool dash_input;
    public bool changeFaceMaterialInput; // Input para trocar o material do rosto

    public float shootCooldown = 1.0f; // Intervalo de tempo entre os tiros em segundos
    private float lastShootTime = 0f; // Tempo do último tiro

    public Renderer faceRenderer; // Renderer do rosto do personagem
    public Material newFaceMaterial; // Novo material para o rosto
    private Material originalFaceMaterial; // Material original do rosto
    private bool isFaceMaterialChanged = false; // Flag para acompanhar o estado do material do rosto

    public Light areaLight; // Referência para a área light

    private bool cameraMovementEnabled = false;
    private bool playerMovementEnabled = false;

    // Adicione referências aos AudioSources
    public AudioSource shootAudioSource;
    public AudioSource footstepAudioSource;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
        uiManager = FindObjectOfType<PlayerUiManager>();
        weaponManager = FindObjectOfType<WeaponManager>();
        playerManager = FindObjectOfType<PlayerManager>();

        if (faceRenderer != null)
        {
            originalFaceMaterial = faceRenderer.material; // Armazena o material original do rosto
        }

        if (areaLight != null)
        {
            areaLight.enabled = false; // Certifica-se de que a luz está desligada inicialmente
        }
    }

    public void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerInputs();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.B.performed += i => b_Input = true;
            playerControls.PlayerActions.B.canceled += i => b_Input = false;
            playerControls.PlayerActions.Walk.performed += i => walk_input = true;
            playerControls.PlayerActions.Walk.canceled += i => walk_input = false;
            playerControls.PlayerActions.Jump.performed += i => jump_input = true;
            playerControls.PlayerActions.Jump.canceled += i => jump_input = false;
            playerControls.PlayerActions.Aim.performed += i => aiming_input = true;
            playerControls.PlayerActions.Aim.canceled += i => aiming_input = false;
            playerControls.PlayerActions.Shoot.performed += i => shoot_input = true;
            playerControls.PlayerActions.Shoot.canceled += i => shoot_input = false;
            playerControls.PlayerActions.Dash.canceled += i => dash_input = true; // Aqui somente no canceled
            playerControls.PlayerActions.ChangeFaceMaterial.performed += i => changeFaceMaterialInput = true; // Adiciona a entrada de ChangeFaceMaterial
        }

        playerControls.Enable();
    }

    public void OnDisable()
    {
        playerControls.Disable();
    }

    public void ResetInputs()
    {
        movementInput = Vector2.zero;
        cameraInput = Vector2.zero;
        cameraInputX = 0f;
        cameraInputY = 0f;
        moveAmount = 0f;
        verticalInput = 0f;
        horizontalInput = 0f;
        b_Input = false;
        walk_input = false;
        jump_input = false;
        aiming_input = false;
        shoot_input = false;
        dash_input = false;
        changeFaceMaterialInput = false;
    }

    public void EnableCameraMovement()
    {
        cameraMovementEnabled = true;
    }

    public void DisableCameraMovement()
    {
        cameraMovementEnabled = false;
    }

    public void EnableMovement()
    {
        playerMovementEnabled = true;
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleDashInput();
        HandleSprintingInput();
        HandleWalkInput();
        HandleJumpingInput();
        HandleAimingInput();
        HandleShootingInput();
        HandleChangeFaceMaterialInput(); // Adiciona a chamada para o novo método
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(horizontalInput, verticalInput, playerLocomotion.isSprinting, playerLocomotion.isWalking);

        // Tocar som de passos
        if (moveAmount > 0 && playerLocomotion.isGrounded && !footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play();
        }
        else if (moveAmount == 0 || !playerLocomotion.isGrounded)
        {
            footstepAudioSource.Stop();
        }
    }

    private void HandleSprintingInput()
    {
        if (b_Input && moveAmount > 0.5f && !playerLocomotion.isPushing)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleWalkInput()
    {
        if (walk_input && moveAmount > 0.5f && !playerLocomotion.isPushing)
        {
            playerLocomotion.isWalking = true;
        }
        else
        {
            playerLocomotion.isWalking = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jump_input && !playerLocomotion.isPushing)
        {
            jump_input = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void HandleDashInput()
    {
        if (dash_input && PlayerPrefs.GetInt("Shire2Finishe") == 1 && !playerLocomotion.isPushing)
        {
            dash_input = false; // Reseta o input após o dash
            playerLocomotion.HandleDash();
        }
    }

    private void HandleAimingInput()
    {
        // Verifica se o jogador não está empurrando algo
        if (playerLocomotion.isPushing)
        {
            aiming_input = false;
            animator.SetBool("isAiming", false);
            uiManager.crossHair.SetActive(false);
            return;
        }

        // Ativa a mira e o input de mira estiver ativo ou se o jogador estiver no ar
        if (aiming_input)
        {
            animator.SetBool("isAiming", true);
            uiManager.crossHair.SetActive(true);
        }
        else
        {
            animator.SetBool("isAiming", false);
            uiManager.crossHair.SetActive(false);
        }
    }

    private void HandleShootingInput()
    {
        if (shoot_input && aiming_input && !playerLocomotion.isPushing)
        {
            if (Time.time - lastShootTime >= shootCooldown)
            {
                lastShootTime = Time.time; // Atualiza o tempo do último tiro
                shoot_input = false; // Reseta o input de tiro
                animator.Play("Shoot"); // Aciona o trigger de tiro no Animator
                weaponManager.ShootWeapon();

                // Tocar som de tiro
                shootAudioSource.Play();
            }
        }
    }

    private void HandleChangeFaceMaterialInput()
    {
        if (changeFaceMaterialInput)
        {
            changeFaceMaterialInput = false; // Reseta o input
            if (isFaceMaterialChanged)
            {
                faceRenderer.material = originalFaceMaterial; // Volta para o material original
                if (areaLight != null)
                {
                    areaLight.enabled = false; // Desativa a luz
                }
            }
            else
            {
                faceRenderer.material = newFaceMaterial; // Troca para o novo material
                if (areaLight != null)
                {
                    areaLight.enabled = true; // Ativa a luz
                }
            }
            isFaceMaterialChanged = !isFaceMaterialChanged; // Alterna o estado
        }
    }
}
