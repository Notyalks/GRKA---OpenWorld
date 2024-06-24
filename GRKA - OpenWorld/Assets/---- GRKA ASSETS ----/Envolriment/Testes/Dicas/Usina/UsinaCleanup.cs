using UnityEngine;

public class UsinaCleanup : MonoBehaviour
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
        objectiveSystem.AddObjective("Faxina 2", "Destrua todos os min�rios na usina");

        foreach (var obj in objectsToDestroy)
        {
            obj.SetActive(true);
        }

        if (!farmMessageShown)
        {
            helpSystem.HideHelpMessage();
            helpSystem.ShowHelpMessage("A usina tamb�m est� contaminada. Converse com os cientistas e destrua os min�rios.");
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
            helpSystem.AddHelpMessage("Voc� conseguiu destruir todos os min�rios, parab�ns!");
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
            helpSystem.AddHelpMessage("Voc� precisa verificar como a mina est�. Siga as pedras roxas e n�o esque�a de recarregar sua bateria!");
            missionManager.SetMission("Mineradores");
            objectiveSystem.SetMissionTitle("Mineradores");
            objectiveSystem.ClearObjectives();
            objectiveSystem.AddObjective("Mineradores", "V� para a mina.");
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
