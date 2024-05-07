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

    public static List<Receptor> resolvedObjects = new List<Receptor>();

    private void Start()
    {
        resolved = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            resolved = true;
            resolvedObjects.Add(this);
            CheckAllResolved();

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
        foreach (Receptor obj in resolvedObjects)
        {
            if (!obj.resolved)
            {
                return;
            }
        }

        
        // Colocar condição de vitória
        Debug.Log("Todos os objetos foram resolvidos!");
    }
}
