using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPause : MonoBehaviour
{
    public string NomeDaCena;
    public GameObject PainelMenuPause;
    public GameObject PainelControles;
    public GameObject PainelConfig;
    private InputManager inputManager;
    private PlayerManager playerManager;
    private NpcCompleto npcCompleto;

    void Start()
    {
        PainelMenuPause.SetActive(false);
        inputManager = FindObjectOfType<InputManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        npcCompleto = FindObjectOfType<NpcCompleto>();

}

void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PainelMenuPause.activeSelf)
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
}
