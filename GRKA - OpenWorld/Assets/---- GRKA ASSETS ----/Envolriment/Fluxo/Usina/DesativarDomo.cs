using UnityEngine;

public class CaixaEmpurravel : MonoBehaviour
{
    public GameObject objetoADesativar; // Referência ao objeto que será desativado

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Push"))
        {
            if (objetoADesativar != null)
            {
                objetoADesativar.SetActive(false);
                Debug.Log("Objeto desativado pelo trigger!");
            }
            else
            {
                Debug.LogWarning("Nenhum objeto para desativar foi atribuído!");
            }
        }
    }
}
