using UnityEngine;

public class CaveTutorialManager : MonoBehaviour
{
    public TooltipManager tooltip; // Referência ao Tooltip
    public HelpSystemAssistant helpSystem; // Referência ao HelpSystem
    public InputManager inputManager; // Referência ao InputManager
    public ObjectiveSystem objectiveSystem; // Referência ao ObjectiveSystem

    private bool messageOneShown = false; // Variável para controlar se a primeira mensagem foi mostrada
    private bool messageTwoShown = false; // Variável para controlar se a segunda mensagem foi mostrada
    private bool messageThreeShown = false; // Variável para controlar se a terceira mensagem foi mostrada
    private bool crystalMissionAdded = false; // Variável para controlar se a missão do cristal foi adicionada
    private bool tutorialCompleted = false; // Variável para controlar se o tutorial foi concluído

    void Start()
    {
        // Iniciar o tutorial com a primeira mensagem
        ShowFirstMessage();
        inputManager.ResetInputs(); // Desativa todos os inputs inicialmente
    }

    void ShowFirstMessage()
    {
        helpSystem.AddHelpMessage("Você chegou no coração da ilha. O cristal mãe está na sua frente, acabando com ele você acaba com a contaminação.");
    }

    void ShowSecondMessage()
    {
        helpSystem.AddHelpMessage("Para acabar com o cristal, leve até ele barris com explosivos e atire neles para explodirem. Use o mapa para achar eles.");
    }

    void ShowThirdMessage()
    {
        helpSystem.AddHelpMessage("Tome cuidado com as pedras com pernas e não esqueça de recarregar sua bateria. Boa sorte!");
    }

    void AddCrystalMission()
    {
        // Adicionar a missão do cristal mãe apenas uma vez
        if (!crystalMissionAdded)
        {
            objectiveSystem.SetMissionTitle("Cristal Mãe");
            objectiveSystem.AddObjective("Cristal Mãe", "Destrua o Cristal mãe.");

            // Mostrar a tooltip para segurar objetos
            tooltip.ShowTooltip("Aperte E para segurar um objeto");

            crystalMissionAdded = true; // Marca a missão do cristal como adicionada
            inputManager.EnableMovement(); // Ativar a movimentação após a adição da missão
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
            Debug.Log("Missão do Cristal mãe adicionada.");
        }

        // Verifica se todas as mensagens foram mostradas, a missão do cristal foi adicionada e o tutorial ainda não foi marcado como concluído
        if (messageOneShown && messageTwoShown && messageThreeShown && crystalMissionAdded && !tutorialCompleted)
        {
            Debug.Log("Condições para tutorial concluído atendidas.");
            // Não marca o tutorial como concluído aqui; esperamos pelo evento do boss ser derrotado
        }
    }

    public void OnBossDefeated()
    {
        // Verifica se todas as mensagens foram mostradas, a missão do cristal foi adicionada e o tutorial ainda não foi marcado como concluído
        if (messageOneShown && messageTwoShown && messageThreeShown && crystalMissionAdded && !tutorialCompleted)
        {
            tutorialCompleted = true;
            Debug.Log("Tutorial concluído!");

            // Marca o tutorial como concluído no ObjectiveSystem
            objectiveSystem.CompleteObjective("Destrua o Cristal mãe");
            Debug.Log("Objetivo 'Destrua o Cristal mãe' completo.");

            // Mostra a mensagem de conclusão no HelpSystem
            helpSystem.AddHelpMessage("Você derrotou o cristal mãe, parabéns! Use o portal para voltar à ilha.");
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
        return tutorialCompleted; // Retorna verdadeiro se o tutorial foi concluído
    }
}
