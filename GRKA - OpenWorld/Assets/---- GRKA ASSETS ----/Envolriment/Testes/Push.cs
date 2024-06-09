using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : MonoBehaviour
{
    private InputManager inputManager;
    private Rigidbody rb;
    private bool isPushing = false;
    private FixedJoint fixedJoint;
    private GameObject objectBeingPushed;

    public LayerMask pushLayer;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isPushing)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("ReleaseObject");
                ReleaseObject();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, pushLayer))
                {
                    if (hit.transform.CompareTag("Pushable"))
                    {
                        Debug.Log("PushObject");
                        PushObject(hit.collider.gameObject);
                    }
                }
            }
        }

        if (isPushing)
        {
            MovePlayerWhilePushing();
        }
    }

    private void PushObject(GameObject pushableObject)
    {
        isPushing = true;
        objectBeingPushed = pushableObject;
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = pushableObject.GetComponent<Rigidbody>();
        fixedJoint.enablePreprocessing = false;
        fixedJoint.massScale = 1;
        fixedJoint.connectedMassScale = 1;
        fixedJoint.enableCollision = false; // Desabilita a colisão entre o jogador e o objeto empurrado
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        objectBeingPushed.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void ReleaseObject()
    {
        isPushing = false;
        Destroy(fixedJoint);
        rb.constraints = RigidbodyConstraints.None;
        objectBeingPushed.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void MovePlayerWhilePushing()
    {
        float moveSpeed = 50f; // Velocidade de movimento enquanto empurra
        Vector3 moveDirection = transform.forward * inputManager.verticalInput + transform.right * inputManager.horizontalInput;
        moveDirection.y = 0f; // Mantém o jogador na altura correta

        rb.velocity = moveDirection.normalized * moveSpeed;
    }
}
