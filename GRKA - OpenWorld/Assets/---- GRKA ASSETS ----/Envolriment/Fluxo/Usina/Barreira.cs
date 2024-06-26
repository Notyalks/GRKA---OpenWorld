using UnityEngine;

public class BarrierTrigger : MonoBehaviour
{
    public HelpSystemAssistant helpSystem;
    public TooltipManager tooltip;

    private bool triggered = false;
    private bool helpMessageShown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            // Mostra a mensagem no HelpSystemAssistant
            helpSystem.ShowHelpMessage("A barreira de prote��o est� ativada. Voc� tem que achar alguma forma de desativar ela.");
            helpMessageShown = true;

            // Define como trigger foi ativado para evitar repeti��es
            triggered = true;
        }
    }

    private void Update()
    {
        // Verifica se a mensagem de ajuda foi fechada pelo jogador (clicando com o mouse)
        if (helpMessageShown && Input.GetMouseButtonDown(0)) // 0 significa bot�o esquerdo do mouse
        {
            // Fecha a mensagem do HelpSystemAssistant
            helpSystem.HideHelpMessage();

            // Mostra o tooltip ap�s fechar a mensagem
            tooltip.ShowTooltip("Aperte E para empurrar caixa");
        }
    }
}
