using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamps : MonoBehaviour
{
    public Light lamp;
    public MeshRenderer mr;
    public Material oldMaterial;
    public Material newMaterial;

   
    
    public void TurnOnLight()
    {
        mr.material = newMaterial;
        lamp.intensity = 300;
    }

    public void TurnOffLight()
    {
        mr.material = oldMaterial;
        lamp.intensity = 0;
    }
}
