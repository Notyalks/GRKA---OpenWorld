using System.Collections.Generic;
using UnityEngine;

public class MissionDestroyObjects : MonoBehaviour
{
    public List<GameObject> objectsToDestroy; // Lista de objetos que precisam ser destru�dos para completar a miss�o

    private int objectsDestroyedCount = 0; // Contador de objetos destru�dos

    void Start()
    {
        // Verifica se a lista de objetos est� vazia
        if (objectsToDestroy == null || objectsToDestroy.Count == 0)
        {
            Debug.LogWarning("Lista de objetos para destruir n�o foi configurada no inspector.");
        }
    }

    // M�todo para chamar quando um objeto da lista � destru�do
    public void ObjectDestroyed(GameObject destroyedObject)
    {
        // Verifica se o objeto destru�do est� na lista
        if (objectsToDestroy.Contains(destroyedObject))
        {
            objectsDestroyedCount++;

            // Remove o objeto da lista para evitar contagens duplicadas
            objectsToDestroy.Remove(destroyedObject);

            // Verifica se todos os objetos foram destru�dos
            if (objectsToDestroy.Count == 0)
            {
                MissionCompleted();
            }
        }
    }

    // M�todo para ser chamado quando todos os objetos forem destru�dos
    private void MissionCompleted()
    {
        Debug.Log("Todos os objetos foram destru�dos. Miss�o conclu�da!");

        // Aqui voc� pode adicionar qualquer a��o adicional que deseja realizar quando a miss�o � conclu�da
    }
}
