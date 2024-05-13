using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private LayerMask readLayerMask;

    private ObjectGrabbable objectGrabbable;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(objectGrabbable == null)
            {
                float pickUpDistance = 15f;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                        Debug.Log(objectGrabbable);
                    }
                }
            }
            else
            {
               objectGrabbable.Drop();
               objectGrabbable = null;
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
    }

    
}
