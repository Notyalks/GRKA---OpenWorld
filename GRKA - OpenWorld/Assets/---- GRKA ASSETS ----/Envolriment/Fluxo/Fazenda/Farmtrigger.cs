using UnityEngine;

public class FarmTrigger : MonoBehaviour
{
    public ObjectiveSystem objectiveSystem; // Refer�ncia ao ObjectiveSystem
    public HelpSystemAssistant helpSystem; // Refer�ncia ao HelpSystemAssistant
    public MissionManager missionManager; // Refer�ncia ao MissionManager

    private bool missionStarted = false; // Indica se a miss�o j� foi iniciada

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !missionStarted)
        {
            missionStarted = true;

            // Verifica se a miss�o atual � "Ir para a fazenda"
            if (missionManager != null && missionManager.GetCurrentMission() == "Ir para a fazenda")
            {
                // Completa a miss�o atual
                missionManager.CompleteMission();

                // Completa o objetivo no ObjectiveSystem
                if (objectiveSystem != null)
                {
                    objectiveSystem.CompleteObjective("Fazenda: V� para a fazenda.");
                }

                // Inicia a sequ�ncia narrativa
                StartFarmSequence();
            }
        }
    }

    private void StartFarmSequence()
    {
        // Exibe a mensagem inicial no HelpSystem
        helpSystem.AddHelpMessage("Wow! A fazenda est� contaminada. Converse com os fazendeiros e destrua os min�rios.");

        // Adicione aqui qualquer l�gica adicional ao iniciar a sequ�ncia
    }
}
