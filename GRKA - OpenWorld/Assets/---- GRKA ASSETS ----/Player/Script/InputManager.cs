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
    FixedJoint grabjoint;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;
    float ikforce = 0;
    public bool grabou;

    public bool b_Input;
    public bool walk_input;
    public bool jump_input;
    public bool aiming_input;
    public bool shoot_input;
    public bool grab_input;
    public bool shield_input;
    public bool dash_input;
    bool QPressed = false;
    bool Click = false;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
        uiManager = FindObjectOfType<PlayerUiManager>();
        weaponManager = FindObjectOfType<WeaponManager>();
        playerManager = FindObjectOfType<PlayerManager>();

        grabou = false;
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
            playerControls.PlayerActions.Grab.performed += i => grab_input = true;
            playerControls.PlayerActions.Grab.canceled += i => grab_input = false;
            playerControls.PlayerActions.Shield.started += i => shield_input = true;
            playerControls.PlayerActions.Shield.canceled += i => shield_input = false;
            playerControls.PlayerActions.Dash.performed += i => dash_input = true;
            playerControls.PlayerActions.Dash.canceled += i => dash_input = false;
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
        grab_input = false;
        shield_input = false;
        dash_input = false;
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleGrab();
        if (grab_input)
            return;
        HandleDashInput();
        HandleSprintingInput();
        HandleWalkInput();
        HandleJumpingInput();
        HandleAimingInput();
        HandleShootingInput();
        HandleShieldInput();
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
        if (dash_input && PlayerPrefs.GetInt("Shire3Finishe") == 1)
        {
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

    private void HandleShieldInput()
    {
        if (Click && PlayerPrefs.GetInt("Shire1Finishe") == 1)
        {
            playerManager.ShieldOn();
            Click = false; // Resetar a variável para garantir que o escudo seja ativado apenas uma vez por clique
        }
    }

    private void HandleGrab()
    {
        if (grab_input)
        {
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 1, 511))
            {
                if (hit.collider.CompareTag("Push"))
                {
                    ikforce = Mathf.Lerp(ikforce, 1, Time.fixedDeltaTime * 1);
                }
                if (ikforce > 0.9f)
                {
                    if (!grabjoint)
                    {

                        animator.SetBool("isGrabing", true);
                        grabjoint = hit.collider.gameObject.AddComponent<FixedJoint>();
                        grabjoint.connectedBody = playerLocomotion.rb;

                    }
                }
            }
        }
        else
        {
            if (grabjoint)
            {

                Destroy(grabjoint);
                animator.SetBool("isGrabing", false);
            }
        }
    }


    private void FixedUpdate()
    {
        if (!grab_input)
        {
            ikforce = Mathf.Lerp(ikforce, 0, Time.fixedDeltaTime * 3);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Verifica se a tecla Q acabou de ser pressionada
        {
            if (!QPressed) // Se não estava pressionada antes, marca como pressionada e define Click como verdadeiro
            {
                QPressed = true;
                Click = true;
            }
            else if (QPressed)// Se estava pressionada antes, marca como não pressionada e define Click como falso
            {
                QPressed = false;
                Click = false;
            }
        }

    }
}
