using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpSystemAssistant : MonoBehaviour
{
    public static HelpSystemAssistant Instance { get; private set; }

    public GameObject helpPanel; // Painel onde as mensagens serão exibidas
    public Text helpText; // Texto para exibir as mensagens

    private bool isHelpMessageActive = false;
    private GameObject player;
    private InputManager inputManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Mensagem inicial de boas-vindas no tutorial
        ShowHelpMessage("Resfriando sistema... tudo bem GARiK?");
    }

    void Update()
    {
        if (isHelpMessageActive && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            HideHelpMessage();
        }
    }

    public void ShowHelpMessage(string message)
    {
        helpText.text = message;
        helpPanel.SetActive(true);
        isHelpMessageActive = true;

        // Desativar o InputManager do player
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null)
        {
            inputManager = player.GetComponent<InputManager>();
            if (inputManager != null)
            {
                inputManager.ResetInputs(); // Zerar valores do InputManager
                inputManager.enabled = false;
            }
        }
    }

    public void HideHelpMessage()
    {
        helpPanel.SetActive(false);
        isHelpMessageActive = false;

        // Reativar o InputManager
        if (inputManager != null)
        {
            inputManager.enabled = true;
            Debug.Log("InputManager reativado!");
        }
    }
}
