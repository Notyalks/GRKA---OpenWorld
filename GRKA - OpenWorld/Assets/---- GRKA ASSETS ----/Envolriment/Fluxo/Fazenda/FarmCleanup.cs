using UnityEngine;

public class FarmCleanup : MonoBehaviour
{
    public ObjectiveSystem objectiveSystem; // Refer�ncia ao ObjectiveSystem
    public HelpSystemAssistant helpSystem; // Refer�ncia ao HelpSystemAssistant
    public MissionManager missionManager; // Refer�ncia ao MissionManager
    public MissionDestroyObjects missionDestroyObjects; // Refer�ncia ao MissionDestroyObjects

    private bool cleanupStarted = false; // Indica se a fase de limpeza j� foi iniciada
    private bool missionChangedToCleanup = false; // Indica se a miss�o j� foi alterada para "Faxina"
    private bool cleanupPhaseCompleted = false; // Indica se a fase de limpeza foi conclu�da

    private bool message1Shown = false; // Indica se a mensagem inicial foi mostrada
    private bool message2Shown = false; // Indica se a mensagem de parab�ns foi mostrada

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cleanupStarted && missionManager.GetCurrentMission() == "Ir para a fazenda")
        {
            // Inicia a fase de limpeza apenas quando o jogador chegar � fazenda
            StartCleanupPhase();
        }
    }

    void Update()
    {
        if (cleanupStarted && !missionChangedToCleanup && missionManager.GetCurrentMission() == "Faxina")
        {
            // Verifica se todos os objetos foram destru�dos
            if (AreAllObjectsDestroyed())
            {
                // Chama quando todos os objetos foram destru�dos
                OnAllObjectsDestroyed();
            }
        }
    }

    private void StartCleanupPhase()
    {
        cleanupStarted = true;

        // Define o t�tulo da miss�o para "Faxina" e o objetivo para "Destrua todos os min�rios na fazenda"
        objectiveSystem.SetMissionTitle("Faxina");
        objectiveSystem.AddObjective("Faxina", "Destrua todos os min�rios na fazenda.");

        // Notifica o MissionDestroyObjects para iniciar a contagem dos objetos
        missionDestroyObjects.enabled = true; // Ativa o script para come�ar a monitorar os objetos

        // Exibe mensagem no HelpSystem
        if (!message1Shown)
        {
            helpSystem.AddHelpMessage("Wow! A fazenda est� contaminada. Converse com os fazendeiros e destrua os min�rios.");
            message1Shown = true;
        }

        // Indica que a miss�o foi alterada para "Faxina"
        missionChangedToCleanup = true;

        // Adicione aqui qualquer l�gica adicional ao iniciar a fase de limpeza
    }

    private bool AreAllObjectsDestroyed()
    {
        // Implemente a l�gica para verificar se todos os objetos foram destru�dos
        // Aqui voc� deve verificar os crit�rios necess�rios para determinar se a fase de destrui��o foi conclu�da
        // Exemplo: voc� pode iterar pelos objetos ou usar uma lista para verificar seu estado

        // Este � um exemplo gen�rico, substitua pela l�gica real necess�ria para o seu jogo
        foreach (var obj in missionDestroyObjects.objectsToDestroy)
        {
            if (obj != null && obj.activeSelf)
            {
                return false; // Ainda h� objetos n�o destru�dos
            }
        }

        return true; // Todos os objetos foram destru�dos
    }

    private void OnAllObjectsDestroyed()
    {
        // Chamado quando todos os objetos foram destru�dos
        helpSystem.AddHelpMessage("Voc� conseguiu destruir todos os min�rios, parab�ns.");

        // Muda a miss�o para "Usina"
        missionManager.SetMission("Usina");
        objectiveSystem.SetMissionTitle("Usina");
        objectiveSystem.AddObjective("Usina", "V� para a usina.");

        // Indica que a fase de limpeza foi conclu�da
        cleanupPhaseCompleted = true;

        // Adicione aqui qualquer l�gica adicional ao completar a fase de limpeza
    }
}
