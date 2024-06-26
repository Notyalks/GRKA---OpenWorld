using UnityEngine;


public class MapTrigger : MonoBehaviour
{
    private TooltipManager tooltip; // Refer?ncia ao TooltipManager
    private bool tooltipShown = false; // Vari?vel para controlar se o tooltip foi mostrado

    void Start()
    {
        // Encontra o TooltipManager na cena
        tooltip = FindObjectOfType<TooltipManager>();
        if (tooltip == null)
        {
            Debug.LogError("TooltipManager n?o encontrado na cena.");
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
                tooltip.ShowTooltip("Aperte M para abrir o mapa");
                tooltipShown = true; // Marca o tooltip como mostrado
            }
        }
    }
}

