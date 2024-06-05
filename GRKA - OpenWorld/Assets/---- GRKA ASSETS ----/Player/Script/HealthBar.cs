using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    public Slider healthSlider;
    public GameObject healthSliderObject;

    [Header("Shield Bar")]
    public Slider shieldSlider;
    public GameObject shieldSliderObject;

    public void ColocarVidaMaxima(float vida)
    {
        healthSlider.maxValue = vida;
        healthSlider.value = vida;
    }

    public void AlterarVida(float vida)
    {
        healthSlider.value = vida;
    }

    public void SetMaxShield(float shield)
    {
        shieldSlider.maxValue = shield;
        shieldSlider.value = shield;
    }

    public void SetShield(float shield)
    {
        shieldSlider.value = shield;
    }

    public void ShowShieldBar(bool show)
    {
        shieldSliderObject.SetActive(show);
    }
}