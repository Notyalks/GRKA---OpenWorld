using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TooltipManager tooltip;
    public HelpSystemAssistant helpSystem;
    public InputManager inputManager;
    public ObjectiveSystem objectiveSystem;
    public GameObject farmIcon;

    private bool cameraMovementCompleted = false;
    private bool showedCameraMovementTooltip = false;
    private bool triggerActivated = false;
    private bool bridgeMessageShown = false;
    private bool batteryMessageShown = false;
    private bool farmMessageShown = false;

    private bool isGamePaused = false;

    void Start()
    {
        helpSystem.AddHelpMessage("Resfriando sistema... tudo bem GARiK?");
        inputManager.ResetInputs();

        if (farmIcon != null)
        {
            farmIcon.SetActive(false);
        }

        // Inicia a missão do tutorial
        StartTutorialMission();
    }

    private void StartTutorialMission()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.SetMission("Tutorial");
            objectiveSystem.SetMissionTitle("Tutorial");
            objectiveSystem.AddObjective("Tutorial", "Complete o tutorial inicial.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGamePaused) return;

        if (other.CompareTag("Player") && !triggerActivated)
        {
            helpSystem.AddHelpMessage("Atenção!!! Há minérios extremamente radioativos na frente. Atire para destruí-los.");
            tooltip.ShowTooltip("Use mouse 1 para atirar");
            inputManager.ResetInputs();
            triggerActivated = true;
            bridgeMessageShown = true;
        }
    }

    void Update()
    {
        if (isGamePaused) return;

        if (!cameraMovementCompleted && showedCameraMovementTooltip)
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                cameraMovementCompleted = true;
                helpSystem.AddHelpMessage("Não esqueça de sua bateria GARiK. Vá para a floresta recarregá-la.");
                inputManager.DisableCameraMovement();
                SetBatteryMission();
            }
        }
    }

    private void SetBatteryMission()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.SetMission("Bateria");
            objectiveSystem.SetMissionTitle("Bateria");
            objectiveSystem.ClearObjectives();
            objectiveSystem.AddObjective("Bateria", "Recarregue a bateria na cápsula.");
        }
    }

    public void OnHelpMessageClosed()
    {
        if (isGamePaused || helpSystem.isDisplayingMessage)
            return;

        if (bridgeMessageShown)
        {
            bridgeMessageShown = false;
            inputManager.EnableMovement();
            return;
        }

        if (batteryMessageShown)
        {
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

        if (cameraMovementCompleted)
        {
            tooltip.ShowTooltip("Use WASD para se movimentar");
            inputManager.EnableMovement();
        }
        else
        {
            tooltip.ShowTooltip("Use o mouse para olhar em volta");
            inputManager.EnableCameraMovement();
            showedCameraMovementTooltip = true;
        }
    }

    public void OnObjectiveCompleted(string objectiveDescription)
    {
        if (isGamePaused) return;

        if (objectiveDescription == "Recarregue a bateria na cápsula.")
        {
            helpSystem.AddHelpMessage("Siga as pedras rosas para voltar para fazenda. Há muitas tarefas a serem feitas.");
            tooltip.ShowTooltip("Aperte M para abrir o mapa");
            inputManager.ResetInputs();

            PlayerPrefs.SetInt("TutorialConcluido", 1);
            PlayerPrefs.Save();

            objectiveSystem.ClearObjectives();
            objectiveSystem.SetMissionTitle("Fazenda");
            objectiveSystem.AddObjective("Fazenda", "Vá para a fazenda.");
            if (farmIcon != null)
            {
                farmIcon.SetActive(true);
            }

            farmMessageShown = true;

            // Define a missão de ir para a fazenda
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.SetMission("Ir para a fazenda");
            }
        }
    }

    public bool IsTutorialCompleted()
    {
        return MissionManager.Instance != null && MissionManager.Instance.GetCurrentMission() == "Ir para a fazenda";
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
