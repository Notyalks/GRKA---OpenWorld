using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    [Header("Configura��es da Cena")]
    public string NomeDaCena;

    [Header("Pain�is do Menu de Pause")]
    public GameObject PainelMenuPause;
    public GameObject PainelControles;
    public GameObject PainelConfig;

    private InputManager inputManager;
    private PlayerManager playerManager;
    private NpcCompleto npcCompleto;

    [Header("Configura��es do Mapa")]
    public Camera minimapCamera;
    public GameObject minimap;
    public Transform player;
    public Camera mainCamera;
    public GameObject mapPanel;
    public Camera mapCamera;
    public RawImage mapRawImage; // Adicionado

    [Header("Configura��es de Zoom")]
    public float zoomSpeed = 10f;
    public float minZoom = 20f;
    public float maxZoom = 100f;

    [Header("Configura��es de Arrastar Mapa")]
    private Vector3 dragOrigin;
    private bool isDragging = false;

    [Header("Limites do Mapa")]
    public float mapLimitLeft = -50f;
    public float mapLimitRight = 50f;
    public float mapLimitTop = 50f;
    public float mapLimitBottom = -50f;

    [Header("Prefab do Marcador")]
    public GameObject waypointPrefab;

    private GameObject currentWaypoint;

    private RectTransform mapRectTransform;

    Resolution[] resolutions;

    // Posi��o e zoom iniciais da c�mera do mapa
    private Vector3 initialMapCameraPosition;
    private float initialMapCameraZoom;

    // Adicionando as vari�veis screenModeSlider e resolutionDropdown
    public Dropdown resolutionDropdown;
    public Slider screenModeSlider;

    void Start()
    {
        PainelMenuPause.SetActive(false);
        inputManager = FindObjectOfType<InputManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        npcCompleto = FindObjectOfType<NpcCompleto>();

        // Inicializa o mapa desativado
        mapPanel.SetActive(false);

        // Armazena a posi��o inicial e o zoom da c�mera do mapa
        initialMapCameraPosition = mapCamera.transform.position;
        initialMapCameraZoom = mapCamera.orthographicSize;

        mapRectTransform = mapRawImage.rectTransform; // Corrigido

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        screenModeSlider.minValue = 0;
        screenModeSlider.maxValue = 1;
        screenModeSlider.wholeNumbers = true;
        screenModeSlider.value = Screen.fullScreen ? 0 : 1; // Inicializa o slider com base no modo de tela atual
        screenModeSlider.onValueChanged.AddListener(delegate { SetFullScreen((int)screenModeSlider.value); });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mapPanel.activeSelf)
            {
                ToggleMap(); // Fecha o mapa se estiver aberto
            }
            else if (PainelMenuPause.activeSelf)
            {
                PausarJogo();
            }
            else if (PainelControles.activeSelf)
            {
                FecharControles();
            }
            else if (PainelConfig.activeSelf)
            {
                FecharConfig();
            }
            else
            {
                PausarJogo();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }

        if (mapPanel.activeSelf)
        {
            HandleZoom(); // Detecta o zoom somente quando o mapa est� ativo
            HandleMapDrag(); // Detecta arrasto do mapa quando o mapa est� ativo
            HandleWaypointPlacement(); // Detecta cliques do bot�o direito para colocar waypoints
        }

        UpdateMinimapCameraPosition();
    }

    private void PausarJogo()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            PainelMenuPause.SetActive(true);
            if (inputManager != null)
            {
                inputManager.ResetInputs();
                inputManager.enabled = false;
            }
            if (playerManager != null)
            {
                playerManager.enabled = false;
            }
            if (npcCompleto == null || !npcCompleto.EstaEmDialogo())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            // Fecha o mapa ao pausar o jogo
            if (mapPanel.activeSelf)
            {
                mapPanel.SetActive(false);
            }
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            PainelMenuPause.SetActive(false);
            PainelControles.SetActive(false);
            PainelConfig.SetActive(false);
            if (inputManager != null && (npcCompleto == null || !npcCompleto.EstaEmDialogo()))
            {
                inputManager.ResetInputs();
                inputManager.enabled = true;
            }
            if (playerManager != null)
            {
                playerManager.enabled = true;
            }
            if (npcCompleto == null || !npcCompleto.EstaEmDialogo())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void Continuar()
    {
        PausarJogo();
    }

    public void AbrirControles()
    {
        PainelMenuPause.SetActive(false);
        PainelControles.SetActive(true);
    }

    public void FecharControles()
    {
        PainelMenuPause.SetActive(true);
        PainelControles.SetActive(false);
    }

    public void AbrirConfig()
    {
        PainelMenuPause.SetActive(false);
        PainelConfig.SetActive(true);
    }

    public void SetFullScreen(int value)
    {
        Screen.fullScreen = (value == 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void FecharConfig()
    {
        PainelMenuPause.SetActive(true);
        PainelConfig.SetActive(false);
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(NomeDaCena);
    }
    // M�todos do MapToggle
    void ToggleMap()
    {
        if (Time.timeScale == 0 && !mapPanel.activeSelf)
        {
            // N�o permite abrir o mapa enquanto o jogo est� pausado
            return;
        }

        bool isMapActive = mapPanel.activeSelf;
        mapPanel.SetActive(!isMapActive);
        if (!isMapActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0; // Pausa o jogo ao abrir o mapa
            if (inputManager != null)
            {
                inputManager.ResetInputs();
                inputManager.enabled = false;
// Desativa o InputManager
            }
        }
        else
        {
            // Redefine a posi��o e o zoom da c�mera do mapa
            mapCamera.transform.position = initialMapCameraPosition;
            mapCamera.orthographicSize = initialMapCameraZoom;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1; // Retoma o jogo ao fechar o mapa
            if (inputManager != null)
            {
                inputManager.enabled = true; // Ativa o InputManager
            }
        }
    }

    void UpdateMinimapCameraPosition()
    {
        if (player != null && minimapCamera != null)
        {
            Vector3 newPosition = player.position;
            newPosition.y = minimapCamera.transform.position.y;
            minimapCamera.transform.position = newPosition;

            // Alinha a rota��o da c�mera do minimapa com a rota��o da c�mera principal
            Vector3 newRotation = mainCamera.transform.eulerAngles;
            minimapCamera.transform.eulerAngles = new Vector3(90, newRotation.y, 0);
        }
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            float newZoom = mapCamera.orthographicSize - scrollInput * zoomSpeed;
            mapCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }

    void HandleMapDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsMouseOverMap())
                return;

            dragOrigin = mapCamera.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = dragOrigin - mapCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = mapCamera.transform.position + difference;

            // Limita a posi��o da c�mera dentro dos limites definidos a partir da posi��o inicial
            newPosition.x = Mathf.Clamp(newPosition.x, initialMapCameraPosition.x + mapLimitLeft, initialMapCameraPosition.x + mapLimitRight);
            newPosition.z = Mathf.Clamp(newPosition.z, initialMapCameraPosition.z + mapLimitBottom, initialMapCameraPosition.z + mapLimitTop);

            mapCamera.transform.position = newPosition;
        }
    }

    void HandleWaypointPlacement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!IsMouseOverMap())
                return;

            Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Se j� existe um marcador, remova-o
                if (currentWaypoint != null)
                {
                    Destroy(currentWaypoint);
                }

                // Instancia o marcador na posi��o do clique
                currentWaypoint = Instantiate(waypointPrefab, hit.point, Quaternion.Euler(90, 0, 0));
            }
        }
    }

    bool IsMouseOverMap()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRectTransform, Input.mousePosition, null, out localMousePosition);
        return mapRectTransform.rect.Contains(localMousePosition);
    }
}