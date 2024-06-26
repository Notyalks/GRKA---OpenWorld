using UnityEngine;

public class FarmTrigger : MonoBehaviour
{
    public ObjectiveSystem objectiveSystem; // Referência ao ObjectiveSystem
    public HelpSystemAssistant helpSystem; // Referência ao HelpSystemAssistant
    public MissionManager missionManager; // Referência ao MissionManager

    private bool missionStarted = false; // Indica se a missão já foi iniciada

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !missionStarted)
        {
            missionStarted = true;

            // Verifica se a missão atual é "Ir para a fazenda"
            if (missionManager != null && missionManager.GetCurrentMission() == "Ir para a fazenda")
            {
                // Completa a missão atual
                missionManager.CompleteMission();

                // Completa o objetivo no ObjectiveSystem
                if (objectiveSystem != null)
                {
                    objectiveSystem.CompleteObjective("Fazenda: Vá para a fazenda.");
                }

                // Inicia a sequência narrativa
                StartFarmSequence();
            }
        }
    }

    private void StartFarmSequence()
    {
        // Exibe a mensagem inicial no HelpSystem
        helpSystem.AddHelpMessage("Wow! A fazenda está contaminada. Converse com os fazendeiros e destrua os minérios.");

        // Adicione aqui qualquer lógica adicional ao iniciar a sequência
    }
}
