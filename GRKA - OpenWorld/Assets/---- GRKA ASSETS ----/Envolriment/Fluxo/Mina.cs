using UnityEngine;

public class MinaCleanup : MonoBehaviour
{
    public ObjectiveSystem objectiveSystem; // Refer�ncia ao ObjectiveSystem
    public HelpSystemAssistant helpSystem; // Refer�ncia ao HelpSystemAssistant
    public MissionManager missionManager; // Refer�ncia ao MissionManager
    public InputManager inputManager; // Refer�ncia ao InputManager

    [Header("Mission Objects")]
    public GameObject[] objectsToDestroy; // Lista de objetos a serem destru�dos

    [Header("Completion Objects")]
    public GameObject completionObject1; // Primeiro objeto a ser ativado quando a miss�o for conclu�da
    public GameObject completionObject2; // Segundo objeto a ser ativado quando a miss�o for conclu�da

    private bool missionStarted = false; // Indica se a miss�o foi iniciada
    private bool missionCompleted = false; // Indica se a miss�o foi conclu�da
    private bool farmMessageShown = false; // Indica se a mensagem da fazenda foi mostrada
    private bool usinaMessageShown = false; // Indica se a mensagem da usina foi mostrada
    private bool allObjectsDestroyed = false; // Indica se todos os objetos foram destru�dos

    void Start()
    {
        if (missionManager.GetCurrentMission() == "Faxina 3")
        {
            StartCleanupPhase();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && missionManager.GetCurrentMission() == "Mineradores")
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
        missionManager.SetMission("Faxina 3");
        objectiveSystem.SetMissionTitle("Faxina 3");
        objectiveSystem.ClearObjectives();
        objectiveSystem.AddObjective("Faxina 3", "Destrua todos os min�rios na mina");

        foreach (var obj in objectsToDestroy)
        {
            obj.SetActive(true);
        }

        if (!farmMessageShown)
        {
            helpSystem.HideHelpMessage();
            helpSystem.ShowHelpMessage("A Mina est� infestada. Converse com os mineradores e destrua os min�rios.");
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
            helpSystem.AddHelpMessage("Voc� conseguiu destruir todos. Parab�ns!");
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
            helpSystem.AddHelpMessage("Se aqui estava desse jeito imagina dentro da caverna, Voc� tem que averiguar");
            missionManager.SetMission("Caverna");
            objectiveSystem.SetMissionTitle("Caverna");
            objectiveSystem.ClearObjectives();
            objectiveSystem.AddObjective("Caverna", "Pegue o portal para a caverna.");
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
