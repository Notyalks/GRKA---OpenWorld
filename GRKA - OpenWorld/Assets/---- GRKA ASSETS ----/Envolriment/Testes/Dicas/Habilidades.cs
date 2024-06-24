using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public int shireIndex; // �ndice da Shire que este item representa (come�a de 0)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Coletar o item
            CollectItem();

            // Desativar o objeto do item ap�s a coleta (opcional)
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
                Debug.LogWarning("�ndice de Shire inv�lido.");
                break;
        }

        // Salvar as prefer�ncias do jogador
        PlayerPrefs.Save();
    }
}
