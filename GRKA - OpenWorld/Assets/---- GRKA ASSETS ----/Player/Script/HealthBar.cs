using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void ColocarVidaMaxima(float vida)
    {
        slider.maxValue = vida;
        slider.value = vida;
    }

    public void AlterarVida(float vida)
    {
        slider.value = vida;
    }
}
