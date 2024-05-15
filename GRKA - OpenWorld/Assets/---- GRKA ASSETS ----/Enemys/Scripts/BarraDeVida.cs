using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    [SerializeField] private Image barraDeVidaImage;
    private Transform Cam;

    private void Awake()
    {
        Cam = Camera.main.transform;
    }

  
    void Update()
    {
      transform.LookAt(transform.position + Cam.forward);  
    }

    public void AlterarBarraDeVida(int vidaAtual, int vidaMaxima)
    {
        barraDeVidaImage.fillAmount = (float) vidaAtual / vidaMaxima;
    }
}
