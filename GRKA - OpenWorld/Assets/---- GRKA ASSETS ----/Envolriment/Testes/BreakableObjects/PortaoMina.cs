using UnityEngine;

public class PortaController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AbrirPorta();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            FecharPorta();
        }
    }

    public void AbrirPorta()
    {
        animator.SetBool("isOpen", true);
        animator.SetBool("isClosed", false);
    }

    public void FecharPorta()
    {
        animator.SetBool("isOpen", false);
        animator.SetBool("isClosed", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AbrirPorta();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FecharPorta();
        }
    }
}
