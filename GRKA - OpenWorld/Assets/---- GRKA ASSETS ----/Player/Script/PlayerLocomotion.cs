using UnityEngine;
using System.Collections;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    Animator animator;
    Vector3 moveDirection;
    public Vector3 lastGrabLadderDirection;
    Transform cam;
    public Rigidbody rb;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private GameObject dashIndicator; // Indicador de teleporte

    [Header("Falling")]
    public float inAirTIme;
    public float learpingVelocity;
    public float fallingVelocity;
    public float rayCAstHeightOffSet = 0.5f;
    public float maxDistance = 1f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isWalking;
    public bool isGrounded;
    public bool isJumping;
    public bool isClimbing;
    public bool isDashing;
    public bool isPushing;

    [Header("Movement Speeds")]
    public float walkingSpeed = 7;
    public float runningSpeed = 7;
    public float spritingSpeed = 20;
    public float rotationSpeed = 15;
    public float dashSpeedInGround = 20;
    public float dashSpeedInAir = 10;
    public float climbingSpeed = 4;
    public float pushingSpeed = 1.5f;

    [Header("Idle 2 Settings")]
    public float idleTimeThreshold = 5f; // Tempo em segundos para transi��o para Idle 2
    private float idleTimer;
    private bool isIdle2;
    private bool idle2Triggered; // Estado para verificar se a anima��o Idle 2 foi ativada

    [Header("Jumps")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;
    public float jumpSpeed;

    [Header("Movement Variables")]
    Quaternion targetRotation;
    Quaternion playerRotation;
    public float rot;

    [Header("Player Health")]
    public float health = 20f; // Vida inicial do jogador
    public float minSpeedMultiplier = 0.5f; // Multiplicador m�nimo de velocidade quando a sa�de est� em 0%
    public float originalWalkingSpeed;

    // Refer�ncia ao topo da escada
    private Transform ladderTop;

    // Velocidade de movimento original antes de empurrar
    private float originalRunningSpeed;

    [Header("Dash")]
    public LayerMask nonTeleportableLayer; // LayerMask para objetos que o dash n�o deve atravessar
    public float dashCooldown = 3f; // Tempo de recarga do dash em segundos
    public float dashDistance = 10f; // Dist�ncia m�xima do dash
    public TerrainCollider terrainCollider; // Refer�ncia ao TerrainCollider
    private bool canDash = true; // Vari�vel para controlar se o jogador pode ou n�o dar o dash

    // Estado de escalada
    private int climbState; // 0: Idle, 1: Idle na parede, 2: Descendo, 3: Subindo, 4: Topo da escada
    private bool isReachingTop;

    public void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cam = Camera.main.transform;
        isClimbing = false;
        isDashing = false;
        isPushing = false;

        // Desativar o indicador inicialmente
        if (dashIndicator != null)
        {
            dashIndicator.SetActive(false);
        }

        // Salva a velocidade original de corrida
        originalRunningSpeed = runningSpeed;
        originalWalkingSpeed = walkingSpeed;

        isIdle2 = false; // Inicializa o estado Idle 2 como falso
        idle2Triggered = false;
        climbState = 0; // Inicializa o estado de escalada como Idle
    }

    public void HandleAllMovement()
    {
        if (playerManager.isDead)
            return;

        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (isJumping || isClimbing)
            return;

        if (inputManager.moveAmount > 0 || isSprinting || isWalking || isPushing)
        {
            ResetIdleTimer();
        }
        else
        {
            UpdateIdleTimer();
        }

        if (isPushing)
        {
            moveDirection = transform.forward * inputManager.verticalInput;
        }
        else
        {
            moveDirection = cam.forward * inputManager.verticalInput;
            moveDirection = moveDirection + cam.right * inputManager.horizontalInput;
        }

        moveDirection.Normalize();
        moveDirection.y = 0;

        // Calcular o multiplicador de velocidade baseado na sa�de
        float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, 1f, health / 100f);

        if (isSprinting)
        {
            moveDirection = moveDirection * spritingSpeed * speedMultiplier;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed * speedMultiplier;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed * speedMultiplier;
            }
        }

        if (isWalking)
        {
            moveDirection = moveDirection * walkingSpeed * speedMultiplier;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed * speedMultiplier;
            }
        }

        if (isPushing)
        {
            moveDirection = moveDirection * pushingSpeed * speedMultiplier;
        }

        Vector3 movementVelocity = moveDirection;
        rb.velocity = movementVelocity;
    }

    private void ResetIdleTimer()
    {
        idleTimer = 0f;
        isIdle2 = false;
        idle2Triggered = false; // Reseta o estado idle2Triggered
    }

    private void UpdateIdleTimer()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleTimeThreshold && !isIdle2 && !idle2Triggered)
        {
            isIdle2 = true;
            idle2Triggered = true; // Define o estado idle2Triggered como verdadeiro
            animator.Play("Idle2");
        }
    }

    private void HandleRotation()
    {
        if (isJumping || isClimbing || isPushing)
            return;

        if (playerManager.isAiming)
        {
            targetRotation = Quaternion.Euler(0, cam.eulerAngles.y, 0);
            playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = playerRotation;
        }
        else
        {
            Vector3 targetDirection = Vector3.zero;
            targetDirection = cam.forward * inputManager.verticalInput;
            targetDirection = targetDirection + cam.right * inputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }
    }

    private void HandleFallingAndLanding()
    {
        // Verifica se est� escalando, se sim, n�o executa o c�digo de queda
        if (isClimbing)
            return;

        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCAstHeightOffSet;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting && !isClimbing)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            inAirTIme = inAirTIme + Time.deltaTime;
            rb.AddForce(transform.forward * learpingVelocity);
            rb.AddForce(-Vector3.up * fallingVelocity * inAirTIme);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, maxDistance, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting && !isClimbing)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }
            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTIme = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !isJumping && !isClimbing)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded && !isClimbing)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);
            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            rb.velocity = playerVelocity;
        }
    }

    public void HandleClimbing()
    {
        if (climbState == 4) // J� chegou ao topo
        {
            HandleReachTop();
            return;
        }

        rot = Mathf.Atan2(inputManager.movementInput.x, inputManager.movementInput.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
        Vector3 targetDirection = transform.forward;

        if (!isClimbing && PlayerPrefs.GetInt("Shire1Finishe") == 1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                float avoidFloorDistance = 0.4f;
                float ladderGrabDistance = 1f;

                if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDirection, out RaycastHit raycastHit, ladderGrabDistance))
                {
                    if (raycastHit.transform.TryGetComponent(out Ladders ladders))
                    {
                        HandleGrabLadders(targetDirection, ladders.topTransform);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                HandleDropLadders();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    HandleJumping();
                }
                animator.SetInteger("climbState", 0);
            }

            if (isClimbing)
            {
                if (Vector3.Distance(transform.position, ladderTop.position) < 0.5f)
                {
                    climbState = 4; // No topo da escada
                    animator.SetInteger("climbState", climbState);
                }
                else if (inputManager.verticalInput > 0)
                {
                    climbState = 3; // Subindo
                    animator.SetInteger("climbState", climbState);
                }
                else if (inputManager.verticalInput < 0)
                {
                    climbState = 2; // Descendo
                    animator.SetInteger("climbState", climbState);
                }
                else
                {
                    climbState = 1; // Idle na parede
                    animator.SetInteger("climbState", climbState);
                }

                Vector3 climbDirection = new Vector3(0f, inputManager.verticalInput * climbingSpeed, 0f);
                rb.velocity = climbDirection;
                isGrounded = true;
            }
        }
    }

    public void HandleGrabLadders(Vector3 lastGrabLadderDirection, Transform ladderTop)
    {
        rb.useGravity = false;
        isClimbing = true;
        this.lastGrabLadderDirection = lastGrabLadderDirection;
        this.ladderTop = ladderTop;
        climbState = 1; // Idle na parede
        animator.SetInteger("climbState", climbState);
    }

    public void HandleDropLadders()
    {
        isClimbing = false;
        animator.SetInteger("climbState", 0);
        rb.useGravity = true;
        ladderTop = null;
    }

    private void HandleReachTop()
    {
        // Ajusta a posi��o do jogador para o topo da escada, mas permite que continue escalando
        transform.position = Vector3.Lerp(transform.position, ladderTop.position + Vector3.up * 0.8f, Time.deltaTime * climbingSpeed);
        rb.velocity = new Vector3(rb.velocity.x, climbingSpeed, rb.velocity.z); // Continuar subindo

        // Checa se o jogador passou do topo da escada
        if (transform.position.y >= ladderTop.position.y + 0.7f)
        {
            isClimbing = false;
            animator.SetInteger("climbState", 0);
            rb.useGravity = true;
            climbState = 0; // Reseta o estado de escalada
        }
    }

    public void HandleDash()
    {
        if (isDashing || !canDash)
            return;

        // Calcular a posi��o alvo do dash
        Vector3 dashTarget = transform.position + transform.forward * dashDistance;

        // Verificar se h� algum obst�culo no caminho usando Raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, dashDistance, nonTeleportableLayer))
        {
            // Se houver um obst�culo, ajustar a posi��o do dash para antes do obst�culo
            dashTarget = hit.point - transform.forward * 0.5f; // Ajusta a posi��o para n�o colidir com o objeto
        }

        // Verificar se a posi��o de destino est� acima do terreno
        if (terrainCollider != null)
        {
            Vector3 terrainPoint;
            if (TerrainRaycast(dashTarget, out terrainPoint))
            {
                dashTarget = terrainPoint + Vector3.up * 0.5f; // Ajustar a posi��o para cima do terreno
            }
        }

        // Mover o jogador para a posi��o de destino ajustada
        rb.MovePosition(dashTarget);
        isDashing = true;
        canDash = false;
        StartCoroutine(ResetDash());
        Debug.Log("deuDash");
    }

    private bool TerrainRaycast(Vector3 targetPosition, out Vector3 hitPoint)
    {
        RaycastHit hit;
        if (Physics.Raycast(targetPosition + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            if (hit.collider == terrainCollider)
            {
                hitPoint = hit.point;
                return true;
            }
        }
        hitPoint = Vector3.zero;
        return false;
    }

    IEnumerator ResetDash()
    {
        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;
        Debug.Log("Saiu da espera");
        canDash = true; // Agora o jogador pode dar dash novamente
    }

    private void UpdateDashIndicator()
    {
        if (dashIndicator == null)
            return;

        if (Input.GetKey(KeyCode.V) && PlayerPrefs.GetInt("Shire2Finishe") == 1 && canDash)
        {
            Vector3 dashPosition = transform.position + transform.forward * dashDistance;
            dashPosition.y += 0.1f; // Deslocamento vertical para elevar o indicador
            dashIndicator.transform.position = dashPosition;
            dashIndicator.SetActive(true);
        }
        else
        {
            dashIndicator.SetActive(false);
        }
    }

    public void Update()
    {
        HandleClimbing();
        UpdateDashIndicator();
    }

    public void FixedUpdate()
    {
        HandleAllMovement();
    }

    public void SetIsPushing(bool isPushing)
    {
        this.isPushing = isPushing;

        // Ajusta a velocidade de corrida conforme necess�rio
        if (isPushing)
        {
            // Define uma velocidade de corrida mais baixa enquanto empurra
            runningSpeed = pushingSpeed;
        }
        else
        {
            // Retorna � velocidade de corrida original
            runningSpeed = originalRunningSpeed;
        }
    }

    public void AdjustHealth(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, 100f); // Garante que a sa�de esteja entre 0 e 100
    }
}