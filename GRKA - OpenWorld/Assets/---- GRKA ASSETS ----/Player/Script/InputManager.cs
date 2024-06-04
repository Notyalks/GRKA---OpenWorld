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

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
        uiManager = FindObjectOfType<PlayerUiManager>();
        weaponManager = FindObjectOfType<WeaponManager>();
        playerManager = FindObjectOfType<PlayerManager>();
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
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSpriting, playerLocomotion.isWalking);
    }

    private void HandleSprintingInput()
    {
        if (b_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSpriting = true;
        }
        else
        {
            playerLocomotion.isSpriting = false;
        }
    }

    private void HandleWalkInput()
    {
        if (walk_input && moveAmount > 0.5f)
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
        if (jump_input)
        {
            jump_input = false;
            playerLocomotion.HandleJumping();
        }
    }

    private void HandleDashInput()
    {
        if (dash_input && PlayerPrefs.GetInt("Shire2Finishe") == 1)
        {
            dash_input = false; // Reseta o input após o dash
            playerLocomotion.HandleDash();
        }
    }

    private void HandleAimingInput()
    {
        if (!playerLocomotion.isGrounded)
        {
            aiming_input = false;
            return;
        }
        else
        {
            if (verticalInput != 0 || horizontalInput != 0)
            {
                aiming_input = false;
                animator.SetBool("isAiming", false);
                uiManager.crossHair.SetActive(false);
                return;
            }

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
    }

    private void HandleShootingInput()
    {
        if (shoot_input && aiming_input)
        {
            shoot_input = false;
            weaponManager.ShootWeapon();
        }
    }
}
