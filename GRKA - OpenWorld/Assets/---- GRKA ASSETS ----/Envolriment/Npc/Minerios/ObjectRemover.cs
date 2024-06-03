using UnityEngine;

public class ObjectRemover : MonoBehaviour
{
    // Detectar entrada de trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é o que queremos remover
        if (other.gameObject.CompareTag("Removable"))
        {
            Debug.Log("Objeto removido: " + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
}
