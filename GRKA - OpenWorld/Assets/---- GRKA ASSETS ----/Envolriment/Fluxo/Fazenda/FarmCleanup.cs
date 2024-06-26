using UnityEngine;

public class FarmCleanup : MonoBehaviour
{
    public ObjectiveSystem objectiveSystem; // Referência ao ObjectiveSystem
    public HelpSystemAssistant helpSystem; // Referência ao HelpSystemAssistant
    public MissionManager missionManager; // Referência ao MissionManager
    public MissionDestroyObjects missionDestroyObjects; // Referência ao MissionDestroyObjects

    private bool cleanupStarted = false; // Indica se a fase de limpeza já foi iniciada
    private bool missionChangedToCleanup = false; // Indica se a missão já foi alterada para "Faxina"
    private bool cleanupPhaseCompleted = false; // Indica se a fase de limpeza foi concluída

    private bool message1Shown = false; // Indica se a mensagem inicial foi mostrada
    private bool message2Shown = false; // Indica se a mensagem de parabéns foi mostrada

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cleanupStarted && missionManager.GetCurrentMission() == "Ir para a fazenda")
        {
            // Inicia a fase de limpeza apenas quando o jogador chegar à fazenda
            StartCleanupPhase();
        }
    }

    void Update()
    {
        if (cleanupStarted && !missionChangedToCleanup && missionManager.GetCurrentMission() == "Faxina")
        {
            // Verifica se todos os objetos foram destruídos
            if (AreAllObjectsDestroyed())
            {
                // Chama quando todos os objetos foram destruídos
                OnAllObjectsDestroyed();
            }
        }
    }

    private void StartCleanupPhase()
    {
        cleanupStarted = true;

        // Define o título da missão para "Faxina" e o objetivo para "Destrua todos os minérios na fazenda"
        objectiveSystem.SetMissionTitle("Faxina");
        objectiveSystem.AddObjective("Faxina", "Destrua todos os minérios na fazenda.");

        // Notifica o MissionDestroyObjects para iniciar a contagem dos objetos
        missionDestroyObjects.enabled = true; // Ativa o script para começar a monitorar os objetos

        // Exibe mensagem no HelpSystem
        if (!message1Shown)
        {
            helpSystem.AddHelpMessage("Wow! A fazenda está contaminada. Converse com os fazendeiros e destrua os minérios.");
            message1Shown = true;
        }

        // Indica que a missão foi alterada para "Faxina"
        missionChangedToCleanup = true;

        // Adicione aqui qualquer lógica adicional ao iniciar a fase de limpeza
    }

    private bool AreAllObjectsDestroyed()
    {
        // Implemente a lógica para verificar se todos os objetos foram destruídos
        // Aqui você deve verificar os critérios necessários para determinar se a fase de destruição foi concluída
        // Exemplo: você pode iterar pelos objetos ou usar uma lista para verificar seu estado

        // Este é um exemplo genérico, substitua pela lógica real necessária para o seu jogo
        foreach (var obj in missionDestroyObjects.objectsToDestroy)
        {
            if (obj != null && obj.activeSelf)
            {
                return false; // Ainda há objetos não destruídos
            }
        }

        return true; // Todos os objetos foram destruídos
    }

    private void OnAllObjectsDestroyed()
    {
        // Chamado quando todos os objetos foram destruídos
        helpSystem.AddHelpMessage("Você conseguiu destruir todos os minérios, parabéns.");

        // Muda a missão para "Usina"
        missionManager.SetMission("Usina");
        objectiveSystem.SetMissionTitle("Usina");
        objectiveSystem.AddObjective("Usina", "Vá para a usina.");

        // Indica que a fase de limpeza foi concluída
        cleanupPhaseCompleted = true;

        // Adicione aqui qualquer lógica adicional ao completar a fase de limpeza
    }
}
