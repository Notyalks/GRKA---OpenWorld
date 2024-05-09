using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receptor : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Light lightComponent;
    
    public Material greenMaterial;
    public Material redMaterial;
    public Material defaultMaterial;
    public Color greenColor = Color.green;
    public Color redColor = Color.red;

    public bool resolved = false;
    public bool ok = false;

    public static List<Receptor> resolvedObjects = new List<Receptor>();

    public GameObject itemHab;
    public Transform spawnPoint;

    private void Start()
    {
        resolved = false;
        ok = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            resolved = true;
            resolvedObjects.Add(this);
            if (resolvedObjects.Count == 8) // Substitua totalObjectsToResolve pelo número total de objetos que você espera resolver
            {
                CheckAllResolved();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            resolved = false;
            resolvedObjects.Remove(this);
        }

    }

    private void Update()
    {
        if(resolved == true)
        {
            lightComponent.color = greenColor;
            meshRenderer.material = greenMaterial;
        }
        else
        {
            lightComponent.color = redColor;
            meshRenderer.material = redMaterial;
        }
    }


    private void CheckAllResolved()
    {
      if (!ok)
      {
        foreach (Receptor obj in resolvedObjects)
        {
            if (!obj.resolved)
            {
                return;
            }
        }
        
        Vector3 spawnPosition = spawnPoint.transform.position;
        Instantiate(itemHab, spawnPosition, Quaternion.identity);
        ok = true;
        Debug.Log("Todos os objetos foram resolvidos!");
      }
    }
}
