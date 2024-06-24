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
            helpSystem.ShowHelpMessage("A barreira de proteção está ativada. Você tem que achar alguma forma de desativar ela.");
            helpMessageShown = true;

            // Define como trigger foi ativado para evitar repetições
            triggered = true;
        }
    }

    private void Update()
    {
        // Verifica se a mensagem de ajuda foi fechada pelo jogador (clicando com o mouse)
        if (helpMessageShown && Input.GetMouseButtonDown(0)) // 0 significa botão esquerdo do mouse
        {
            // Fecha a mensagem do HelpSystemAssistant
            helpSystem.HideHelpMessage();

            // Mostra o tooltip após fechar a mensagem
            tooltip.ShowTooltip("Aperte E para empurrar caixa");
        }
    }
}
