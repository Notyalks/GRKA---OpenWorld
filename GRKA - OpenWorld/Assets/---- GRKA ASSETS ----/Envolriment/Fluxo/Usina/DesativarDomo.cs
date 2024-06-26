using UnityEngine;

public class CaixaEmpurravel : MonoBehaviour
{
    public GameObject objetoADesativar; // Refer�ncia ao objeto que ser� desativado

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
                Debug.LogWarning("Nenhum objeto para desativar foi atribu�do!");
            }
        }
    }
}
