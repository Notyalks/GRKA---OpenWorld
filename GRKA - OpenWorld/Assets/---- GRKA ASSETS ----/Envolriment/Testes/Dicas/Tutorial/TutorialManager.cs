using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Refer�ncias de Componentes")]
    public TooltipManager tooltip; // Refer�ncia ao Tooltip
    public HelpSystemAssistant helpSystem; // Refer�ncia ao HelpSystem
    public InputManager inputManager; // Refer�ncia ao InputManager
    public ObjectiveSystem objectiveSystem; // Refer�ncia ao ObjectiveSystem
    public GameObject farmIcon; // Refer�ncia ao �cone da fazenda no mapa

    // Flags de controle de progresso
    private bool cameraMovementCompleted = false;
    private bool showedCameraMovementTooltip = false;
    private bool triggerActivated = false;
    private bool bridgeMessageShown = false;
    private bool batteryMissionCompleted = false;
    private bool batteryMessageShown = false;
    private bool farmMessageShown = false;

    private bool isGamePaused = false; // Controle do estado do jogo

    void Start()
    {
        // Iniciar o tutorial com a mensagem de boas-vindas
        helpSystem.AddHelpMessage("Resfriando sistema... tudo bem GARiK?");
        inputManager.ResetInputs(); // Desativa todos os inputs inicialmente

        // Certifique-se de que o �cone da fazenda esteja desativado no in�cio
        if (farmIcon != null)
        {
            farmIcon.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGamePaused || triggerActivated) return; // N�o fazer nada se o jogo estiver pausado ou o trigger j� foi ativado

        if (other.CompareTag("Player"))
        {
            helpSystem.AddHelpMessage("Aten��o!!! H� min�rios extremamente radioativos na frente. Atire para destru�-los.");
            tooltip.ShowTooltip("Use mouse 1 para atirar");
            inputManager.ResetInputs(); // Reseta os inputs para permitir que o jogador feche a mensagem manualmente com um clique do mouse
            triggerActivated = true; // Marca o trigger como ativado
            bridgeMessageShown = true; // Marca a mensagem dos min�rios como mostrada
        }
    }

    void Update()
    {
        if (isGamePaused) return; // N�o fazer nada se o jogo estiver pausado

        // Verificar se o jogador moveu a c�mera
        if (!cameraMovementCompleted && showedCameraMovementTooltip)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cameraMovementCompleted = true;
                helpSystem.AddHelpMessage("N�o esque�a de sua bateria GARiK. V� para a floresta recarreg�-la.");
                inputManager.DisableCameraMovement(); // Desativa a movimenta��o da c�mera ap�s conclu�do
            }
        }
    }

    public void OnHelpMessageClosed()
    {
        if (isGamePaused || helpSystem.isDisplayingMessage) return;

        if (bridgeMessageShown)
        {
            bridgeMessageShown = false; // Reseta a flag de mensagem dos min�rios
            inputManager.EnableMovement(); // Ativa a movimenta��o do jogador
            return;
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

        // Coordenar as a��es ap�s cada mensagem de ajuda
        if (cameraMovementCompleted)
        {
            if (!objectiveSystem.HasObjective("Bateria"))
            {
                objectiveSystem.SetMissionTitle("Bateria"); // Define o t�tulo da miss�o
                objectiveSystem.AddObjective("Bateria", "Recarregue a bateria na c�psula."); // Passa t�tulo e descri��o
            }
            tooltip.ShowTooltip("Use WASD para se movimentar");
            inputManager.EnableMovement(); // Ativar apenas a movimenta��o
        }
        else
        {
            tooltip.ShowTooltip("Use o mouse para olhar em volta");
            inputManager.EnableCameraMovement(); // Ativar apenas a movimenta��o da c�mera
            showedCameraMovementTooltip = true;
        }
    }

    public void OnObjectiveCompleted(string objectiveDescription)
    {
        if (isGamePaused) return; // N�o fazer nada se o jogo estiver pausado

        if (objectiveDescription == "Recarregue a bateria na c�psula.")
        {
            batteryMissionCompleted = true;
            helpSystem.AddHelpMessage("Volte para a fazenda. H� muitas tarefas a serem feitas.");
            tooltip.ShowTooltip("Aperte M para abrir o mapa");
            inputManager.ResetInputs();
            batteryMessageShown = true;

            // Informar ao PlayerManager que o tutorial foi conclu�do
            PlayerPrefs.SetInt("TutorialConcluido", 1);
            PlayerPrefs.Save();

            // Limpar objetivos e definir novos
            objectiveSystem.ClearObjectives();
            objectiveSystem.SetMissionTitle("Fazenda");
            objectiveSystem.AddObjective("Fazenda", "V� para a fazenda.");

            // Ativar o �cone da fazenda no mapa
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
        return batteryMissionCompleted; // Retorna verdadeiro se a miss�o da bateria foi conclu�da
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
