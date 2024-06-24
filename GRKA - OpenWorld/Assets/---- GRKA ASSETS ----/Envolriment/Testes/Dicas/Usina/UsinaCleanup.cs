using UnityEngine;

public class UsinaCleanup : MonoBehaviour
{
    public ObjectiveSystem objectiveSystem; // Referência ao ObjectiveSystem
    public HelpSystemAssistant helpSystem; // Referência ao HelpSystemAssistant
    public MissionManager missionManager; // Referência ao MissionManager
    public InputManager inputManager; // Referência ao InputManager

    [Header("Mission Objects")]
    public GameObject[] objectsToDestroy; // Lista de objetos a serem destruídos

    [Header("Completion Objects")]
    public GameObject completionObject1; // Primeiro objeto a ser ativado quando a missão for concluída
    public GameObject completionObject2; // Segundo objeto a ser ativado quando a missão for concluída

    private bool missionStarted = false; // Indica se a missão foi iniciada
    private bool missionCompleted = false; // Indica se a missão foi concluída
    private bool farmMessageShown = false; // Indica se a mensagem da fazenda foi mostrada
    private bool usinaMessageShown = false; // Indica se a mensagem da usina foi mostrada
    private bool allObjectsDestroyed = false; // Indica se todos os objetos foram destruídos

    void Start()
    {
        if (missionManager.GetCurrentMission() == "Faxina 2")
        {
            StartCleanupPhase();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && missionManager.GetCurrentMission() == "Usina")
        {
            StartCleanupPhase();
        }
    }

    void Update()
    {
        if (missionStarted && !allObjectsDestroyed)
        {
            CheckAllObjectsDestroyed();
        }
    }

    private void StartCleanupPhase()
    {
        missionStarted = true;
        missionManager.SetMission("Faxina 2");
        objectiveSystem.SetMissionTitle("Faxina 2");
        objectiveSystem.ClearObjectives();
        objectiveSystem.AddObjective("Faxina 2", "Destrua todos os minérios na usina");

        foreach (var obj in objectsToDestroy)
        {
            obj.SetActive(true);
        }

        if (!farmMessageShown)
        {
            helpSystem.HideHelpMessage();
            helpSystem.ShowHelpMessage("A usina também está contaminada. Converse com os cientistas e destrua os minérios.");
            farmMessageShown = true;
            inputManager.ResetInputs();
        }
    }

    private void CheckAllObjectsDestroyed()
    {
        allObjectsDestroyed = true;
        foreach (var obj in objectsToDestroy)
        {
            if (obj != null && obj.activeSelf)
            {
                allObjectsDestroyed = false;
                break;
            }
        }

        if (allObjectsDestroyed)
        {
            OnAllObjectsDestroyed();
        }
    }

    private void OnAllObjectsDestroyed()
    {
        if (!missionCompleted)
        {
            helpSystem.AddHelpMessage("Você conseguiu destruir todos os minérios, parabéns!");
            missionCompleted = true;
            inputManager.ResetInputs();

            // Ativa os objetos referenciais
            if (completionObject1 != null)
            {
                completionObject1.SetActive(true);
            }
            if (completionObject2 != null)
            {
                completionObject2.SetActive(true);
            }
        }

        if (!usinaMessageShown)
        {
            helpSystem.AddHelpMessage("Você precisa verificar como a mina está. Siga as pedras roxas e não esqueça de recarregar sua bateria!");
            missionManager.SetMission("Mineradores");
            objectiveSystem.SetMissionTitle("Mineradores");
            objectiveSystem.ClearObjectives();
            objectiveSystem.AddObjective("Mineradores", "Vá para a mina.");
            usinaMessageShown = true;
        }
    }

    public void OnHelpMessageClosed()
    {
        if (helpSystem.isDisplayingMessage)
            return;

        if (farmMessageShown && !missionCompleted)
        {
            farmMessageShown = false;
            inputManager.EnableMovement();
            return;
        }

        if (missionCompleted && !usinaMessageShown)
        {
            usinaMessageShown = false;
            inputManager.EnableMovement();
            return;
        }
    }
}
