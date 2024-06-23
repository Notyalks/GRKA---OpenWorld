using UnityEngine;

public class LanternTooltipTrigger : MonoBehaviour
{
    private TooltipManager tooltip; // Referência ao TooltipManager
    private bool tooltipShown = false; // Variável para controlar se o tooltip foi mostrado

    void Start()
    {
        // Encontra o TooltipManager na cena
        tooltip = FindObjectOfType<TooltipManager>();
        if (tooltip == null)
        {
            Debug.LogError("TooltipManager não encontrado na cena.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verifica se o jogador entrou no trigger
        if (other.CompareTag("Player") && !tooltipShown)
        {
            if (tooltip != null)
            {
                // Mostrar o tooltip
                tooltip.ShowTooltip("Aperte Q para ligar a lanterna");
                tooltipShown = true; // Marca o tooltip como mostrado
            }
        }
    }
}
