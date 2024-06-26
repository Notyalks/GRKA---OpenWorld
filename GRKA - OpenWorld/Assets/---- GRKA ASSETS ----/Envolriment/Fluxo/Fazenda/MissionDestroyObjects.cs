using System.Collections.Generic;
using UnityEngine;

public class MissionDestroyObjects : MonoBehaviour
{
    public List<GameObject> objectsToDestroy; // Lista de objetos que precisam ser destruídos para completar a missão

    private int objectsDestroyedCount = 0; // Contador de objetos destruídos

    void Start()
    {
        // Verifica se a lista de objetos está vazia
        if (objectsToDestroy == null || objectsToDestroy.Count == 0)
        {
            Debug.LogWarning("Lista de objetos para destruir não foi configurada no inspector.");
        }
    }

    // Método para chamar quando um objeto da lista é destruído
    public void ObjectDestroyed(GameObject destroyedObject)
    {
        // Verifica se o objeto destruído está na lista
        if (objectsToDestroy.Contains(destroyedObject))
        {
            objectsDestroyedCount++;

            // Remove o objeto da lista para evitar contagens duplicadas
            objectsToDestroy.Remove(destroyedObject);

            // Verifica se todos os objetos foram destruídos
            if (objectsToDestroy.Count == 0)
            {
                MissionCompleted();
            }
        }
    }

    // Método para ser chamado quando todos os objetos forem destruídos
    private void MissionCompleted()
    {
        Debug.Log("Todos os objetos foram destruídos. Missão concluída!");

        // Aqui você pode adicionar qualquer ação adicional que deseja realizar quando a missão é concluída
    }
}
