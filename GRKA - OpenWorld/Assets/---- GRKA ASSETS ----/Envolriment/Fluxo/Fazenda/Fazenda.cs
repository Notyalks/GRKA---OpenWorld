using UnityEngine;

public class FarmMissionController : MonoBehaviour
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
        if (missionManager.GetCurrentMission() == "Faxina")
        {
            StartCleanupPhase();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && missionManager.GetCurrentMission() == "Ir para a fazenda")
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
        missionManager.SetMission("Faxina");
        objectiveSystem.SetMissionTitle("Faxina");
        objectiveSystem.ClearObjectives();
        objectiveSystem.AddObjective("Faxina", "Destrua todos os minérios na fazenda");

        foreach (var obj in objectsToDestroy)
        {
            obj.SetActive(true);
        }

        if (!farmMessageShown)
        {
            helpSystem.HideHelpMessage();
            helpSystem.ShowHelpMessage("Wow! A fazenda está contaminada. Converse com os fazendeiros e destrua os minérios.");
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
            helpSystem.AddHelpMessage("Vai ver como a Usina está. Segue reto o caminho de terra e não esqueça de recarregar sua bateria!");
            missionManager.SetMission("Usina");
            objectiveSystem.SetMissionTitle("Usina");
            objectiveSystem.ClearObjectives();
            objectiveSystem.AddObjective("Usina", "Vá para a usina.");
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
