using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objRb;
    private Transform objectGrabPointTransform;
    private bool isBeingHeld = false;

    [Header("Destroy on Ground Impact")]
    public bool destroyOnGroundImpact = false; // Adicionar check no inspector
    public GameObject destructionParticlePrefab; // Prefab de partículas para instanciar na destruição
    public Collider terrainCollider; // Referência ao collider do terreno

    private void Awake()
    {
        objRb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        isBeingHeld = true;
        objRb.useGravity = false;
        objRb.isKinematic = false; // Mantenha o objeto dinâmico
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        isBeingHeld = false;
        objRb.useGravity = true;
        objRb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (isBeingHeld && objectGrabPointTransform != null)
        {
            Vector3 directionToGrabPoint = objectGrabPointTransform.position - transform.position;
            float distanceToGrabPoint = directionToGrabPoint.magnitude;
            float moveSpeed = 10f;

            objRb.velocity = directionToGrabPoint.normalized * moveSpeed * distanceToGrabPoint;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isBeingHeld)
        {
            // Parar o movimento ao colidir com algo
            objRb.velocity = Vector3.zero;
        }

        if (destroyOnGroundImpact && collision.collider == terrainCollider)
        {
            Instantiate(destructionParticlePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
