using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{

    private Rigidbody objRb;
    private Transform objectGrabPointTransform;


    private void Awake()
    {
        objRb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objRb.useGravity = false;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objRb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if(objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
           Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objRb.MovePosition(newPosition);
        }
    }
}
