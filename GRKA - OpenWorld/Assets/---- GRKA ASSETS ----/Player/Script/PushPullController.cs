using UnityEngine;

public class PushPullController : MonoBehaviour
{
    public string pushTag = "Push"; // Tag dos objetos empurráveis
    public Transform holdPosition; // A posição onde o objeto será mantido
    public float pushDistance = 2f; // A distância máxima do Raycast
    private GameObject currentObject = null;
    private bool isPushing = false;
    private Animator animator;
    private PlayerLocomotion playerLocomotion;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPushing)
            {
                ReleaseObject();
            }
            else
            {
                TryToPushObject();
            }
        }
    }

    void TryToPushObject()
    {
        // Apenas tentar empurrar se não estivermos empurrando nada
        if (isPushing) return;

        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * pushDistance, Color.red, 1f); // Desenhar o Raycast para debug

        if (Physics.Raycast(origin, direction, out hit, pushDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            if (hit.collider.CompareTag(pushTag))
            {
                currentObject = hit.collider.gameObject;
                currentObject.transform.SetParent(holdPosition);
                currentObject.transform.localPosition = Vector3.zero;
                currentObject.transform.localRotation = Quaternion.identity;

                isPushing = true;
                playerLocomotion.SetIsPushing(true); // Define isPushing no PlayerLocomotion
                animator.SetBool("isPushing", true);
            }
        }
    }

    void ReleaseObject()
    {
        if (currentObject != null)
        {
            currentObject.transform.SetParent(null);
            currentObject = null;

            isPushing = false;
            playerLocomotion.SetIsPushing(false); // Define isPushing no PlayerLocomotion
            animator.SetBool("isPushing", false);
        }
    }
}
