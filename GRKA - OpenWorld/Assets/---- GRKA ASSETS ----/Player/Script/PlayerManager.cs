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

    [Header("Status Bars")]
    public HealthBar healthBar;

    [Header("Health Settings")]
    public float vida = 100f;
    public float vidaMaxima = 100f;

    [Header("Health Regeneration Settings")]
    [Tooltip("Intervalo de tempo para regenera��o (segundos)")]
    public float regenerationInterval = 1f; // Intervalo de tempo para regenera��o (segundos)
    [Tooltip("Quantidade de vida regenerada por intervalo")]
    public float regenerationAmount = 1f; // Quantidade de vida regenerada por intervalo
    [Tooltip("Controle de ativa��o da regenera��o")]
    [SerializeField]
    private bool enableHealthRegeneration = true; // Controle de ativa��o da regenera��o

    private Coroutine regenerationCoroutine;

    [Header("Health Decay Settings")]
    [Tooltip("Intervalo de tempo para deteriora��o (segundos)")]
    public float lifeDecayInterval = 5f; // Intervalo de tempo para deteriora��o (segundos)
    [Tooltip("Quantidade de vida deca�da por intervalo")]
    public float lifeDecayAmount = 2f; // Quantidade de vida deca�da por intervalo

    private Coroutine decayCoroutine;

    [Header("Shield Settings")]
    public float shieldVida = 50f;
    public float shieldVidaMaxima = 50f;
    public GameObject Shield;
    public bool isShieldActive = false;
    public float shieldRechargeTime = 5f;
    private bool isRechargingShield = false; // Flag to control shield recharge
    private bool _shieldOn = false; // Track if the shield is currently on or off

    [SerializeField] float _DisolveSpeed;

    Renderer _shieldRenderer;
    Camera _cam;
    Coroutine _disolveCoroutine;

    [Header("Damage Settings")]
    public float launchUpForce = 10f;
    public float launchBackForce = 5f;

    [Header("Player Flags")]
    public bool isInteracting;
    public bool isAiming;
    public bool isDead = false;
    public bool Dead = false;
    public bool canMoveCam = true;

    [Header("Speed Settings")]
    public float maxSpeed = 10f;
    public float minSpeedMultiplier = 0.5f;
    [Tooltip("Percentual m�nimo de vida para redu��o de velocidade")]
    public float minHealthForSpeedReduction = 0.2f; // Percentual m�nimo de vida para redu��o de velocidade

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        rb = GetComponent<Rigidbody>();
        _shieldRenderer = Shield.GetComponent<Renderer>();
        _cam = Camera.main;
    }

    private void Start()
    {
        vida = vidaMaxima;
        healthBar.ColocarVidaMaxima(vidaMaxima);
        healthBar.AlterarVida(vida);
        Cursor.lockState = CursorLockMode.Locked;

        // Desativar a barra do escudo no in�cio
        healthBar.ShowShieldBar(false);

        // Ativar o escudo no in�cio se a Shire3 estiver completa
        if (PlayerPrefs.GetInt("Shire3Finishe", 0) == 1)
        {
            ShieldOn();
        }
        else
        {
            Shield.SetActive(false);
            isShieldActive = false;
        }

        // Iniciar regenera��o de vida se Shire4 estiver completa e a regenera��o estiver habilitada
        if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 1 && enableHealthRegeneration)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealth());
        }

        // Configurar a barra de vida do escudo
        healthBar.SetMaxShield(shieldVidaMaxima);
        healthBar.SetShield(shieldVida);

        // Iniciar a deteriora��o da vida se Shire4 n�o estiver completa
        if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 0)
        {
            decayCoroutine = StartCoroutine(DecayLife());
        }
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        isAiming = animator.GetBool("isAiming");

        if (vida <= 0 && !isDead)
        {
            Debug.Log("N�o morreu");
            rb.constraints = RigidbodyConstraints.FreezePosition;
            inputManager.OnDisable();
            isDead = true;
            animator.SetBool("isDead", true);

            // Parar regenera��o de vida se o jogador estiver morto
            if (regenerationCoroutine != null)
            {
                StopCoroutine(regenerationCoroutine);
            }

            // Parar deteriora��o de vida se o jogador estiver morto
            if (decayCoroutine != null)
            {
                StopCoroutine(decayCoroutine);
            }
        }

        // Verificar se Shire3 foi completada e ativar ou desativar o escudo conforme necess�rio
        if (PlayerPrefs.GetInt("Shire3Finishe", 0) == 1 && !isShieldActive && !isRechargingShield)
        {
            ShieldOn();
        }
        else if (PlayerPrefs.GetInt("Shire3Finishe", 0) == 0 && isShieldActive)
        {
            // Desativar o escudo sem iniciar a recarga
            Shield.SetActive(false);
            isShieldActive = false;
            healthBar.ShowShieldBar(false); // Desativar a barra do escudo
            Debug.Log("Escudo desativado devido � Shire3 n�o estar completa.");
        }

        // Verificar se Shire4 foi completada e iniciar ou parar a regenera��o de vida conforme necess�rio
        if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 1 && enableHealthRegeneration && regenerationCoroutine == null)
        {
            regenerationCoroutine = StartCoroutine(RegenerateHealth());
        }
        else if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 0 && regenerationCoroutine != null)
        {
            StopCoroutine(regenerationCoroutine);
            regenerationCoroutine = null;
            Debug.Log("Regenera��o de vida desativada devido � Shire4 n�o estar completa.");
        }

        // Verificar se Shire4 foi completada e iniciar ou parar a deteriora��o de vida conforme necess�rio
        if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 1 && decayCoroutine != null)
        {
            StopCoroutine(decayCoroutine);
            decayCoroutine = null;
            Debug.Log("Deteriora��o de vida desativada devido � Shire4 estar completa.");
        }
        else if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 0 && decayCoroutine == null)
        {
            decayCoroutine = StartCoroutine(DecayLife());
        }

        // Atualizar a posi��o do escudo na tela e fazer o escudo olhar para a c�mera
        Vector3 screenPoint = _cam.WorldToScreenPoint(Shield.transform.position);
        screenPoint.x /= Screen.width;
        screenPoint.y /= Screen.height;
        _shieldRenderer.material.SetVector("_ObjScreenPos", screenPoint);
        Shield.transform.forward = _cam.transform.position - Shield.transform.position;

        // Adjust player speed based on health
        float healthPercentage = vida / vidaMaxima;
        if (healthPercentage < minHealthForSpeedReduction)
        {
            healthPercentage = minHealthForSpeedReduction;
        }
        float speedMultiplier = Mathf.Lerp(minSpeedMultiplier, 1f, healthPercentage);
        playerLocomotion.runningSpeed = maxSpeed * speedMultiplier;
        playerLocomotion.walkingSpeed = playerLocomotion.originalWalkingSpeed * speedMultiplier;
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
        if (!isShieldActive && !isRechargingShield)
        {
            Shield.SetActive(true);
            isShieldActive = true;
            _shieldOn = true;
            shieldVida = shieldVidaMaxima;
            healthBar.SetShield(shieldVida);
            healthBar.ShowShieldBar(true); // Ativar a barra do escudo
            Debug.Log("Escudo ativado. Vida do escudo: " + shieldVida);

            // Iniciar a dissolu��o do escudo
            if (_disolveCoroutine != null)
            {
                StopCoroutine(_disolveCoroutine);
            }
            _disolveCoroutine = StartCoroutine(Coroutine_DisolveShield(0));
        }
    }

    public void ShieldOff()
    {
        if (isShieldActive)
        {
            _shieldOn = false;
            // Iniciar a dissolu��o do escudo
            if (_disolveCoroutine != null)
            {
                StopCoroutine(_disolveCoroutine);
            }
            _disolveCoroutine = StartCoroutine(Coroutine_DisolveShield(1, () =>
            {
                Shield.SetActive(false);
                isShieldActive = false;
                Debug.Log("Escudo desativado.");
                StartCoroutine(RechargeShieldAfterDelay(shieldRechargeTime));
            }));
        }
    }

    public void OpenCloseShield()
    {
        float target = _shieldOn ? 1 : 0;
        _shieldOn = !_shieldOn;

        // Iniciar a dissolu��o do escudo
        if (_disolveCoroutine != null)
        {
            StopCoroutine(_disolveCoroutine);
        }
        _disolveCoroutine = StartCoroutine(Coroutine_DisolveShield(target, () =>
        {
            // Ativar/desativar o escudo visualmente
            Shield.SetActive(_shieldOn);
            isShieldActive = _shieldOn;
        }));
    }

    private IEnumerator Coroutine_DisolveShield(float target, System.Action onComplete = null)
    {
        float start = _shieldRenderer.material.GetFloat("_Disolve");
        float lerp = 0;
        while (lerp < 1)
        {
            _shieldRenderer.material.SetFloat("_Disolve", Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * _DisolveSpeed;
            yield return null;
        }
        _shieldRenderer.material.SetFloat("_Disolve", target); // Garantir que o valor final seja definido
        onComplete?.Invoke(); // Chamar a a��o de conclus�o, se fornecida
    }

    private IEnumerator RechargeShieldAfterDelay(float delay)
    {
        Debug.Log("Recarregando escudo em " + delay + " segundos.");
        isRechargingShield = true;
        yield return new WaitForSeconds(delay);
        isRechargingShield = false;
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
            healthBar.SetShield(shieldVida);
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

    private IEnumerator DecayLife()
    {
        while (true)
        {
            yield return new WaitForSeconds(lifeDecayInterval);
            if (PlayerPrefs.GetInt("Shire4Finishe", 0) == 1)
            {
                Debug.Log("Deteriora��o de vida parada devido � Shire4 estar completa.");
                yield break;
            }
            if (vida > 0 && !isDead)
            {
                vida -= lifeDecayAmount;
                if (vida < 0)
                {
                    vida = 0;
                }
                healthBar.AlterarVida(vida);
                Debug.Log("Vida deca�da: " + vida);
            }
        }
    }
}
