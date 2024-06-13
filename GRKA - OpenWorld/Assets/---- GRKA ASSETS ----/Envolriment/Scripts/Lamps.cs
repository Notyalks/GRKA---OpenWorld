using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamps : MonoBehaviour
{
    public Light lamp;
    public MeshRenderer mr;
    public Material oldMaterial;
    public Material newMaterial;
    public float lightIntensity = 300;  // Vari�vel p�blica para definir a intensidade da luz para cada l�mpada
    public float lightRange = 10;       // Vari�vel p�blica para definir o range da luz para cada l�mpada

    public void TurnOnLight()
    {
        mr.material = newMaterial;
        lamp.intensity = 100;
        lamp.intensity = lightIntensity;  // Usando a vari�vel lightIntensity para definir a intensidade da luz
        lamp.range = lightRange;          // Usando a vari�vel lightRange para definir o range da luz
    }

    public void TurnOffLight()
    {
        mr.material = oldMaterial;
        lamp.intensity = 0;
        lamp.range = 0;                   // Opcionalmente, redefina o range para 0 ao desligar a luz
    }
}
