using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TooltipManager tooltip; // Referência ao Tooltip
    public HelpSystemAssistant helpSystem; // Referência ao HelpSystem
    public InputManager inputManager; // Referência ao InputManager
    public ObjectiveSystem objectiveSystem; // Referência ao ObjectiveSystem
    public GameObject farmIcon; // Referência ao ícone da fazenda no mapa

    private bool cameraMovementCompleted = false;
    private bool showedCameraMovementTooltip = false;
    private bool triggerActivated = false; // Variável para controlar se o trigger já foi ativado
    private bool bridgeMessageShown = false; // Variável para controlar se a mensagem dos minérios já foi mostrada
    private bool batteryMissionCompleted = false; // Variável para controlar se a missão da bateria foi concluída
    private bool batteryMessageShown = false; // Variável para controlar se a mensagem de conclusão da missão foi mostrada
    private bool farmMessageShown = false; // Variável para controlar se a mensagem da fazenda foi mostrada

    private bool isGamePaused = false; // Variável para controlar o estado do jogo

    void Start()
    {
        // Iniciar o tutorial com a mensagem de boas-vindas
        helpSystem.AddHelpMessage("Resfriando sistema... tudo bem GARiK?");
        inputManager.ResetInputs(); // Desativa todos os inputs inicialmente

        // Certifique-se de que o ícone da fazenda esteja desativado no início
        if (farmIcon != null)
        {
            farmIcon.SetActive(false);
        }
    }

    // Método chamado quando o jogador entra no trigger
    private void OnTriggerEnter(Collider other)
    {
        if (isGamePaused) return; // Não fazer nada se o jogo estiver pausado

        if (other.CompareTag("Player") && !triggerActivated) // Verifica se o trigger não foi ativado ainda
        {
            helpSystem.AddHelpMessage("Atenção!!! Há minérios extremamente radioativos na frente. Atire para destruí-los.");
            tooltip.ShowTooltip("Use mouse 1 para atirar");
            inputManager.ResetInputs(); // Reseta os inputs para permitir que o jogador feche a mensagem manualmente com um clique do mouse
            triggerActivated = true; // Marca o trigger como ativado
            bridgeMessageShown = true; // Marca a mensagem dos minérios como mostrada
        }
    }

    void Update()
    {
        if (isGamePaused) return; // Não fazer nada se o jogo estiver pausado

        // Verificar se o jogador moveu a câmera
        if (!cameraMovementCompleted && showedCameraMovementTooltip)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cameraMovementCompleted = true;
                helpSystem.AddHelpMessage("Não esqueça de sua bateria GARiK. Vá para a floresta recarregá-la.");
                inputManager.DisableCameraMovement(); // Desativa a movimentação da câmera após concluído
            }
        }
    }

    public void OnHelpMessageClosed()
    {
        if (isGamePaused || helpSystem.isDisplayingMessage)
            return;

        if (bridgeMessageShown)
        {
            bridgeMessageShown = false; // Reseta a flag de mensagem dos minérios
            inputManager.EnableMovement(); // Ativa a movimentação do jogador
            return; // Retorna sem executar mais ações
        }

        if (batteryMessageShown)
        {
            batteryMissionCompleted = false;
            batteryMessageShown = false;
            inputManager.EnableMovement();
            return;
        }

        if (farmMessageShown)
        {
            farmMessageShown = false;
            tooltip.ShowTooltip("Aperte M para abrir o mapa");
            inputManager.EnableMovement();
            return;
        }

        // Coordenar as ações após cada mensagem de ajuda
        if (cameraMovementCompleted)
        {
            if (!objectiveSystem.HasObjective("Bateria"))
            {
                objectiveSystem.SetMissionTitle("Bateria"); // Define o título da missão
                objectiveSystem.AddObjective("Bateria", "Recarregue a bateria na cápsula."); // Passa título e descrição
            }
            tooltip.ShowTooltip("Use WASD para se movimentar");
            inputManager.EnableMovement(); // Ativar apenas a movimentação
        }
        else
        {
            tooltip.ShowTooltip("Use o mouse para olhar em volta");
            inputManager.EnableCameraMovement(); // Ativar apenas a movimentação da câmera
            showedCameraMovementTooltip = true;
        }
    }

    public void OnObjectiveCompleted(string objectiveDescription)
    {
        if (isGamePaused) return; // Não fazer nada se o jogo estiver pausado

        if (objectiveDescription == "Recarregue a bateria na cápsula.")
        {
            batteryMissionCompleted = true;
            helpSystem.AddHelpMessage("Volte para a fazenda. Há muitas tarefas a serem feitas.");
            tooltip.ShowTooltip("Aperte M para abrir o mapa");
            inputManager.ResetInputs();
            batteryMessageShown = true;

            // Informar ao PlayerManager que o tutorial foi concluído
            PlayerPrefs.SetInt("TutorialConcluido", 1);
            PlayerPrefs.Save();

            // Limpar objetivos e definir novos
            objectiveSystem.ClearObjectives();
            objectiveSystem.SetMissionTitle("Fazenda");
            objectiveSystem.AddObjective("Fazenda", "Vá para a fazenda.");
            // Ativar o ícone da fazenda no mapa
            if (farmIcon != null)
            {
                farmIcon.SetActive(true);
            }

            // Marcar a mensagem da fazenda como mostrada
            farmMessageShown = true;
        }
    }

    public bool IsTutorialCompleted()
    {
        return batteryMissionCompleted; // Retorna verdadeiro se a missão da bateria foi concluída
    }

    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }
}
