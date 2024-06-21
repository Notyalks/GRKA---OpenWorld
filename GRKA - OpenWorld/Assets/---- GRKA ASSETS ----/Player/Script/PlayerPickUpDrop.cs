using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private LayerMask readLayerMask;
    [SerializeField] private Animator animator;

    private ObjectGrabbable objectGrabbable;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
                float pickUpDistance = 15f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                        animator.SetBool("isGrabbing", true); // Ativa a animação de pegar
                        Debug.Log(objectGrabbable);
                    }
                }
            }
            else
            {
                objectGrabbable.Drop();
                objectGrabbable = null;
                animator.SetBool("isGrabbing", false); // Desativa a animação de pegar
            }
        }

        float readDistance = 10f;

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit ray, readDistance, readLayerMask))
        {
            if (ray.collider.gameObject.GetComponent<Letter>())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Entrou");
                    ray.collider.gameObject.GetComponent<Letter>().openCloseLetter();
                }
            }
            
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hit.distance, Color.yellow);

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance <= 10f)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.transform.GetComponent<KeypadKey>() != null)
                    {
                        hit.transform.GetComponent<KeypadKey>().SendKey();
                    }
                }
            }
        }

    } 
}
