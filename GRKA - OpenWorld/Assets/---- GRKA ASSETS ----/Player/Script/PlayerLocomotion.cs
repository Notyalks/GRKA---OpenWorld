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
    public bool isSpriting;
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

    [Header("Jumps")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;
    public float jumpSpeed;

    [Header("Movement Variables")]
    Quaternion targetRotation;
    Quaternion playerRotation;
    public float rot;

    [Header("Player Health")]
    public float health = 100f; // Vida inicial do jogador
    public float minSpeedMultiplier = 0.5f; // Multiplicador mínimo de velocidade quando a saúde está em 0%
    public float originalWalkingSpeed;

    // Referência ao topo da escada
    private Transform ladderTop;

    // Velocidade de movimento original antes de empurrar
    private float originalRunningSpeed;

    [Header("Dash")]
    public LayerMask nonTeleportableLayer; // LayerMask para objetos que o dash não deve atravessar
    public float dashCooldown = 3f; // Tempo de recarga do dash em segundos
    public float dashDistance = 10f; // Distância máxima do dash
    public TerrainCollider terrainCollider; // Referência ao TerrainCollider
    private bool canDash = true; // Variável para controlar se o jogador pode ou não dar o dash

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

        // Calcular o multiplicador de velocidade baseado na saúde
        float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, 1f, health / 100f);

        if (isSpriting)
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
        // Verifica se está escalando, se sim, não executa o código de queda
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
        if (isGrounded)
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
            float avoidFloorDistance = 0.4f;
            float ladderGrabDistance = 1f;

            if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDirection, out RaycastHit raycastHit, ladderGrabDistance))
            {
                if (raycastHit.transform.TryGetComponent(out Ladders ladders))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        HandleDropLadders();
                        animator.SetBool("isClimbing", false);
                    }
                    else if (Input.GetKeyDown(KeyCode.Space))
                    {
                        HandleDropLadders();
                        HandleJumping();
                        animator.SetBool("isClimbing", false);
                    }
                }
                else
                {
                    HandleDropLadders();
                    animator.SetBool("isClimbing", false);
                }
            }
            else
            {
                HandleDropLadders();
                animator.SetBool("isClimbing", false);
            }

            if (isClimbing)
            {
                // Verificar se está perto do topo
                if (ladderTop != null && Vector3.Distance(transform.position, ladderTop.position) < 0.5f)
                {
                    // Parar de escalar e subir no topo
                    HandleReachTop();
                }
                else
                {
                    targetDirection.x = 0f;
                    targetDirection.y = inputManager.verticalInput * climbingSpeed;
                    targetDirection.z = 0f;
                    rb.velocity = targetDirection;
                    isGrounded = true;
                    animator.SetBool("isClimbing", true);
                }
            }
        }
    }

        public void HandleGrabLadders(Vector3 lastGrabLadderDirection, Transform ladderTop)
        {
            rb.useGravity = false;
            isClimbing = true;
            this.lastGrabLadderDirection = lastGrabLadderDirection;
            this.ladderTop = ladderTop;
        }

        public void HandleDropLadders()
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.useGravity = true;
            ladderTop = null;
        }

        private void HandleReachTop()
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.useGravity = true;

            // Ajusta a posição do jogador para o topo da escada
            transform.position = ladderTop.position;
            rb.velocity = Vector3.zero; // Para o movimento
        }

        public void HandleDash()
        {
            if (isDashing || !canDash)
                return;

            // Calcular a posição alvo do dash
            Vector3 dashTarget = transform.position + transform.forward * dashDistance;

            // Verificar se há algum obstáculo no caminho usando Raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, dashDistance, nonTeleportableLayer))
            {
                // Se houver um obstáculo, ajustar a posição do dash para antes do obstáculo
                dashTarget = hit.point - transform.forward * 0.5f; // Ajusta a posição para não colidir com o objeto
            }

            // Verificar se a posição de destino está acima do terreno
            if (terrainCollider != null)
            {
                Vector3 terrainPoint;
                if (TerrainRaycast(dashTarget, out terrainPoint))
                {
                    dashTarget = terrainPoint + Vector3.up * 0.5f; // Ajustar a posição para cima do terreno
                }
            }

            // Mover o jogador para a posição de destino ajustada
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

            // Ajusta a velocidade de corrida conforme necessário
            if (isPushing)
            {
                // Define uma velocidade de corrida mais baixa enquanto empurra
                runningSpeed = pushingSpeed;
            }
            else
            {
                // Retorna à velocidade de corrida original
                runningSpeed = originalRunningSpeed;
            }
        }

        public void AdjustHealth(float amount)
        {
            health += amount;
            health = Mathf.Clamp(health, 0f, 100f); // Garante que a saúde esteja entre 0 e 100
        }
    }

