using UnityEngine;

public class PushPullController : MonoBehaviour
{
    public string pushTag = "Push"; // Tag dos objetos empurráveis
    public Transform holdPosition; // A posição onde o objeto será mantido
    public float pushDistance = 2f; // A distância máxima do Raycast
    private GameObject currentObject = null;
    private PlayerLocomotion playerLocomotion;
    private Animator animator;
    private bool isPushing = false;
    private int boxState = 0; // 0 - Idle, 1 - IdleOnBox, 2 - PushBox, 3 - PullBox

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPushing)
            {
                TryToPushObject();
            }
            else
            {
                ReleaseObject();
            }
        }

        // Atualizar boxState com base no estado de isPushing
        if (!isPushing)
        {
            boxState = 0; // Idle
        }
        else if (isPushing)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                boxState = 2; // Empurrar (push)
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
            {
                boxState = 3; // Puxar (pull)
            }
            else
            {
                boxState = 1; // Idle na caixa
            }
        }

        UpdateAnimator();
    }

    void TryToPushObject()
    {
        // Apenas tentar empurrar se não estivermos empurrando nada
        if (currentObject != null) return;

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
                playerLocomotion.SetIsPushing(true); // Ativar a animação de empurrar no PlayerLocomotion
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
            playerLocomotion.SetIsPushing(false); // Desativar a animação de empurrar no PlayerLocomotion
            boxState = 0; // Voltar para o estado de Idle do jogador
        }
    }

    void UpdateAnimator()
    {
        animator.SetInteger("boxState", boxState);
    }
}
