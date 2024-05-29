using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    [Header("Configurações da Cena")]
    public string NomeDaCena;

    [Header("Painéis do Menu de Pause")]
    public GameObject PainelMenuPause;
    public GameObject PainelControles;
    public GameObject PainelConfig;

    private InputManager inputManager;
    private PlayerManager playerManager;
    private NpcCompleto npcCompleto;

    [Header("Configurações do Mapa")]
    public Camera minimapCamera;
    public GameObject minimap;
    public Transform player;
    public Camera mainCamera;
    public GameObject mapPanel;
    public Camera mapCamera;

    [Header("Configurações de Zoom")]
    public float zoomSpeed = 10f;
    public float minZoom = 20f;
    public float maxZoom = 100f;

    [Header("Configurações de Arrastar Mapa")]
    private Vector3 dragOrigin;
    private bool isDragging = false;

    [Header("Limites do Mapa")]
    public float mapLimitLeft = -50f;
    public float mapLimitRight = 50f;
    public float mapLimitTop = 50f;
    public float mapLimitBottom = -50f;

    // Posição e zoom iniciais da câmera do mapa
    private Vector3 initialMapCameraPosition;
    private float initialMapCameraZoom;

    void Start()
    {
        PainelMenuPause.SetActive(false);
        inputManager = FindObjectOfType<InputManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        npcCompleto = FindObjectOfType<NpcCompleto>();

        // Inicializa o mapa desativado
        mapPanel.SetActive(false);

        // Armazena a posição inicial e o zoom da câmera do mapa
        initialMapCameraPosition = mapCamera.transform.position;
        initialMapCameraZoom = mapCamera.orthographicSize;
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
            HandleZoom(); // Detecta o zoom somente quando o mapa está ativo
            HandleMapDrag(); // Detecta arrasto do mapa quando o mapa está ativo
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

    // Métodos do MapToggle
    void ToggleMap()
    {
        if (Time.timeScale == 0 && !mapPanel.activeSelf)
        {
            // Não permite abrir o mapa enquanto o jogo está pausado
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
                inputManager.enabled = false; // Desativa o InputManager
            }
        }
        else
        {
            // Redefine a posição e o zoom da câmera do mapa
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

            // Alinha a rotação da câmera do minimapa com a rotação da câmera principal
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

            // Limita a posição da câmera dentro dos limites definidos a partir da posição inicial
            newPosition.x = Mathf.Clamp(newPosition.x, initialMapCameraPosition.x + mapLimitLeft, initialMapCameraPosition.x + mapLimitRight);
            newPosition.z = Mathf.Clamp(newPosition.z, initialMapCameraPosition.z + mapLimitBottom, initialMapCameraPosition.z + mapLimitTop);

            mapCamera.transform.position = newPosition;
        }
    }
}
