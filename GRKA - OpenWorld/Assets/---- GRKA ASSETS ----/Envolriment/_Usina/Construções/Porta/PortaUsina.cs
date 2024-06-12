using UnityEngine;

public class PortaUsina : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    void OnTriggerEnter(Collider _col)
    {
        if (_col.gameObject.CompareTag("Player"))
        {
            myDoor.SetBool("Abrir", true);
            myDoor.SetBool("Fechar", false);
        }
    }

    void OnTriggerExit(Collider _col)
    {
        if (_col.gameObject.CompareTag("Player"))
        {
            myDoor.SetBool("Fechar", true);
            myDoor.SetBool("Abrir", false);
        }
    }
}
