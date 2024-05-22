using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLife : MonoBehaviour
{
    public Slider slider1;

    public void BossColocarVidaMaxima(float vida)
    {
        slider1.maxValue = vida;
        slider1.value = vida;
    }

    public void BossAlterarVida(float vida)
    {
        slider1.value = vida;
    }
}
