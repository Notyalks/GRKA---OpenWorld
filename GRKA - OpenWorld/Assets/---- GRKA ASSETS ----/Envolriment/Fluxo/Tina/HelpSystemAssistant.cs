using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HelpSystemAssistant : MonoBehaviour
{
    public static HelpSystemAssistant Instance { get; private set; }

    public GameObject helpPanel; // Painel onde as mensagens serão exibidas
    public Text helpText; // Texto para exibir as mensagens

    private Queue<string> messageQueue = new Queue<string>();
    public bool isDisplayingMessage = false;

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

    void Update()
    {
        if (isDisplayingMessage && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            HideHelpMessage();
        }
    }

    public void AddHelpMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isDisplayingMessage)
        {
            DisplayNextMessage();
        }
    }

    private void DisplayNextMessage()
    {
        if (messageQueue.Count > 0)
        {
            string nextMessage = messageQueue.Dequeue();
            ShowHelpMessage(nextMessage);
        }
    }

    public void ShowHelpMessage(string message)
    {
        helpText.text = message;
        helpPanel.SetActive(true);
        isDisplayingMessage = true;

        // Desativar o InputManager
        InputManager inputManager = FindObjectOfType<InputManager>();
        if (inputManager != null)
        {
            inputManager.ResetInputs();
            inputManager.enabled = false;
        }
    }

    public void HideHelpMessage()
    {
        helpPanel.SetActive(false);
        isDisplayingMessage = false;

        // Reativar o InputManager
        InputManager inputManager = FindObjectOfType<InputManager>();
        if (inputManager != null)
        {
            inputManager.enabled = true;
        }

        // Exibir a próxima mensagem, se houver
        DisplayNextMessage();

        // Notificar o TutorialManager
        FindObjectOfType<TutorialManager>()?.OnHelpMessageClosed();
    }

    public void ShowRadioactiveMineralsMessage()
    {
        string message = "Atenção!!! Há minérios extremamente radioativos na frente. Atire para destruí-los.";
        AddHelpMessage(message);
    }
}
