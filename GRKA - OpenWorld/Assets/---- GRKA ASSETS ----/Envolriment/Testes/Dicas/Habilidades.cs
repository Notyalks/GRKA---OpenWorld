using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public int shireIndex; // Índice da Shire que este item representa (começa de 0)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Coletar o item
            CollectItem();

            // Desativar o objeto do item após a coleta (opcional)
            gameObject.SetActive(false);
        }
    }

    private void CollectItem()
    {
        // Atualizar o progresso da Shire correspondente
        switch (shireIndex)
        {
            case 0:
                PlayerPrefs.SetInt("Shire1Finished", 1);
                Debug.Log("Item coletado para Shire 1.");
                break;
            case 1:
                PlayerPrefs.SetInt("Shire2Finished", 1);
                Debug.Log("Item coletado para Shire 2.");
                break;
            case 2:
                PlayerPrefs.SetInt("Shire3Finished", 1);
                Debug.Log("Item coletado para Shire 3.");
                break;
            case 3:
                PlayerPrefs.SetInt("Shire4Finished", 1);
                Debug.Log("Item coletado para Shire 4.");
                break;
            default:
                Debug.LogWarning("Índice de Shire inválido.");
                break;
        }

        // Salvar as preferências do jogador
        PlayerPrefs.Save();
    }
}
