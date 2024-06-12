using UnityEngine;

public class PortaSimples : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    private bool _colidindo;
    private bool _portaAberta = false;

    [SerializeField] private string abrindo = "Abrindo";
    [SerializeField] private string fechando = "Fechando";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _colidindo)
        {
            if (_portaAberta)
            {
                myDoor.Play(fechando, 0, 0.0f);
                _portaAberta = false;
            }
            else
            {
                myDoor.Play(abrindo, 0, 0.0f);
                _portaAberta = true;
            }
        }
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.gameObject.CompareTag("Player"))
        {
            _colidindo = true;
        }
    }

    void OnTriggerExit(Collider _col)
    {
        if (_col.gameObject.CompareTag("Player"))
        {
            _colidindo = false;
        }
    }
}