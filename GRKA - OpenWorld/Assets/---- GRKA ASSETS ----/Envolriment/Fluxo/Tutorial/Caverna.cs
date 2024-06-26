using UnityEngine;

public class CaveTutorialManager : MonoBehaviour
{
    public TooltipManager tooltip; // Refer�ncia ao Tooltip
    public HelpSystemAssistant helpSystem; // Refer�ncia ao HelpSystem
    public InputManager inputManager; // Refer�ncia ao InputManager
    public ObjectiveSystem objectiveSystem; // Refer�ncia ao ObjectiveSystem

    private bool messageOneShown = false; // Vari�vel para controlar se a primeira mensagem foi mostrada
    private bool messageTwoShown = false; // Vari�vel para controlar se a segunda mensagem foi mostrada
    private bool messageThreeShown = false; // Vari�vel para controlar se a terceira mensagem foi mostrada
    private bool crystalMissionAdded = false; // Vari�vel para controlar se a miss�o do cristal foi adicionada
    private bool tutorialCompleted = false; // Vari�vel para controlar se o tutorial foi conclu�do

    void Start()
    {
        // Iniciar o tutorial com a primeira mensagem
        ShowFirstMessage();
        inputManager.ResetInputs(); // Desativa todos os inputs inicialmente
    }

    void ShowFirstMessage()
    {
        helpSystem.AddHelpMessage("Voc� chegou no cora��o da ilha. O cristal m�e est� na sua frente, acabando com ele voc� acaba com a contamina��o.");
    }

    void ShowSecondMessage()
    {
        helpSystem.AddHelpMessage("Para acabar com o cristal, leve at� ele barris com explosivos e atire neles para explodirem. Use o mapa para achar eles.");
    }

    void ShowThirdMessage()
    {
        helpSystem.AddHelpMessage("Tome cuidado com as pedras com pernas e n�o esque�a de recarregar sua bateria. Boa sorte!");
    }

    void AddCrystalMission()
    {
        // Adicionar a miss�o do cristal m�e apenas uma vez
        if (!crystalMissionAdded)
        {
            objectiveSystem.SetMissionTitle("Cristal M�e");
            objectiveSystem.AddObjective("Cristal M�e", "Destrua o Cristal m�e.");

            // Mostrar a tooltip para segurar objetos
            tooltip.ShowTooltip("Aperte E para segurar um objeto");

            crystalMissionAdded = true; // Marca a miss�o do cristal como adicionada
            inputManager.EnableMovement(); // Ativar a movimenta��o ap�s a adi��o da miss�o
        }
    }

    public void OnHelpMessageClosed()
    {
        if (helpSystem.isDisplayingMessage)
            return;

        if (!messageOneShown)
        {
            messageOneShown = true; // Marca a primeira mensagem como mostrada
            ShowSecondMessage();
            Debug.Log("Segunda mensagem mostrada.");
        }
        else if (!messageTwoShown)
        {
            messageTwoShown = true; // Marca a segunda mensagem como mostrada
            ShowThirdMessage();
            Debug.Log("Terceira mensagem mostrada.");
        }
        else if (!messageThreeShown)
        {
            messageThreeShown = true; // Marca a terceira mensagem como mostrada
            AddCrystalMission();
            Debug.Log("Miss�o do Cristal m�e adicionada.");
        }

        // Verifica se todas as mensagens foram mostradas, a miss�o do cristal foi adicionada e o tutorial ainda n�o foi marcado como conclu�do
        if (messageOneShown && messageTwoShown && messageThreeShown && crystalMissionAdded && !tutorialCompleted)
        {
            Debug.Log("Condi��es para tutorial conclu�do atendidas.");
            // N�o marca o tutorial como conclu�do aqui; esperamos pelo evento do boss ser derrotado
        }
    }

    public void OnBossDefeated()
    {
        // Verifica se todas as mensagens foram mostradas, a miss�o do cristal foi adicionada e o tutorial ainda n�o foi marcado como conclu�do
        if (messageOneShown && messageTwoShown && messageThreeShown && crystalMissionAdded && !tutorialCompleted)
        {
            tutorialCompleted = true;
            Debug.Log("Tutorial conclu�do!");

            // Marca o tutorial como conclu�do no ObjectiveSystem
            objectiveSystem.CompleteObjective("Destrua o Cristal m�e");
            Debug.Log("Objetivo 'Destrua o Cristal m�e' completo.");

            // Mostra a mensagem de conclus�o no HelpSystem
            helpSystem.AddHelpMessage("Voc� derrotou o cristal m�e, parab�ns! Use o portal para voltar � ilha.");
        }
    }

    void Update()
    {
        // Verifica se uma mensagem foi fechada e chama OnHelpMessageClosed
        if (Input.GetMouseButtonDown(0) && !helpSystem.isDisplayingMessage)
        {
            OnHelpMessageClosed();
        }
    }

    public bool IsTutorialCompleted()
    {
        return tutorialCompleted; // Retorna verdadeiro se o tutorial foi conclu�do
    }
}
